using System.Xml.Serialization;

namespace Brady_GenerationReport.Input;

public class Wind
{
    [XmlElement("WindGenerator")]
    public List<WindGenerator> WindGenerators { get; set; }
}