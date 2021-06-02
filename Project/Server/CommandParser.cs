using System;
using System.IO;
using System.Collections.Generic;
using System.Xml.Serialization;
using ServiceLib;
using EntitiesProcessingLib.Entities;

namespace Server
{
    public class CommandParser
    {
        private IService _service;
        public CommandParser(IService service)
        {
            _service = service;
        }

        public string ParseCommand(string command)
        {
            string argumentSeparator = "<ARG>";
            string[] splited = command.Split(argumentSeparator);

            Dictionary<string, Func<string[], string>> functions = new Dictionary<string, Func<string[], string>>();
            functions.Add("register", OnRegister);
            functions.Add("login", OnLogin);
            functions.Add("updateAuth", OnUpdateUserAuth);
            functions.Add("export", OnExport);
            functions.Add("import", OnImport);
            functions.Add("report", OnReport);
            functions.Add("insert", OnInsert);
            functions.Add("get", OnGet);
            functions.Add("update", OnUpdate);
            functions.Add("delete", OnDelete);
            functions.Add("page", OnGetPage);
            functions.Add("pages", OnGetTotalPagesCount);

            var func = functions[splited[0]];
            return func.Invoke(splited);
        }

        private static T GetEntity<T>(string xml)
        {
            XmlSerializer ser = new XmlSerializer(typeof(T));
            StringReader reader = new StringReader(xml);

            T res = (T)ser.Deserialize(reader);
            reader.Close();

            return res;
        }

        private static string EntityToXml<T>(T entity)
        {
            XmlSerializer ser = new XmlSerializer(typeof(T));
            StringWriter writer = new StringWriter();
            ser.Serialize(writer, entity);

            string result = writer.ToString();
            writer.Close();

            return result;
        }

        private string OnRegister(string[] data)
        {
            if (data.Length != 2)
            {
                throw new ArgumentException();
            }

            string xmlUser = data[1];

            User registered = _service.Register(GetEntity<User>(xmlUser));

            return EntityToXml(registered);
        }

        private string OnLogin(string[] data)
        {
            if (data.Length != 2)
            {
                throw new ArgumentException();
            }

            string xmlUser = data[1];

            User logined = _service.Login(GetEntity<User>(xmlUser));

            return EntityToXml(logined);
        }

        private string OnUpdateUserAuth(string[] data)
        {
            if (data.Length != 2)
            {
                throw new ArgumentException();
            }

            string xmlUser = data[1];

            User updated = _service.UpdateUserAuth(GetEntity<User>(xmlUser));

            return EntityToXml(updated);
        }

        private string OnExport(string[] data)
        {
            if (data.Length != 3)
            {
                throw new ArgumentException();
            }

            string courseName = GetEntity<string>(data[1]);
            string exportFilePath = GetEntity<string>(data[2]);

            _service.Export(courseName, exportFilePath);

            return EntityToXml("Succesfull!");
        }

        private string OnImport(string[] data)
        {
            if (data.Length != 2)
            {
                throw new ArgumentException();
            }

            string importFilePath = GetEntity<string>(data[1]);
            _service.Import(importFilePath);

            return EntityToXml("Succesfull!");
        }

        private string OnReport(string[] data)
        {
            if (data.Length != 1)
            {
                throw new ArgumentException();
            }

            _service.GenerateReport();

            return EntityToXml("Succesfull!");
        }

        private string OnInsert(string[] data)
        {
            if (data.Length != 3)
            {
                throw new ArgumentException();
            }

            string entityType = GetEntity<string>(data[1]);
            string entityXml = data[2];
            long resID = 0;

            if (entityType == "user")
            {
                resID = _service.Insert(GetEntity<User>(entityXml));
            }
            else if (entityType == "course")
            {
                resID = _service.Insert(GetEntity<Course>(entityXml));
            }
            else if (entityType == "lecture")
            {
                resID = _service.Insert(GetEntity<Lecture>(entityXml));
            }
            else if (entityType == "subscription")
            {
                resID = _service.Insert(GetEntity<Subscription>(entityXml));
            }
            else
            {
                throw new ArgumentException($"Unknown entity {entityType}.");
            }
            return EntityToXml(resID);
        }

