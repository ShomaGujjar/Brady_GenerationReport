using System.Xml.Serialization;

namespace Brady_GenerationReport.OutPut;

[XmlRoot("ActualHeatRate")]
public class ActualHeatRate
{
    [XmlElement("Name")]
    public string Name { get; set; }
    [XmlElement("HeatRate")]
    public string HeatRate { get; set; }
}