using System.Xml.Serialization;

namespace Brady_GenerationReport.Input;

public class Factors
{
    [XmlElement(ElementName = "ValueFactor")]
    public ValueFactor ValueFactor { get; set; }
    [XmlElement(ElementName = "EmissionsFactor")]
    public EmissionsFactor EmissionsFactor { get; set; }
}