        private string OnGet(string[] data)
        {
            if (data.Length != 3)
            {
                throw new ArgumentException();
            }

            string entityType = GetEntity<string>(data[1]);
            string entityXml = data[2];
            string res = String.Empty;

            if (entityType == "user")
            {
                var user = _service.GetUser(GetEntity<long>(entityXml));
                res = EntityToXml(user);
            }
            else if (entityType == "course")
            {
                var course = _service.GetCourse(GetEntity<long>(entityXml));
                res = EntityToXml(course);
            }
            else if (entityType == "lecture")
            {
                var lecture = _service.GetLecture(GetEntity<long>(entityXml));
                res = EntityToXml(lecture);
            }
            else if (entityType == "subscription")
            {
                var subscription = _service.GetSubscription(GetEntity<Subscription>(entityXml));
                res = EntityToXml(subscription);
            }
            else
            {
                throw new ArgumentException($"Unknown entity {entityType}.");
            }
            return res;
        }

        private string OnUpdate(string[] data)
        {
            if (data.Length != 4)
            {
                throw new ArgumentException();
            }

            string entityType = GetEntity<string>(data[1]);
            long id = GetEntity<long>(data[2]);
            string entityXml = data[3];
            bool updated = false;

            if (entityType == "user")
            {
                updated = _service.Update(id, GetEntity<User>(entityXml));
            }
            else if (entityType == "course")
            {
                updated = _service.Update(id, GetEntity<Course>(entityXml));
            }
            else if (entityType == "lecture")
            {
                updated = _service.Update(id, GetEntity<Lecture>(entityXml));
            }
            else
            {
                throw new ArgumentException($"Unknown entity {entityType}.");
            }
            return EntityToXml(updated);
        }

        private string OnDelete(string[] data)
        {
            if (data.Length != 3)
            {
                throw new ArgumentException();
            }

            string entityType = GetEntity<string>(data[1]);
            long id = GetEntity<long>(data[2]);

            bool deleted = false;
            if (entityType == "user")
            {
                deleted = _service.DeleteUser(id);
            }
            else if (entityType == "course")
            {
                deleted = _service.DeleteCourse(id);
            }
            else if (entityType == "courses")
            {
                deleted = _service.DeleteCourses(id);
            }
            else if (entityType == "lecture")
            {
                deleted = _service.DeleteLecture(id);
            }
            else if (entityType == "lectures")
            {
                deleted = _service.DeleteLectures(id);
            }
            else if (entityType == "subscription")
            {
                deleted = _service.DeleteSubscription(id);
            }
            else
            {
                throw new ArgumentException($"Unknown entity {entityType}.");
            }
            return EntityToXml(deleted);
        }

        private string OnGetPage(string[] data)
        {
            if (data.Length != 6)
            {
                throw new ArgumentException();
            }

            string entityType = GetEntity<string>(data[1]);
            int pageNumber = GetEntity<int>(data[2]);
            int pageSize = GetEntity<int>(data[3]);
            string word = GetEntity<string>(data[4]);
            long id = GetEntity<long>(data[5]);

            string res = String.Empty;
            if (entityType == "user")
            {
                var list = _service.GetPageUser(pageNumber, pageSize, word);
                res = EntityToXml(list);
            }
            else if (entityType == "course")
            {
                var list = _service.GetPageCourse(pageNumber, pageSize, word, id);
                res = EntityToXml(list);
            }
            else if (entityType == "lecture")
            {
                var list = _service.GetPageLecture(pageNumber, pageSize, word, id);
                res = EntityToXml(list);
            }
            else
            {
                throw new ArgumentException($"Unknown entity {entityType}.");
            }
            return res;
        }

        private string OnGetTotalPagesCount(string[] data)
        {
            if (data.Length != 5)
            {
                throw new ArgumentException();
            }

            string entityType = GetEntity<string>(data[1]);
            int pageSize = GetEntity<int>(data[2]);
            string word = GetEntity<string>(data[3]);

            int count = 0;
            if (entityType == "user")
            {
                count = _service.GetTotalPagesCountUser(pageSize, word);
            }
            else if (entityType == "course")
            {
                long authorID = GetEntity<long>(data[4]);
                count = _service.GetTotalPagesCountCourse(pageSize, authorID, word);
            }
            else if (entityType == "lecture")
            {
                long userID = GetEntity<long>(data[4]);
                count = _service.GetTotalPagesCountLecture(pageSize, word, userID);
            }
            else
            {
                throw new ArgumentException($"Unknown entity {entityType}.");
            }
            return EntityToXml(count);
        }
    }
}