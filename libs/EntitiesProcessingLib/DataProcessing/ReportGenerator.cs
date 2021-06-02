using System.IO;
using System.IO.Compression;
using System.Xml;
using System.Xml.Linq;
using System.Collections.Generic;
using EntitiesProcessingLib.Repositories;
using EntitiesProcessingLib.Entities;

namespace EntitiesProcessingLib.DataProcessing
{
    public class ReportGenerator
    {
        Dictionary<string, string> _dict;
        private string _xmlPathTemplate = "./../../data/template/word/document.xml";
        private string _xmlPathResult = "./../../data/report/word/document.xml";
        private string _imagePath = "./../../data/report/word/media/image1.png";
        public void ReadFile(CourseRepository _courseRep, SubscriptionRepository _subRep,
             LectureRepository _lectureRep, UserRepository _useRep)
        {
            _dict = new Dictionary<string, string>();
            XElement root = XElement.Load(_xmlPathTemplate);

            var ids = _courseRep.GetAllIDs();
            if (ids.Count == 0)
            {
                throw new System.ArgumentException("No courses!");
            }

            int coursesCount = ids.Count;
            int subCount = 0;
            int lectureCount = 0;

            List<Course> courses = new List<Course>();
            foreach (var id in ids)
            {
                var course = _courseRep.GetCourse(id);
                course.Author = _useRep.GetUser(course.Author.ID);
                course.Subscribers = _subRep.GetListenersByCourse(id);
                course.Lectures = _lectureRep.GetLecturesByCourse(id);

                subCount += course.Subscribers.Count;
                lectureCount += course.Lectures.Count;
                courses.Add(course);
            }

            float subAverage = (float) subCount / coursesCount;
            float lectureAverage = (float) lectureCount / coursesCount;

            var maxSubs = CourseProcessor.SortByListeners(courses)[0];
            var maxLectures = CourseProcessor.SortByLectures(courses)[0];

            _dict.Add("{{course}}", coursesCount.ToString());
            _dict.Add("{{lecture}}", lectureAverage.ToString());
            _dict.Add("{{subs}}", subAverage.ToString());
            _dict.Add("{{authorOne}}", maxSubs.Author.Login);
            _dict.Add("{{titleOne}}", maxSubs.Title);
            _dict.Add("{{subOne}}", maxSubs.Subscribers.Count.ToString());
            _dict.Add("{{lectureOne}}", maxSubs.Lectures.Count.ToString());
            _dict.Add("{{authorTwo}}", maxLectures.Author.Login);
            _dict.Add("{{titleTwo}}", maxLectures.Title);
            _dict.Add("{{subTwo}}", maxLectures.Subscribers.Count.ToString());
            _dict.Add("{{lectureTwo}}", maxLectures.Lectures.Count.ToString());

            CourseProcessor proc = new CourseProcessor(_courseRep, _lectureRep);
            proc.DrawPlot(10, _imagePath);

            FindAndReplace(root);

            root.Save(_xmlPathResult);

            string resPath = "./../../data/report.docx";
            if (File.Exists(resPath))
            {
                File.Delete("./../../data/report.docx");
            }

            ZipFile.CreateFromDirectory("./../../data/report", resPath);
        }

        private void FindAndReplace(XElement node)
        {
            if (node.FirstNode != null
                && node.FirstNode.NodeType == XmlNodeType.Text)
            {
                string replasment;
                if (_dict.TryGetValue(node.Value, out replasment))
                {
                    node.Value = replasment;
                }
            }

            foreach (XElement el in node.Elements())
            {
                FindAndReplace(el);
            }
        }

    }
}