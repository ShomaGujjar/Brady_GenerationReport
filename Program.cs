// See https://aka.ms/new-console-template for more information

using System.Xml.Serialization;
using Brady_GenerationReport;
using Brady_GenerationReport.Input;
using Brady_GenerationReport.OutPut;
using Brady_GenerationReport.Utilities;
using Day = Brady_GenerationReport.Output.Day;

Console.WriteLine("Hello, Brady!");
var path = AppsettingReader.ReadSection<ConfigLocations>("ConfigLocations");

CalculateTotalGenerationValue(path);

static void CalculateTotalGenerationValue(ConfigLocations? path)
{
    Console.WriteLine("Calculating Total Generation Value!");
    MonitorDirectory(path);
    Console.ReadKey();
}

static void MonitorDirectory(ConfigLocations? path)
{
    Console.WriteLine("Waiting for input file to be dropped.....");
    if (path != null)
    {
        FileSystemWatcher fileSystemWatcher = new FileSystemWatcher
        {
            Path = path.Input,
            Filter = "*.xml",
            EnableRaisingEvents = true
        };
        fileSystemWatcher.Created += FileSystemWatcherCreated;
    }
}

static void FileSystemWatcherCreated(object sender, FileSystemEventArgs e)
{
    var fileDrop = e.Name;
    Console.WriteLine("File created: {0}", fileDrop);
    var report = ParsingGenerationReport(e.FullPath);
    //once valid xml file is found perform calculation and produce out put file in xml format
    DataProcessing(report, fileDrop);
}

static GenerationReport ParsingGenerationReport(string path)
{
    var generationReport = Helper.ParseXMLFile<GenerationReport>(path);
    return generationReport;
}

static List<Generator> GetWindGenerators(List<WindGenerator> windGenerators, ValueFactor valueFactor)
{
    var generators = new List<Generator>();
    foreach (var windGenerator in windGenerators)
    {
        var days = windGenerator.Generation.Days;
        var windGeneratorName = windGenerator.Name;
        double totalWindGenerationValue = 0.0;
        var actualValueFactor = windGeneratorName == "Wind[Offshore]" ? valueFactor.Low : valueFactor.High;
        foreach (var day in days)
        {
            //Daily Generation Value 
            totalWindGenerationValue += day.Energy * day.Price * actualValueFactor;
        }
        generators.Add(new Generator { Name = windGeneratorName, Total = totalWindGenerationValue });
    }
    return generators;
}

static Generator GetGasGenerator(GasGenerator gasGenerator, ValueFactor valueFactor,
    EmissionsFactor emissionFactor, ref List<Day> outputDays)
{
    var emissionRating = gasGenerator.EmissionsRating;
    double totalGasGenerationValue = 0.0;
    foreach (var day in gasGenerator.Generation.Days)
    {
        totalGasGenerationValue += day.Energy * day.Price * valueFactor.Medium;
        var emission = day.Energy * emissionRating * emissionFactor.Medium;
        outputDays.Add(new Day
        {
            Date = day.Date,
            Emission = emission,
            Name = gasGenerator.Name
        });
    }
    return new Generator { Name = gasGenerator.Name, Total = totalGasGenerationValue };
}

static Generator GetCoalGenerator(CoalGenerator coalGenerator, ValueFactor valueFactor,
    EmissionsFactor emissionFactor, ref List<Day> outputDays)
{
    var coalEmissionRating = coalGenerator.EmissionsRating;
    double totalCoalGenerationValue = 0.0;
    foreach (var day in coalGenerator.Generation.Days)
    {
        totalCoalGenerationValue += day.Energy * day.Price * valueFactor.Medium;
        var coalEmission = day.Energy * coalEmissionRating * emissionFactor.High;
        outputDays.Add(new Day
        {
            Date = day.Date,
            Emission = coalEmission,
            Name = coalGenerator.Name
        });
    }
    return new Generator { Name = coalGenerator.Name, Total = totalCoalGenerationValue };
}

static ReferenceData ParseReferenceDataFile()
{
    var path = AppsettingReader.ReadSection<ConfigLocations>("ConfigLocations");
    string filename = $"{path.ReferenceData}\\ReferenceData.xml";
    var referenceData = Helper.ParseXMLFile<ReferenceData>(filename);
    return referenceData;
}

static void DataProcessing(GenerationReport report, string inputFileName)
{
    //Create output xml file
    List<Generator> generators = new List<Generator>();
    var referenceData = ParseReferenceDataFile();
    var valueFactor = referenceData.Factors.ValueFactor;
    var emissionFactor = referenceData.Factors.EmissionsFactor;

    //Wind
    var windGenerators = GetWindGenerators(report.Wind.WindGenerators, valueFactor);
    generators.AddRange(windGenerators);

    //Gas
    var outPutDays = new List<Day>();
    var gasGenerator = GetGasGenerator(report.Gas.GasGenerator, valueFactor, emissionFactor, ref outPutDays);
    generators.Add(gasGenerator);

    //Coal
    var coalGenerator = GetCoalGenerator(report.Coal.CoalGenerator, valueFactor, emissionFactor, ref outPutDays);
    generators.Add(coalGenerator);

    //Calculate Actual Heat Rate = TotalHeatInput / ActualNetGeneration
    var actualHeatRateValue = report.Coal.CoalGenerator.TotalHeatInput / report.Coal.CoalGenerator.ActualNetGeneration;

    var actualHeatRate = new ActualHeatRate { Name = coalGenerator.Name, HeatRate = actualHeatRateValue };

    var totals = new Totals { Generator = generators };

    var maxEmissionGenerators = new MaxEmissionGenerators { Day = outPutDays };

    var actualHeatRates = new ActualHeatRates { ActualHeatRate = actualHeatRate };

    var generationOutput = new GenerationOutput
    {
        Totals = totals,
        MaxEmissionGenerators = maxEmissionGenerators,
        ActualHeatRates = actualHeatRates
    };

    CreateOutputXmlFile(inputFileName, generationOutput);
}

static void CreateOutputXmlFile(string inputFileName, GenerationOutput output)
{
    //Create an output file based on input
    var path = AppsettingReader.ReadSection<ConfigLocations>("ConfigLocations");
    XmlSerializer writer = new XmlSerializer(typeof(GenerationOutput));
    string filename = $"{path.Output}\\{Path.GetFileNameWithoutExtension(inputFileName)}-Result.xml";
    FileStream file = File.Create(filename);
    writer.Serialize(file, output);
    file.Close();
}