// See https://aka.ms/new-console-template for more information

using System.Xml;
using System.Xml.Serialization;
using Brady_GenerationReport;
using Brady_GenerationReport.Input;
using Brady_GenerationReport.OutPut;
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
    Console.WriteLine("File created: {0}", e.Name);
    var report = PerformParsingOfTheXmlFile(e.FullPath);
    //once valid xml file is found perform calculation and produce out put file in xml format
    DataProcessing(report, e.Name);
    Console.WriteLine("XML file parsed");
}

static GenerationReport PerformParsingOfTheXmlFile(string path)
{
    XmlReader reader = XmlReader.Create(path);
    XmlSerializer xmlSerializer = new XmlSerializer(typeof(GenerationReport));
    var generationReport = (GenerationReport)xmlSerializer.Deserialize(reader);
    return generationReport;
}

static void DataProcessing(GenerationReport report, string inputFileName)
{
    //Create output xml file
    List<Generator> generators = new List<Generator>();
    var referenceData = ReadReferenceDataFile();

    //Wind
    var valueFactor = referenceData.Factors.ValueFactor;
    var emissionFactor = referenceData.Factors.EmissionsFactor;
    var windGenerators = report.Wind.WindGenerators;
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

    var outPutDays = new List<Day>();

    //Gas
    var gasGenerator = report.Gas.GasGenerator;
    var emissionRating = gasGenerator.EmissionsRating;
    double totalGasGenerationValue = 0.0;
    foreach (var day in gasGenerator.Generation.Days)
    {
        totalGasGenerationValue += day.Energy * day.Price * valueFactor.Medium;
        var emission = day.Energy * emissionRating * emissionFactor.Medium;
        outPutDays.Add(new Day
        {
            Date = day.Date,
            Emission = emission,
            Name = gasGenerator.Name
        });
    }
    generators.Add(new Generator { Name = gasGenerator.Name, Total = totalGasGenerationValue });

    //Coal
    var coalGenerator = report.Coal.CoalGenerator;
    var coalEmissionRating = coalGenerator.EmissionsRating;
    double totalCoalGenerationValue = 0.0;
    foreach (var day in coalGenerator.Generation.Days)
    {
        totalCoalGenerationValue += day.Energy * day.Price * valueFactor.Medium;
        var coalEmission = day.Energy * coalEmissionRating * emissionFactor.High;
        outPutDays.Add(new Day
        {
            Date = day.Date,
            Emission = coalEmission,
            Name = coalGenerator.Name
        });
    }
    generators.Add(new Generator { Name = coalGenerator.Name, Total = totalCoalGenerationValue });

    //Calculate Actual Heat Rate = TotalHeatInput / ActualNetGeneration
    var actualHeatRateValue = coalGenerator.TotalHeatInput / coalGenerator.ActualNetGeneration;
    ActualHeatRate actualHeatRate = new ActualHeatRate()
    {
        Name = coalGenerator.Name,
        HeatRate = actualHeatRateValue
    };

    Totals totals = new Totals()
    {
        Generator = generators
    };

    MaxEmissionGenerators maxEmissionGenerators = new MaxEmissionGenerators()
    {
        Day = outPutDays
    };

    ActualHeatRates actualHeatRates = new ActualHeatRates()
    {
        ActualHeatRate = actualHeatRate
    };

    GenerationOutput output = new GenerationOutput()
    {
        Totals = totals,
        MaxEmissionGenerators = maxEmissionGenerators,
        ActualHeatRates = actualHeatRates
    };
    //CreateOutPutFile
    CreateOutputXmlFile(inputFileName, output);
}

static ReferenceData ReadReferenceDataFile()
{
    var path = AppsettingReader.ReadSection<ConfigLocations>("ConfigLocations");
    string filename = $"{path.ReferenceData}\\ReferenceData.xml";
    XmlReader reader = XmlReader.Create(filename);
    XmlSerializer xmlSerializer = new XmlSerializer(typeof(ReferenceData));
    ReferenceData referenceData = (ReferenceData)xmlSerializer.Deserialize(reader);
    return referenceData;
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