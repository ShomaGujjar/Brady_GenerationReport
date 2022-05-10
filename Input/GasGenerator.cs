using System.Xml.Serialization;

namespace Brady_GenerationReport.Input;

public class GasGenerator
{
    [XmlElement("Name")]
    public string Name { get; set; }

    [XmlElement("Generation")]
    public Generation Generation { get; set; }

    [XmlElement("EmissionsRating")]
    public double EmissionsRating { get; set; }
}