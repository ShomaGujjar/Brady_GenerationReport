using System.Xml.Serialization;

namespace Brady_GenerationReport.Input;

public class Gas
{
    [XmlElement("GasGenerator")]
    public GasGenerator GasGenerator { get; set; }
}