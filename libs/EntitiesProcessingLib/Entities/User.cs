using System.Collections.Generic;
using System.Xml.Serialization;

namespace EntitiesProcessingLib.Entities
{
    [XmlType(TypeName = "user")]
    public class User
    {
        [XmlElement("id")]
        public long ID { get; set; }

        [XmlElement("login")]
        public string Login { get; set; }

        [XmlElement("password")]
        public string Password { get; set; }

        [XmlElement("fullname")]
        public string Fullname { get; set; }

        public List<Course> Courses { get; set; }
        public List<Course> OwnCourses { get; set; }

        public override string ToString()
        {
            return $"[{ID}] {Login} - {Fullname}";
        }
    }
}