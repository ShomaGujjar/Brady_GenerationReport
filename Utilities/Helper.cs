using System.Xml;
using System.Xml.Serialization;

namespace Brady_GenerationReport.Utilities
{
    public static class Helper
    {
        public static T ParseXMLFile<T>(string path)
        {
            var reader = XmlReader.Create(path);
            var xmlSerializer = new XmlSerializer(typeof(T));
            T data = (T)xmlSerializer.Deserialize(reader);
            return data;
        }
    }
}
