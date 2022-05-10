using System.Xml.Serialization;

namespace Brady_GenerationReport.Output;

[XmlRoot("Day")]
public class Day
{
    [XmlElement("Name")]
    public string Name { get; set; }
    [XmlElement("Date")]
    public string Date { get; set; }
    [XmlElement("Emission")]
    public double Emission { get; set; }
}
