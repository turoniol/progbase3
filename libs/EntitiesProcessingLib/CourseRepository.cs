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
            command.Parameters.AddWithValue("$imported", course.IsImported);

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
            command.Parameters.AddWithValue("$name", course.Title);
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
   
        public long GetTotalCount()
        {
            _connection.Open();
            var command = _connection.CreateCommand();

            command.CommandText = @"
                SELECT COUNT(*) FROM courses;
            ";

            long count = (long) command.ExecuteScalar();
            _connection.Close();

            return count;
        }

        public List<Course> GetPage(int pageNumber, int pageSize)
        {
            _connection.Open();
            var command = _connection.CreateCommand();

            command.CommandText = @"
                SELECT * FROM courses LIMIT $page_size OFFSET $page_number;
            ";
            command.Parameters.AddWithValue("$page_size", pageSize);
            command.Parameters.AddWithValue("$page_number", (pageNumber - 1) * pageSize);

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
                    }
                );
            }
            reader.Close();
            _connection.Close();

            return courses;
        }

        public int GetTotalPagesCount(int pageSize)
        {
            return (int) System.Math.Ceiling(GetTotalCount() / (float) pageSize);
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
                course.Title = reader.GetString(1);
                course.Author = new User { ID = long.Parse(reader.GetString(2)) };
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

        // public long MakeRelationship(User user, Course course)
        // {
        //     _connection.Open();
        //     var command = _connection.CreateCommand();

        //     command.CommandText = @"
        //         INSERT INTO users_courses (user_id, course_id)
        //         VALUES ($user_id, $course_id);
        //         SELECT last_insert_rowid();
        //     ";

        //     command.Parameters.AddWithValue("$user_id", user.ID);
        //     command.Parameters.AddWithValue("$course_id", course.ID);
        //     long ID = (long) command.ExecuteScalar();

        //     return ID;
        // }

        public List<Course> GetCourseExport(string word)
        {
            _connection.Open();
            var command = _connection.CreateCommand();

            command.CommandText = @"
                SELECT theme, courses.id, name, lectures.id
                FROM lectures, courses 
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
                long lectureID = long.Parse(reader.GetString(3));
                
                Lecture lecture = new Lecture() { ID = lectureID, Theme = lectureTheme, };
                Course course = null;

                if (!courses.TryGetValue(ID, out course))
                {                
                    course = new Course();
                    course.ID = ID;
                    course.Title = courseName;
                    course.Lectures = new List<Lecture>();
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
            
            var sorted = SortByListeners(coursesDictionary);
            List<Course> result = new List<Course>(sorted.GetRange(0, n));

            return result;
        }

        private List<Course> SortByListeners(Dictionary<long, Course> coursesDictionary)
        {
            Dictionary<int, List<long>> counts = new Dictionary<int, List<long>>();
            foreach (var course in coursesDictionary.Values)
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

            List<Course> sorted = new List<Course>();
            for (int i = 0; i < sortedCounts.Count; ++i)
            {
                int j = sortedCounts.Count - 1 - i;
                foreach (var id in counts[sortedCounts[j]])
                {
                    sorted.Add(coursesDictionary[id]);
                }
            }

            return sorted;
        }
    }
}