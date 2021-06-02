using System.Collections.Generic;
using System.Xml.Serialization;

namespace EntitiesProcessingLib.Entities
{
    [XmlType(TypeName = "course")]
    public class Course
    {
        [XmlElement("id")]
        public long ID { get; set; }

        [XmlElement("title")]
        public string Title { get; set; }

        [XmlElement("imported")]
        public bool IsImported { get; set; }
        [XmlElement("access")]
        public bool CanSubcribe;

        [XmlElement("author")]
        public User Author { get; set; }

        [XmlElement("lectures")]
        public List<Lecture> Lectures { get; set; }

        public List<User> Subscribers { get; set; }

        public override string ToString()
        {
            return $"[{ID}] {Title}";
        }
    }
}