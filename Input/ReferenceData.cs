using System.Xml.Serialization;

namespace Brady_GenerationReport.Input;

[Serializable, XmlRoot(ElementName = "ReferenceData")]
public class ReferenceData
{
    [XmlElement(ElementName = "Factors")]
    public Factors Factors { get; set; }
}