using System.Xml.Serialization;

namespace Brady_GenerationReport.Input;
public class Day
{
    [XmlElement("Date")]
    public string Date { get; set; }
    [XmlElement("Energy")]
    public double Energy { get; set; }
    [XmlElement("Price")]
    public double Price { get; set; }
}