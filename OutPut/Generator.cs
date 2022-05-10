using System.Xml.Serialization;

namespace Brady_GenerationReport.OutPut;

[XmlRoot("Generator")]
public class Generator
{
    [XmlElement("Name")]
    public string Name { get; set; }
    [XmlElement("Total")]
    public double Total { get; set; }
}