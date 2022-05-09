using System.Xml.Serialization;

namespace Brady_GenerationReport.Input;

public class Generation
{
    [XmlElement("Day")]
    public List<Day> Days { get; set; }
}