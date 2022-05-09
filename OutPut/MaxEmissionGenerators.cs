using System.Xml.Serialization;
using Brady_GenerationReport.Output;

namespace Brady_GenerationReport.OutPut;

[XmlRoot("MaxEmissionGenerators")]
public class MaxEmissionGenerators
{
    [XmlElement("Day")]
    public List<Day> Day { get; set; }
}