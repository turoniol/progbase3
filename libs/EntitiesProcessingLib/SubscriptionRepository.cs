using Microsoft.Data.Sqlite;
using EntitiesProcessingLib.Entities;
using System.Collections.Generic;

namespace EntitiesProcessingLib.Repositories
{
    public class SubscriptionRepository
    {
        private SqliteConnection _connection;

        public SubscriptionRepository(string dataBaseFileName)
        {
            _connection = new SqliteConnection($"Data Source={dataBaseFileName}");
            _connection.Open();
            var command = _connection.CreateCommand();
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS users_courses (
                id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE, 
                user_id INTEGER NOT NULL,
                course_id INTEGER NOT NULL);
            ";
            command.ExecuteNonQuery();
            _connection.Close();
        }

        public long Insert(long userID, long courseID)
        {
            _connection.Open();
            var command = _connection.CreateCommand();

            command.CommandText = @"
                INSERT INTO users_courses (user_id, course_id)
                VALUES ($user_id, $course_id);
                SELECT last_insert_rowid();
            ";

            command.Parameters.AddWithValue("$user_id", userID);
            command.Parameters.AddWithValue("$course_id", courseID);
            long ID = (long) command.ExecuteScalar();

            return ID;
        }

        public Subscription GetSubscription(long userID, long courseID)
        {
            _connection.Open();
            var command = _connection.CreateCommand();
            command.CommandText = @"
                SELECT * FROM users_courses
                WHERE user_id = $user_id AND
                    course_id = $course_id;
            ";
            command.Parameters.AddWithValue("$course_id", courseID);
            command.Parameters.AddWithValue("$user_id", userID);
            
            SqliteDataReader reader = command.ExecuteReader();
            Subscription subs = null; 

            if (reader.Read())
            {
                
                subs = new Subscription {
                    id = long.Parse(reader.GetString(0)),
                    userID = long.Parse(reader.GetString(1)),
                    courseID = long.Parse(reader.GetString(0)),
                };
            }

            reader.Close();
            _connection.Close();

            return subs;
        }

        public bool Update(long id, Subscription subs)
        {
            _connection.Open();
            var command = _connection.CreateCommand();
            command.CommandText = 
            @"
                UPDATE users_courses 
                SET 
                user_id = $user_id, 
                course_id = $course_id
                WHERE id = $id
            ";
            command.Parameters.AddWithValue("$id", id);
            command.Parameters.AddWithValue("$user_id", subs.userID);
            command.Parameters.AddWithValue("$course_id", subs.courseID);
            
            int count = command.ExecuteNonQuery();
            _connection.Close();            

            return count == 1;
        }
    
        public bool Delete(long id)
        {
            _connection.Open();
            var command = _connection.CreateCommand();
            command.CommandText = @"DELETE FROM users_courses WHERE id = $id";
            command.Parameters.AddWithValue("$id", id);
            int count = command.ExecuteNonQuery();
            _connection.Close();
            
            return count == 1;
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
    }
}