using System.Xml.Serialization;

namespace Brady_GenerationReport.OutPut;

public class Totals
{
    [XmlElement("Generator")]
    public List<Generator> Generator { get; set; }
}