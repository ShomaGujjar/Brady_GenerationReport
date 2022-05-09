// See https://aka.ms/new-console-template for more information

using System.Xml;
using System.Xml.Serialization;
using Brady_GenerationReport.Input;

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
    string path = @"C:\Projects\Interviews\Brady_GenerationReport\ReferenceData\";
    var referenceData = ReadReferenceDataFile(path);
    //Calculate Total Generation Value = Energy x Price x ValueFactor
    //Wind
    var valueFactor = referenceData.Factors.ValueFactor;
    var emissionFactor = referenceData.Factors.EmissionsFactor;
    var windGenerators = report.Wind.WindGenerators;
    foreach (var windGenerator in windGenerators)
    {
        var days = windGenerator.Generation.Days;
        var location = windGenerator.Location;
        var name = windGenerator.Name;
        foreach (var day in days)
        {
            //double valueFactor;
            var value = location == "Offshore" ? valueFactor.Low : valueFactor.High;
            var dailyGenerationValue = day.Energy * day.Price * value;
        }

    }

    //Gas

    var gasGenerator = report.Gas.GasGenerator;
    //Coal
    var coalGenerator = report.Coal.CoalGenerator;
    //Calculate highest Daily Emissions = Energy x EmissionRating x EmissionFactor for for Coal and Gas
    //Calculate Actual Heat Rate = TotalHeatInput / ActualNetGeneration
}