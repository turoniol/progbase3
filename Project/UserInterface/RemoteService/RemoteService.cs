using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using EntitiesProcessingLib.Entities;

namespace ServiceLib
{
    public class RemoteService : IService
    {
        private Socket _sender;

        public RemoteService()
        {

            IPAddress ipAddress = IPAddress.Loopback;
            int port = 3000;

            _sender = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);

            try
            {
                _sender.Connect(remoteEP);
            }
            catch (Exception e)
            {
                throw new Exception($"Can't connect to the server: {e.Message}");
            }
        }

        public bool DeleteCourse(long id)
        {
            string command = "delete";
            command = ServerProcessor.AddAgrument(command, "course");
            command = ServerProcessor.AddAgrument(command, id);

            return ServerProcessor.SendRequest<bool>(_sender, command);
        }

        public bool DeleteLecture(long id)
        {
            string command = "delete";
            command = ServerProcessor.AddAgrument(command, "lecture");
            command = ServerProcessor.AddAgrument(command, id);
            return ServerProcessor.SendRequest<bool>(_sender, command);
        }

        public bool DeleteLectures(long courseID)
        {
            string command = "delete";
            command = ServerProcessor.AddAgrument(command, "lectures");
            command = ServerProcessor.AddAgrument(command, courseID);
            return ServerProcessor.SendRequest<bool>(_sender, command);
        }

        public bool DeleteSubscription(long id)
        {
            string command = "delete";
            command = ServerProcessor.AddAgrument(command, "subscription");
            command = ServerProcessor.AddAgrument(command, id);
            return ServerProcessor.SendRequest<bool>(_sender, command);
        }

        public bool DeleteUser(long id)
        {
            string command = "delete";
            command = ServerProcessor.AddAgrument(command, "user");
            command = ServerProcessor.AddAgrument(command, id);
            return ServerProcessor.SendRequest<bool>(_sender, command);
        }

        public void DrawPlot(int count, string imagePath)
        {
            string command = "draw";
            command = ServerProcessor.AddAgrument(command, command);
            command = ServerProcessor.AddAgrument(command, imagePath);
            ServerProcessor.SendRequest<string>(_sender, command);
        }

        public void Export(string courseWord, string exportFilePath)
        {
            string command = "export";
            command = ServerProcessor.AddAgrument(command, courseWord);
            command = ServerProcessor.AddAgrument(command, exportFilePath);
            ServerProcessor.SendRequest<string>(_sender, command);
        }

        public Course GetCourse(long id)
        {
            string command = "get";
            command = ServerProcessor.AddAgrument(command, "course");
            command = ServerProcessor.AddAgrument(command, id);
            return ServerProcessor.SendRequest<Course>(_sender, command);
        }

        public Lecture GetLecture(long id)
        {
            string command = "get";
            command = ServerProcessor.AddAgrument(command, "lecture");
            command = ServerProcessor.AddAgrument(command, id);
            return ServerProcessor.SendRequest<Lecture>(_sender, command);
        }

        public List<Course> GetPageCourse(int pageNumber, int pageSize, string name, long authorID)
        {
            string command = "page";
            command = ServerProcessor.AddAgrument(command, "course");
            command = ServerProcessor.AddAgrument(command, pageNumber);
            command = ServerProcessor.AddAgrument(command, pageSize);
            command = ServerProcessor.AddAgrument(command, name);
            command = ServerProcessor.AddAgrument(command, authorID);

            return ServerProcessor.SendRequest<List<Course>>(_sender, command);
        }

        public List<Lecture> GetPageLecture(int pageNumber, int pageSize, string theme, long subscriberID)
        {
            string command = "page";
            command = ServerProcessor.AddAgrument(command, "lecture");
            command = ServerProcessor.AddAgrument(command, pageNumber);
            command = ServerProcessor.AddAgrument(command, pageSize);
            command = ServerProcessor.AddAgrument(command, theme);
            command = ServerProcessor.AddAgrument(command, subscriberID);

            return ServerProcessor.SendRequest<List<Lecture>>(_sender, command);
        }

        public List<User> GetPageUser(int pageNumber, int pageSize, string login)
        {
            string command = "page";
            command = ServerProcessor.AddAgrument(command, "user");
            command = ServerProcessor.AddAgrument(command, pageNumber);
            command = ServerProcessor.AddAgrument(command, pageSize);
            command = ServerProcessor.AddAgrument(command, login);
            command = ServerProcessor.AddAgrument(command, (long)-1);

            return ServerProcessor.SendRequest<List<User>>(_sender, command);
        }

