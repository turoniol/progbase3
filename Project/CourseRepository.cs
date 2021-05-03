using Microsoft.Data.Sqlite;
using System.Collections.Generic;

namespace Repositories
{
    public class CourseRepository
    {
        private SqliteConnection _connection;

        public CourseRepository(string dataBaseFileName)
        {
            _connection = new SqliteConnection($"Data Source={dataBaseFileName}");
        }    

        public long Insert(Course course)
        {
            _connection.Open();
            var command = _connection.CreateCommand();
            command.CommandText = 
            @"
                INSERT INTO courses (name, author_id) 
                VALUES ($name, $author_id);
                SELECT last_insert_rowid();
            ";
            command.Parameters.AddWithValue("$name", course.Name);
            command.Parameters.AddWithValue("$author_id", course.Author.ID);

            long id = (long)command.ExecuteScalar();
            _connection.Close();            

            return id;
        }

        public Course GetCourse(long id)
        {
            _connection.Open();
            var command = _connection.CreateCommand();
            command.CommandText = @"SELECT * FROM courses WHERE id = $id";
            command.Parameters.AddWithValue("$id", id);
            
            SqliteDataReader reader = command.ExecuteReader();
            Course course = new Course();    

            if (reader.Read())
            {
                course.ID = int.Parse(reader.GetString(0));
                course.Name = reader.GetString(1);
            }

            reader.Close();
            _connection.Close();

            return course;
        }

        public bool Update(long ID, Course course)
        {
            _connection.Open();
            var command = _connection.CreateCommand();
            command.CommandText = 
            @"
                UPDATE courses 
                SET 
                name = $name,
                author_id = $author_id 
                WHERE id = $id;
            ";
            command.Parameters.AddWithValue("$id", ID);
            command.Parameters.AddWithValue("$name", course.Name);
            command.Parameters.AddWithValue("$author_id", course.Author.ID);
            
            int count = command.ExecuteNonQuery();
            _connection.Close();            

            return count == 1;
        }
    
        public bool Delete(long id)
        {
            _connection.Open();
            var command = _connection.CreateCommand();
            command.CommandText = @"DELETE FROM courses WHERE id = $id";
            command.Parameters.AddWithValue("$id", id);
            int count = command.ExecuteNonQuery();
            _connection.Close();
            
            return count == 1;
        }
   
        public List<Course> GetCoursesByAuthor(long authorID)
        {
            _connection.Open();
            var command = _connection.CreateCommand();

            command.CommandText = @"SELECT * FROM courses WHERE author_id = $id";
            command.Parameters.AddWithValue("$id", authorID);

            List<Course> courses = new List<Course>();
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                Course course = new Course();
                course.ID = long.Parse(reader.GetString(0));
                course.Name = reader.GetString(1);

                courses.Add(course);
            }
            reader.Close();
            _connection.Close();

            return courses;
        }
    
        public List<Course> GetCoursesByListener(long listenerID)
        {
            _connection.Open();
            var command = _connection.CreateCommand();

            command.CommandText = @"SELECT course_id, name
                FROM users_courses, courses 
                WHERE users_courses.user_id = $id AND users_courses.course_id = courses.id;";
            command.Parameters.AddWithValue("$id", listenerID);

            List<Course> courses = new List<Course>();
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                Course course = new Course();
                course.ID = long.Parse(reader.GetString(0));
                course.Name = reader.GetString(1);

                courses.Add(course);
            }
            reader.Close();
            _connection.Close();

            return courses;
        }
    
        public List<long> GetAllIDs()
        {
            _connection.Open();
            var command = _connection.CreateCommand();

            command.CommandText = @"SELECT id FROM courses";

            List<long> ids = new List<long>();
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                ids.Add(long.Parse(reader.GetString(0)));
            }

            reader.Close();
            _connection.Close();

            return ids;
        }

        public long MakeRelationship(User user, Course course)
        {
            _connection.Open();
            var command = _connection.CreateCommand();

            command.CommandText = @"
                INSERT INTO users_courses (user_id, course_id)
                VALUES ($user_id, $course_id);
                SELECT last_insert_rowid();
            ";

            command.Parameters.AddWithValue("$user_id", user.ID);
            command.Parameters.AddWithValue("$course_id", course.ID);
            long ID = (long) command.ExecuteScalar();

            return ID;
        }

        public List<Course> GetCourseExport(string word)
        {
            _connection.Open();
            var command = _connection.CreateCommand();

            command.CommandText = @"
                SELECT theme, courses.id, name FROM lectures, courses 
                WHERE courses.name 
                LIKE '%' || $value || '%' AND lectures.course_id = courses.id;
            ";
            command.Parameters.AddWithValue("$value", word);
            
            Dictionary<long, Course> courses = new Dictionary<long, Course>();
           
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                long ID = long.Parse(reader.GetString(1));
                string courseName = reader.GetString(2);
                string lectureTheme = reader.GetString(0);
                
                Lecture lecture = new Lecture();
                lecture.Theme = lectureTheme;
                Course course = null;

                if (!courses.TryGetValue(ID, out course))
                {                
                    course = new Course();
                    course.ID = ID;
                    course.Name = courseName;
                    course.Lections = new List<Lecture>();
                    courses.Add(ID, course);
                }

                course.Lections.Add(lecture);
            }

            reader.Close();
            _connection.Close();

            return new List<Course>(courses.Values);
        }
    
        public List<Course> GetImageData(int n)
        {
            if (n <= 0)
            {
                throw new System.ArgumentException();
            }

            _connection.Open();
            var command = _connection.CreateCommand();

            command.CommandText = @"
                SELECT user_id, login, password, fullname, course_id, name 
                FROM users_courses, courses, users
                WHERE users_courses.course_id = courses.id AND users_courses.user_id = users.id;
            ";

            var reader = command.ExecuteReader();
            Dictionary<long, Course> coursesDictionary = new Dictionary<long, Course>();
            while (reader.Read())
            {
                User user = new User();
                user.ID = long.Parse(reader.GetString(0));
                user.Login = reader.GetString(1);
                user.Password = reader.GetString(2);
                user.Fullname = reader.GetString(3);

                Course course = null;
                long ID = long.Parse(reader.GetString(4));

                if (!coursesDictionary.TryGetValue(ID, out course))
                {
                    course = new Course();                
                    course.ID = ID;
                    course.Name = reader.GetString(5);
                    course.Subscribers = new List<User>();
                    coursesDictionary.Add(course.ID, course);
                }

                course.Subscribers.Add(user);
            }

            reader.Close();
            _connection.Close();

            List<Course> courses = new List<Course>(coursesDictionary.Values);
            Dictionary<int, List<long>> counts = new Dictionary<int, List<long>>();
            foreach (var course in courses)
            {
                int count = course.Subscribers.Count;
                List<long> ids = null;
                if (!counts.TryGetValue(count, out ids))
                {
                    ids = new List<long>();
                    counts.Add(count, ids);
                }
                if (!ids.Contains(course.ID))
                {
                    ids.Add(course.ID);
                }
            }
            List<int> sortedCounts = new List<int>(counts.Keys);
            sortedCounts.Sort();

            List<Course> result = new List<Course>();

            n = System.Math.Min(sortedCounts.Count, n);
            for (int i = 0; i < n; ++i)
            {
                int j = sortedCounts.Count - 1 - i;
                int count = sortedCounts[j];
                var ids = counts[count];
                foreach (var id in ids)
                {
                    result.Add(coursesDictionary[id]);
                }
            }
            
            return result;
        }
    }
}