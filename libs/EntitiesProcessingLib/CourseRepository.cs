using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using EntitiesProcessingLib.Entities;

namespace EntitiesProcessingLib.Repositories
{
    public class CourseRepository
    {
        private SqliteConnection _connection;

        public CourseRepository(string dataBaseFileName)
        {
            _connection = new SqliteConnection($"Data Source={dataBaseFileName}");
            _connection.Open();
            var command = _connection.CreateCommand();
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS courses (
                id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE, 
                name TEXT NOT NULL,
                author_id INTEGER NOT NULL,
                imported TEXT DEFAULT 'False');
            ";
            command.ExecuteNonQuery();
            _connection.Close();
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
            command.Parameters.AddWithValue("$name", course.Title);
            command.Parameters.AddWithValue("$author_id", course.Author.ID);

            long id = (long)command.ExecuteScalar();
            _connection.Close();            

            return id;
        }

        public long Insert(long id, Course course)
        {
            _connection.Open();
            var command = _connection.CreateCommand();
            command.CommandText = 
            @"
                INSERT INTO courses (id, name, author_id, imported) 
                VALUES ($id, $name, $author_id, $imported);
                SELECT last_insert_rowid();
            ";
            command.Parameters.AddWithValue("$id", id);
            command.Parameters.AddWithValue("$name", course.Title);
            command.Parameters.AddWithValue("$author_id", course.Author.ID);
            command.Parameters.AddWithValue("$imported", "True");

            long insertedID = (long)command.ExecuteScalar();
            _connection.Close();            

            return insertedID;
        }

