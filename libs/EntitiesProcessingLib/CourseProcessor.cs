using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using EntitiesProcessingLib.Entities;
using EntitiesProcessingLib.Repositories;
using ScottPlot;

namespace EntitiesProcessingLib.DataProcessing
{
    public class CourseProcessor
    {
        private CourseRepository _courseRep;

        private LectureRepository _lectureRep;

        public CourseProcessor(CourseRepository courseRepository, LectureRepository lectureRepository)
        {
            _courseRep = courseRepository;
            _lectureRep = lectureRepository;
        }

        public void Export(string courseWord, string exportFilePath)
        {
            List<Course> courses = _courseRep.GetCourseExport(courseWord);
            foreach (var course in courses)
            {
                course.Subscribers = null;
            }

            var serializer = new XmlSerializer(typeof(Root));
            var writer = new StreamWriter(exportFilePath);  

            serializer.Serialize(writer, new Root() {courses = courses});
            writer.Close();
        }
   
        public void Import(string dataPath)
        {
            XmlSerializer ser = new XmlSerializer(typeof(Root));
            StreamReader sr = new StreamReader(dataPath);

            var root = (Root) ser.Deserialize(sr);
            foreach (var course in root.courses)
            {
                if (_courseRep.GetCourse(course.ID) != null)
                {
                    continue;
                }

                course.IsImported = true;
                _courseRep.Insert(course.ID, course);

                foreach (var lecture in course.Lectures)
                {
                    lecture.Course = course;

                    if (_lectureRep.GetLecture(lecture.ID) != null)
                    {
                        _lectureRep.Update(lecture.ID, lecture);
                        continue;
                    }

                    lecture.IsImported = true;
                    _lectureRep.Insert(lecture.ID, lecture);
                }
            }
            sr.Close();
        }
    
        public void DrawPlot(int count, string imagePath)
        {
            var plt = new Plot(600, 400);
            var data = _courseRep.GetImageData(count);
            int len = data.Count;

            string[] labels = new string[len];
            double[] ys = new double[len];
            for (int i = 0; i < len; ++i)
            {
                ys[i] = data[i].Subscribers.Count;
                labels[i] = data[i].ID.ToString();
            }
            double[] xs = new double[count];
            for (int i = 0; i < count; ++i)
            {
                xs[i] = i + 1;
            }
        
            plt.PlotBar(xs, ys, horizontal: true);
            plt.Grid(enableHorizontal: false, lineStyle: LineStyle.Dot);
            plt.YTicks(xs, labels);

            plt.SaveFig(imagePath);
        }
    }
    
    [XmlRoot("courses")]
    public class Root
    {
        [XmlElement("course")]
        public List<Course> courses;
    }
}