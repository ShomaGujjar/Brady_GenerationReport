using System.Xml.Serialization;

namespace Brady_GenerationReport.Input;

public class EmissionsFactor
{
    [XmlElement(ElementName = "High")]
    public double High { get; set; }
    [XmlElement(ElementName = "Medium")]
    public double Medium { get; set; }
    [XmlElement(ElementName = "Low")]
    public double Low { get; set; }
}