// See https://aka.ms/new-console-template for more information

using System.Xml;
using System.Xml.Serialization;

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

	Console.WriteLine("XML file parsed");
}

static GenerationReport PerformParsingOfTheXmlFile(string path)
{
	string filename = $"{path}\\01-Basic.xml";
	XmlReader reader = XmlReader.Create(filename);
	XmlSerializer xmlSerializer = new XmlSerializer(typeof(GenerationReport));
	GenerationReport generationReport = (GenerationReport)xmlSerializer.Deserialize(reader);
	return generationReport;
}

[Serializable, XmlRoot("GenerationReport")]
public class GenerationReport
{
	[XmlElement("Wind")]
	public Wind Wind { get; set; }

	[XmlElement("Gas")]
	public Gas Gas { get; set; }

	[XmlElement("Coal")]
	public Coal Coal { get; set; }
}

public class Wind
{
	[XmlElement("WindGenerator")]
	public List<WindGenerator> WindGenerators { get; set; }
}

public class WindGenerator
{
	[XmlElement("Name")]
	public string Name { get; set; }
	[XmlElement("Generation")]
	public Generation Generation { get; set; }
	[XmlElement("Location")]
	public string Location { get; set; }
}

public class Generation
{
	[XmlElement("Day")]
	public List<Day> Days { get; set; }
}

public class Day
{
	[XmlElement("Date")]
	public string Date { get; set; }
	[XmlElement("Energy")]
	public string Energy { get; set; }
	[XmlElement("Price")]
	public string Price { get; set; }
}

public class Coal
{
	[XmlElement("CoalGenerator")]
	public CoalGenerator CoalGenerator { get; set; }
}

public class CoalGenerator
{
	[XmlElement("Name")]
	public string Name { get; set; }
	[XmlElement("Generation")]
	public Generation Generation { get; set; }
	[XmlElement("TotalHeatInput")]
	public string TotalHeatInput { get; set; }
	[XmlElement("ActualNetGeneration")]
	public string ActualNetGeneration { get; set; }
	[XmlElement("EmissionsRating")]
	public string EmissionsRating { get; set; }
}
public class Gas
{
	[XmlElement("GasGenerator")]
	public GasGenerator GasGenerator { get; set; }
}

public class GasGenerator
{
	[XmlElement("Name")]
	public string Name { get; set; }
	[XmlElement("Generation")]
	public Generation Generation { get; set; }
	[XmlElement("EmissionsRating")]
	public string EmissionsRating { get; set; }
}






