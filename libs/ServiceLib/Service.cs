using System.Collections.Generic;
using EntitiesProcessingLib.Repositories;
using EntitiesProcessingLib.Entities;
using EntitiesProcessingLib.DataProcessing;
using EntitiesProcessingLib.Authentication;

namespace ServiceLib
{
    public class Service : IService
    {
        private UserRepository _userRep;
        private CourseRepository _courseRep;
        private LectureRepository _lectureRep;
        private SubscriptionRepository _subRep;

        public Service(UserRepository userRep, CourseRepository courseRep,
            LectureRepository lectureRep, SubscriptionRepository subRep)
        {
            _userRep = userRep;
            _courseRep = courseRep;
            _lectureRep = lectureRep;
            _subRep = subRep;
        }

        public bool DeleteCourse(long id) => _courseRep.Delete(id);

        public bool DeleteLecture(long id) => _lectureRep.Delete(id);

        public bool DeleteLectures(long courseID) => _lectureRep.DeleteLectures(courseID);

        public bool DeleteSubscription(long id) => _subRep.Delete(id);

        public bool DeleteUser(long id) => _userRep.Delete(id);

        public void DrawPlot(int count, string imagePath)
        {
            CourseProcessor proc = new CourseProcessor(_courseRep, _lectureRep);
            proc.DrawPlot(count, imagePath);
        }

        public void Export(string courseWord, string exportFilePath)
        {
            CourseProcessor proc = new CourseProcessor(_courseRep, _lectureRep);
            proc.Export(courseWord, exportFilePath);
        }

        public Course GetCourse(long id) => _courseRep.GetCourse(id);

        public Lecture GetLecture(long id) => _lectureRep.GetLecture(id);

        public List<Course> GetPageCourse(int pageNumber, int pageSize, string name, long authorID) 
            => _courseRep.GetPage(pageNumber, pageSize, authorID, name);

        public List<Lecture> GetPageLecture(int pageNumber, int pageSize, string theme, long subscriberID)
        {
            if (subscriberID == -1)
            {
                return _lectureRep.GetPage(pageNumber, pageSize, theme);
            }

            if (pageNumber < 1 || pageNumber > GetTotalPagesCountLecture(pageSize, theme, subscriberID))
            {
                throw new System.ArgumentException();
            }

            var courses = _subRep.GetCoursesByListener(subscriberID);
            var lectures = GetLecturesFromCourses(courses);

            int offset = (pageNumber - 1) * pageSize;
            int len = lectures.Count;
            pageSize = len - offset > pageSize ? pageSize : len - offset;

            return lectures.GetRange(offset, pageSize);
        }

        public List<User> GetPageUser(int pageNumber, int pageSize, string login)
            =>  _userRep.GetPage(pageNumber, pageSize, login);

        public int GetTotalPagesCountCourse(int pageSize, long authorID, string name)
            => _courseRep.GetTotalPagesCount(pageSize, authorID, name);

        public int GetTotalPagesCountLecture(int pageSize, string theme, long subscriberID)
        {
            if (subscriberID == -1)
            {
                return _lectureRep.GetTotalPagesCount(pageSize, theme);
            }

            if (pageSize < 1 )
            {
                throw new System.ArgumentException();
            }

            var courses = _subRep.GetCoursesByListener(subscriberID);
            List<Lecture> lectures = GetLecturesFromCourses(courses);

            return (int) System.Math.Ceiling(lectures.Count / (float) pageSize);
        }

        private List<Lecture> GetLecturesFromCourses(List<Course> courses)
        {
            List<Lecture> lectures = new List<Lecture>();

            foreach (var course in courses)
            {
                var list = _lectureRep.GetLecturesByCourse(course.ID);
                lectures.AddRange(list);
            }
            
            return lectures;
        }

        public int GetTotalPagesCountUser(int pageSize, string login)
            => _userRep.GetTotalPagesCount(pageSize, login);

        public User GetUser(long id) => _userRep.GetUser(id);

        public Subscription GetSubscription(Subscription sb) => _subRep.GetSubscription(sb.userID, sb.courseID);

        public void Import(string dataPath)
        {
            CourseProcessor proc = new CourseProcessor(_courseRep, _lectureRep);
            proc.Import(dataPath);
        }

        public long Insert(User user) => _userRep.Insert(user);

        public long Insert(Course course) => _courseRep.Insert(course);

        public long Insert(Lecture lecture) => _lectureRep.Insert(lecture);

        public long Insert(Subscription sb) => _subRep.Insert(sb.userID, sb.courseID);

        public User Login(User user)
        {
            Authenticator auth = new Authenticator(_userRep);
            return auth.Login(user);
        }

        public User Register(User user)
        {
            Authenticator auth = new Authenticator(_userRep);
            return auth.Register(user);
        }

        public bool Update(long ID, User user) => _userRep.Update(ID, user);

        public bool Update(long ID, Course course) => _courseRep.Update(ID, course);

        public bool Update(long ID, Lecture lecture) => _lectureRep.Update(ID, lecture);

        public User UpdateUserAuth(User user)
        {
            Authenticator auth = new Authenticator(_userRep);
            return auth.UpdateUser(user);
        }
    }
}