        public Course GetCourse(long id)
        {
            _connection.Open();
            var command = _connection.CreateCommand();
            command.CommandText = @"SELECT * FROM courses WHERE id = $id";
            command.Parameters.AddWithValue("$id", id);
            
            SqliteDataReader reader = command.ExecuteReader();
            Course course = null;    

            if (reader.Read())
            {
                course = new Course();
                course.ID = long.Parse(reader.GetString(0));
                course.Title = reader.GetString(1);
                course.Author = new User() {
                    ID = long.Parse(reader.GetString(2)),
                };
                course.IsImported = bool.Parse(reader.GetString(3));
                course.CanSubcribe = bool.Parse(reader.GetString(4));
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
                author_id = $author_id,
                access = $access
                WHERE id = $id;
            ";
            command.Parameters.AddWithValue("$id", ID);
            command.Parameters.AddWithValue("$name", course.Title);
            command.Parameters.AddWithValue("$author_id", course.Author.ID);
            command.Parameters.AddWithValue("$access", course.CanSubcribe.ToString());

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
   
        public long GetTotalCount(string name)
        {
            _connection.Open();
            var command = _connection.CreateCommand();

            command.CommandText = @"
                SELECT COUNT(*) FROM courses
                WHERE name LIKE '%' || $value || '%';
            ";
            command.Parameters.AddWithValue("$value", name);

            long count = (long) command.ExecuteScalar();
            _connection.Close();

            return count;
        }

        public long GetTotalCount(long authorID, string name)
        {
            if (authorID == -1)
            {
                return GetTotalCount(name);
            }

            _connection.Open();
            var command = _connection.CreateCommand();

            command.CommandText = @"
                SELECT COUNT(*) FROM courses 
                WHERE author_id = $author_id AND
                    name LIKE '%' || $value || '%';
            ";
            command.Parameters.AddWithValue("$author_id", authorID);
            command.Parameters.AddWithValue("$value", name);

            long count = (long) command.ExecuteScalar();
            _connection.Close();

            return count;
        }

        public List<Course> GetPage(int pageNumber, int pageSize, string name)
        {
            _connection.Open();
            var command = _connection.CreateCommand();

            command.CommandText = @"
                SELECT * FROM courses 
                WHERE name LIKE '%' || $value || '%'
                LIMIT $page_size OFFSET $page_number;
            ";
            command.Parameters.AddWithValue("$page_size", pageSize);
            command.Parameters.AddWithValue("$page_number", (pageNumber - 1) * pageSize);
            command.Parameters.AddWithValue("$value", name);

            var reader = command.ExecuteReader();
            List<Course> courses = new List<Course>();
            while (reader.Read())
            {
                courses.Add(
                    new Course
                    {
                        ID = long.Parse(reader.GetString(0)),
                        Title = reader.GetString(1),
                        Author = new User { ID = long.Parse(reader.GetString(2)) },
                        IsImported = bool.Parse(reader.GetString(3)),
                        CanSubcribe = bool.Parse(reader.GetString(4)),
                    }
                );
            }
            reader.Close();
            _connection.Close();

            return courses;
        }

        public List<Course> GetPage(int pageNumber, int pageSize, long authorID, string name)
        {
            if (authorID == -1)
            {
                return GetPage(pageNumber, pageSize, name);
            }

            _connection.Open();
            var command = _connection.CreateCommand();

            command.CommandText = @"
                SELECT * FROM courses 
                WHERE author_id = $author_id AND
                    name LIKE '%' || $value || '%'
                LIMIT $page_size OFFSET $page_number;
            ";
            command.Parameters.AddWithValue("$page_size", pageSize);
            command.Parameters.AddWithValue("$page_number", (pageNumber - 1) * pageSize);
            command.Parameters.AddWithValue("$author_id", authorID);
            command.Parameters.AddWithValue("$value", name);

            var reader = command.ExecuteReader();
            List<Course> courses = new List<Course>();
            while (reader.Read())
            {
                courses.Add(
                    new Course
                    {
                        ID = long.Parse(reader.GetString(0)),
                        Title = reader.GetString(1),
                        Author = new User { ID = long.Parse(reader.GetString(2)) },
                        IsImported = bool.Parse(reader.GetString(3)),
                        CanSubcribe = bool.Parse(reader.GetString(4)),
                    }
                );
            }
            reader.Close();
            _connection.Close();

            return courses;
        }

        public int GetTotalPagesCount(int pageSize, string name)
        {
            return (int) System.Math.Ceiling(GetTotalCount(name) / (float) pageSize);
        }

        public int GetTotalPagesCount(int pageSize, long authorID, string name)
        {
            return (int) System.Math.Ceiling(GetTotalCount(authorID, name) / (float) pageSize);
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
                course.Title = reader.GetString(1);
                course.Author = new User { ID = authorID};
                course.IsImported = bool.Parse(reader.GetString(3));

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

        public List<Course> GetCourseExport(string word)
        {
            _connection.Open();
            var command = _connection.CreateCommand();

            command.CommandText = @"
                SELECT theme, courses.id, name, lectures.id, courses.author_id, courses.access
                FROM lectures, courses 
                WHERE courses.name LIKE '%' || $value || '%' AND 
                    lectures.course_id = courses.id;
            ";
            command.Parameters.AddWithValue("$value", word);
            
            Dictionary<long, Course> courses = new Dictionary<long, Course>();
           
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                long ID = long.Parse(reader.GetString(1));
                string courseName = reader.GetString(2);
                string lectureTheme = reader.GetString(0);
                long lectureID = long.Parse(reader.GetString(3));
                long authorID = long.Parse(reader.GetString(4));
                bool cabSubs = bool.Parse(reader.GetString(5));
                
                Lecture lecture = new Lecture() { ID = lectureID, Theme = lectureTheme, };
                Course course = null;

                if (!courses.TryGetValue(ID, out course))
                {                
                    course = new Course();
                    course.ID = ID;
                    course.Title = courseName;
                    course.Lectures = new List<Lecture>();
                    course.Author = new User {ID = authorID};
                    course.CanSubcribe = cabSubs;
                    courses.Add(ID, course);
                }

                course.Lectures.Add(lecture);
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
                SELECT user_id, course_id, name 
                FROM users_courses, courses
                WHERE users_courses.course_id = courses.id;
            ";

            var reader = command.ExecuteReader();
            Dictionary<long, Course> coursesDictionary = new Dictionary<long, Course>();
            while (reader.Read())
            {
                User user = new User(){ ID = long.Parse(reader.GetString(0)) };

                Course course = null;
                long ID = long.Parse(reader.GetString(1));

                if (!coursesDictionary.TryGetValue(ID, out course))
                {
                    course = new Course(){
                        ID = ID, 
                        Title = reader.GetString(2),
                        Subscribers = new List<User>(),
                    };
                    coursesDictionary.Add(course.ID, course);
                }

                course.Subscribers.Add(user);
            }

            reader.Close();
            _connection.Close();
            
            var sorted = EntitiesProcessingLib.DataProcessing.CourseProcessor.SortByListeners(coursesDictionary);
            n = System.Math.Min(sorted.Count, n);
            List<Course> result = new List<Course>(sorted.GetRange(0, n));

            return result;
        }
    }
}