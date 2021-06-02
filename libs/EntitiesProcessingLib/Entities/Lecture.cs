using System.Collections.Generic;
using System.Xml.Serialization;

namespace EntitiesProcessingLib.Entities
{
    [XmlType(TypeName = "lecture")]
    public class Lecture
    {
        [XmlElement("id")]
        public long ID { get; set; }

        [XmlElement("theme")]
        public string Theme { get; set; }

        [XmlElement("imported")]
        public bool IsImported;

        [XmlElement("course")]
        public Course Course { get; set; }

        public List<User> Subscribers {get; set;}

        public override string ToString()
        {
            return $"[{ID}] - {Theme}";
        }
    }
}