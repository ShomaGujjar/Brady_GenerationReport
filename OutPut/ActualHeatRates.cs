using System.Xml.Serialization;

namespace Brady_GenerationReport.OutPut;
[XmlRoot("ActualHeatRates")]
public class ActualHeatRates
{
    [XmlElement("ActualHeatRate")]
    public ActualHeatRate ActualHeatRate { get; set; }
}