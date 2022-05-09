using System.Xml.Serialization;

namespace Brady_GenerationReport.OutPut;
[XmlRoot("GenerationOutput")]
public class GenerationOutput
{
    [XmlElement("Totals")]
    public Totals Totals { get; set; }
    [XmlElement("MaxEmissionGenerators")]
    public MaxEmissionGenerators MaxEmissionGenerators { get; set; }
    [XmlElement("ActualHeatRates")]
    public ActualHeatRates ActualHeatRates { get; set; }
    [XmlAttribute(AttributeName = "xsi", Namespace = "http://www.w3.org/2000/xmlns/")]
    public string Xsi { get; set; }
    [XmlAttribute(AttributeName = "xsd", Namespace = "http://www.w3.org/2000/xmlns/")]
    public string Xsd { get; set; }
}