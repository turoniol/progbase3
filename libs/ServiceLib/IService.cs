using EntitiesProcessingLib.Entities;
using System.Collections.Generic;

namespace ServiceLib
{
    public interface IService
    {
        // authentication
        User Register(User user);
        User UpdateUserAuth(User user);
        User Login(User user);

        // data processing
        void Export(string courseWord, string exportFilePath);
        void Import(string dataPath);
        void DrawPlot(int count, string imagePath);

        // repositories
        long Insert(User user);
        long Insert(Course course);
        long Insert(Lecture lecture);
        long Insert(Subscription sb);
        User GetUser(long id);
        Course GetCourse(long id);
        Lecture GetLecture(long id);
        Subscription GetSubscription(Subscription sb);
        bool Update(long ID, User user);
        bool Update(long ID, Course course);
        bool Update(long ID, Lecture lecture);
        bool DeleteUser(long id);
        bool DeleteCourse(long id);
        bool DeleteLecture(long id);
        bool DeleteLectures(long courseID);
        bool DeleteSubscription(long id);
        List<User> GetPageUser(int pageNumber, int pageSize, string login);
        List<Course> GetPageCourse(int pageNumber, int pageSize, string name, long authorID);
        List<Lecture> GetPageLecture(int pageNumber, int pageSize, string theme, long subscriberID);
        int GetTotalPagesCountUser(int pageSize, string login);
        int GetTotalPagesCountCourse(int pageSize, long authorID, string name);
        int GetTotalPagesCountLecture(int pageSize, string theme, long subscriberID);
        void GenerateReport(string folderPath);
    }
}