        public Subscription GetSubscription(Subscription sb)
        {
            string command = "get";
            command = ServerProcessor.AddAgrument(command, "subscription");
            command = ServerProcessor.AddAgrument(command, sb);
            return ServerProcessor.SendRequest<Subscription>(_sender, command);
        }

        public int GetTotalPagesCountCourse(int pageSize, long authorID, string name)
        {
            string command = "pages";
            command = ServerProcessor.AddAgrument(command, "course");
            command = ServerProcessor.AddAgrument(command, pageSize);
            command = ServerProcessor.AddAgrument(command, name);
            command = ServerProcessor.AddAgrument(command, authorID);
            return ServerProcessor.SendRequest<int>(_sender, command);
        }

        public int GetTotalPagesCountLecture(int pageSize, string theme, long subscriberID)
        {
            string command = "pages";
            command = ServerProcessor.AddAgrument(command, "lecture");
            command = ServerProcessor.AddAgrument(command, pageSize);
            command = ServerProcessor.AddAgrument(command, theme);
            command = ServerProcessor.AddAgrument(command, subscriberID);
            return ServerProcessor.SendRequest<int>(_sender, command);
        }

        public int GetTotalPagesCountUser(int pageSize, string login)
        {
            string command = "pages";
            command = ServerProcessor.AddAgrument(command, "user");
            command = ServerProcessor.AddAgrument(command, pageSize);
            command = ServerProcessor.AddAgrument(command, login);
            command = ServerProcessor.AddAgrument(command, (long)-1);
            return ServerProcessor.SendRequest<int>(_sender, command);
        }

        public User GetUser(long id)
        {
            string command = "get";
            command = ServerProcessor.AddAgrument(command, "user");
            command = ServerProcessor.AddAgrument(command, id);
            return ServerProcessor.SendRequest<User>(_sender, command);
        }

        public void Import(string dataPath)
        {
            string command = "import";
            command = ServerProcessor.AddAgrument(command, dataPath);
            ServerProcessor.SendRequest<string>(_sender, command);
        }

        public long Insert(User user)
        {
            string command = "insert";
            command = ServerProcessor.AddAgrument(command, "user");
            command = ServerProcessor.AddAgrument(command, user);
            return ServerProcessor.SendRequest<long>(_sender, command);
        }

        public long Insert(Course course)
        {
            string command = "insert";
            command = ServerProcessor.AddAgrument(command, "course");
            command = ServerProcessor.AddAgrument(command, course);
            return ServerProcessor.SendRequest<long>(_sender, command);
        }

        public long Insert(Lecture lecture)
        {
            string command = "insert";
            command = ServerProcessor.AddAgrument(command, "lecture");
            command = ServerProcessor.AddAgrument(command, lecture);
            return ServerProcessor.SendRequest<long>(_sender, command);
        }

        public long Insert(Subscription sb)
        {
            string command = "insert";
            command = ServerProcessor.AddAgrument(command, "subscription");
            command = ServerProcessor.AddAgrument(command, sb);
            return ServerProcessor.SendRequest<long>(_sender, command);
        }

        public User Login(User user)
        {
            string command = "login";
            command = ServerProcessor.AddAgrument(command, user);
            return ServerProcessor.SendRequest<User>(_sender, command);
        }

        public User Register(User user)
        {
            string command = "register";
            command = ServerProcessor.AddAgrument(command, user);
            return ServerProcessor.SendRequest<User>(_sender, command);
        }

        public bool Update(long ID, User user)
        {
            string command = "update";
            command = ServerProcessor.AddAgrument(command, "user");
            command = ServerProcessor.AddAgrument(command, ID);
            command = ServerProcessor.AddAgrument(command, user);
            return ServerProcessor.SendRequest<bool>(_sender, command);
        }

        public bool Update(long ID, Course course)
        {
            string command = "update";
            command = ServerProcessor.AddAgrument(command, "course");
            command = ServerProcessor.AddAgrument(command, ID);
            command = ServerProcessor.AddAgrument(command, course);
            return ServerProcessor.SendRequest<bool>(_sender, command);
        }

        public bool Update(long ID, Lecture lecture)
        {
            string command = "update";
            command = ServerProcessor.AddAgrument(command, "lecture");
            command = ServerProcessor.AddAgrument(command, ID);
            command = ServerProcessor.AddAgrument(command, lecture);
            return ServerProcessor.SendRequest<bool>(_sender, command);
        }

        public User UpdateUserAuth(User user)
        {
            string command = "updateAuth";
            command = ServerProcessor.AddAgrument(command, user);
            return ServerProcessor.SendRequest<User>(_sender, command);
        }
    
        public void Close() => _sender.Close();
    }
}