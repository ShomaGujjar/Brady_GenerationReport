using System.Xml.Serialization;

namespace Brady_GenerationReport.Input;

public class CoalGenerator
{
    [XmlElement("Name")]
    public string Name { get; set; }
    [XmlElement("Generation")]
    public Generation Generation { get; set; }
    [XmlElement("TotalHeatInput")]
    public string TotalHeatInput { get; set; }
    [XmlElement("ActualNetGeneration")]
    public string ActualNetGeneration { get; set; }
    [XmlElement("EmissionsRating")]
    public double EmissionsRating { get; set; }
}