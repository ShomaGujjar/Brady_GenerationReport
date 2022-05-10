// See https://aka.ms/new-console-template for more information

using System.Xml;
using System.Xml.Serialization;
using Brady_GenerationReport.Input;
using Brady_GenerationReport.OutPut;
using Day = Brady_GenerationReport.Output.Day;

Console.WriteLine("Hello, Brady!");
Main();

static void Main()
{
    CalculateTotalGenerationValue();
}

static void CalculateTotalGenerationValue()
{
    //Console.WriteLine("Calculating Total Generation Value!");
    //Read input location from config
    string path = @"C:\Projects\Interviews\Input";
    MonitorDirectory(path);
    Console.ReadKey();
    //ping input location for any valid xml file
    //once valid xml file is found perform calculation and produce out put file in xml format
}

static void MonitorDirectory(string path)
{
    FileSystemWatcher fileSystemWatcher = new FileSystemWatcher
    {
        Path = path,
        Filter = "*.xml",
        EnableRaisingEvents = true
    };
    fileSystemWatcher.Created += FileSystemWatcherCreated;
}

static void FileSystemWatcherCreated(object sender, FileSystemEventArgs e)
{
    Console.WriteLine("File created: {0}", e.Name);
    var report = PerformParsingOfTheXmlFile(@"C:\Projects\Interviews\Input");
    //once valid xml file is found perform calculation and produce out put file in xml format
    DataProcessing(report);
    Console.WriteLine("XML file parsed");
}

static GenerationReport PerformParsingOfTheXmlFile(string path)
{
    string filename = $"{path}\\01-Basic.xml";
    XmlReader reader = XmlReader.Create(filename);
    XmlSerializer xmlSerializer = new XmlSerializer(typeof(GenerationReport));
    var generationReport = (GenerationReport)xmlSerializer.Deserialize(reader);
    return generationReport;
}

static ReferenceData ReadReferenceDataFile(string path)
{
    string filename = $"{path}\\ReferenceData.xml";
    XmlReader reader = XmlReader.Create(filename);
    XmlSerializer xmlSerializer = new XmlSerializer(typeof(ReferenceData));
    ReferenceData referenceData = (ReferenceData)xmlSerializer.Deserialize(reader);
    return referenceData;
}

static void DataProcessing(GenerationReport report)
{
    //Create output xml file
    List<Generator> generators = new List<Generator>();
    string path = @"C:\Projects\Interviews\Brady_GenerationReport\ReferenceData\";
    var referenceData = ReadReferenceDataFile(path);

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

    //Gas
    var gasGenerator = report.Gas.GasGenerator;
    var emissionRating = gasGenerator.EmissionsRating;
    double totalGasGenerationValue = 0.0;
    double totalDailyEmission = 0.0;
    foreach (var day in gasGenerator.Generation.Days)
    {
        totalGasGenerationValue += day.Energy * day.Price * valueFactor.Medium;
        totalDailyEmission += day.Energy * emissionRating * emissionFactor.Medium;
    }
    generators.Add(new Generator { Name = gasGenerator.Name, Total = totalGasGenerationValue });

    //Coal
    var coalGenerator = report.Coal.CoalGenerator;
    var coalEmissionRating = coalGenerator.EmissionsRating;
    double totalCoalGenerationValue = 0.0;
    double totalCoalDailyEmission = 0.0;
    foreach (var day in coalGenerator.Generation.Days)
    {
        totalCoalGenerationValue += day.Energy * day.Price * valueFactor.Medium;
        totalCoalDailyEmission += day.Energy * coalEmissionRating * emissionFactor.Medium;
    }
    generators.Add(new Generator { Name = coalGenerator.Name, Total = totalCoalGenerationValue });

    Totals totals = new Totals()
    {
        Generator = generators
    };

    MaxEmissionGenerators maxEmissionGenerators = new MaxEmissionGenerators()
    {
        Day = new List<Day>()
    };

    ActualHeatRates actualHeatRates = new ActualHeatRates()
    {
        ActualHeatRate = new ActualHeatRate()
    };

    GenerationOutput output = new GenerationOutput()
    {
        Totals = totals,
        MaxEmissionGenerators = maxEmissionGenerators,
        ActualHeatRates = actualHeatRates
    };
    //Calculate highest Daily Emissions = Energy x EmissionRating x EmissionFactor for for Coal and Gas
    //Calculate Actual Heat Rate = TotalHeatInput / ActualNetGeneration
}