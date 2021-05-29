using System.Collections.Generic;
using System.Xml.Serialization;

namespace EntitiesProcessingLib.Entities 
{
    public class User
    {
        [XmlElement("id")]
        public long ID { get; set; }

        public string Login { get; set; }

        public string Password { get; set; }

        [XmlElement("fullname")]
        public string Fullname { get; set; }

        public List<Course> Courses { get; set; }
        public List<Course> OwnCourses { get; set; }

        public override string ToString()
        {
            return $"[{ID}] - {Fullname}";
        }
    }

    public class Lecture
    {
        [XmlElement("id")]
        public long ID { get; set; }

        [XmlElement("theme")]
        public string Theme { get; set; }
        
        [XmlElement("imported")]
        public bool IsImported;

        public Course Course { get; set; }

        public override string ToString()
        {
            return $"[{ID}] - {Theme}";
        }
    }

    public class Course
    {
        [XmlElement("id")]
        public long ID { get; set; }

        [XmlElement("title")]
        public string Title { get; set; }       
        
        [XmlElement("imported")]
        public bool IsImported { get; set; }

        [XmlElement("author")]
        public User Author { get; set; }

        [XmlElement("lecture")]
        public List<Lecture> Lectures { get; set; }

        public List<User> Subscribers { get; set; }

        public override string ToString()
        {
            return $"[{ID}] - {Title}";
        }
    }
}