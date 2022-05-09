using System.Xml.Serialization;

namespace Brady_GenerationReport.Input;

public class Coal
{
    [XmlElement("CoalGenerator")]
    public CoalGenerator CoalGenerator { get; set; }
}