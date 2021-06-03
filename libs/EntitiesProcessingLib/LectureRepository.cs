using Microsoft.Data.Sqlite;
using EntitiesProcessingLib.Entities;
using System.Collections.Generic;

namespace EntitiesProcessingLib.Repositories
{
    public class LectureRepository
    {
        private SqliteConnection _connection;

        public LectureRepository(string dataBaseFileName)
        {
            _connection = new SqliteConnection($"Data Source={dataBaseFileName}");
            _connection.Open();
            var command = _connection.CreateCommand();
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS lectures (
                id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE, 
                theme TEXT NOT NULL,
                course_id INTEGER NOT NULL,
                imported TEXT DEFAULT 'False');
            ";
            command.ExecuteNonQuery();
            _connection.Close();
        }    

        public long Insert(Lecture lecture)
        {
            _connection.Open();
            var command = _connection.CreateCommand();
            command.CommandText = 
            @"
                INSERT INTO lectures (theme, course_id) 
                VALUES ($theme, $course_id);
                SELECT last_insert_rowid();
            ";
            command.Parameters.AddWithValue("$theme", lecture.Theme);
            command.Parameters.AddWithValue("$course_id", lecture.Course.ID);
            long id = (long)command.ExecuteScalar();
            _connection.Close();            

            return id;
        }

        public long Insert(long id, Lecture lecture)
        {
            _connection.Open();
            var command = _connection.CreateCommand();
            command.CommandText = 
            @"
                INSERT INTO lectures (id, theme, course_id, imported) 
                VALUES ($id, $theme, $course_id, $imported);
                SELECT last_insert_rowid();
            ";
            command.Parameters.AddWithValue("$id", id);
            command.Parameters.AddWithValue("$theme", lecture.Theme);
            command.Parameters.AddWithValue("$course_id", lecture.Course.ID);
            command.Parameters.AddWithValue("$imported", lecture.IsImported.ToString());
            long resId = (long) command.ExecuteScalar();
            _connection.Close();            

            return resId;
        }

        public Lecture GetLecture(long id)
        {
            _connection.Open();
            var command = _connection.CreateCommand();
            command.CommandText = @"
                SELECT * FROM lectures
                WHERE id = $id;
            ";
            command.Parameters.AddWithValue("$id", id);
            
            SqliteDataReader reader = command.ExecuteReader();
            Lecture lecture = null; 

            if (reader.Read())
            {
                lecture = new Lecture();
                lecture.ID = long.Parse(reader.GetString(0));
                lecture.Theme = reader.GetString(1);
                lecture.Course = new Course() {
                    ID = long.Parse(reader.GetString(2))
                };
                lecture.IsImported = bool.Parse(reader.GetString(3));
            }

            reader.Close();
            _connection.Close();

            return lecture;
        }

        public bool Update(long lectureID, Lecture lecture)
        {
            _connection.Open();
            var command = _connection.CreateCommand();
            command.CommandText = 
            @"
                UPDATE lectures 
                SET 
                theme = $theme, 
                course_id = $course_id
                WHERE id = $id
            ";
            command.Parameters.AddWithValue("$id", lectureID);
            command.Parameters.AddWithValue("$theme", lecture.Theme);
            command.Parameters.AddWithValue("$course_id", lecture.Course.ID);
            
            int count = command.ExecuteNonQuery();
            _connection.Close();            

            return count == 1;
        }
    
        public bool Delete(long id)
        {
            _connection.Open();
            var command = _connection.CreateCommand();
            command.CommandText = @"DELETE FROM lectures WHERE id = $id";
            command.Parameters.AddWithValue("$id", id);
            int count = command.ExecuteNonQuery();
            _connection.Close();
            
            return count == 1;
        }

        public bool DeleteLectures(long courseID)
        {
            _connection.Open();
            var command = _connection.CreateCommand();
            command.CommandText = @"DELETE FROM lectures WHERE course_id = $course_id";
            command.Parameters.AddWithValue("$course_id", courseID);
            int count = command.ExecuteNonQuery();
            _connection.Close();
            
            return count != 0;
        }
   
        public long GetTotalCount(string theme)
        {
            _connection.Open();
            var command = _connection.CreateCommand();

            command.CommandText = @"
                SELECT COUNT(*) FROM lectures
                WHERE theme LIKE '%' || $value || '%';
            ";
            command.Parameters.AddWithValue("$value", theme);

            long count = (long) command.ExecuteScalar();
            _connection.Close();

            return count;
        }

        public List<Lecture> GetPage(int pageNumber, int pageSize, string theme)
        {
            _connection.Open();
            var command = _connection.CreateCommand();

            command.CommandText = @"
                SELECT * FROM lectures
                WHERE theme LIKE '%' || $value || '%' 
                LIMIT $page_size OFFSET $page_number;
            ";
            command.Parameters.AddWithValue("$page_size", pageSize);
            command.Parameters.AddWithValue("$page_number", (pageNumber - 1) * pageSize);
            command.Parameters.AddWithValue("$value", theme);

            var reader = command.ExecuteReader();
            List<Lecture> lectures = new List<Lecture>();
            while (reader.Read())
            {
                lectures.Add(
                    new Lecture
                    {
                        ID = long.Parse(reader.GetString(0)),
                        Theme = reader.GetString(1),
                        Course = new Course { 
                            ID = long.Parse(reader.GetString(2)) 
                        },
                        IsImported = bool.Parse(reader.GetString(3)),
                    }
                );
            }
            reader.Close();
            _connection.Close();

            return lectures;
        }

        public int GetTotalPagesCount(int pageSize, string theme)
        {
            return (int) System.Math.Ceiling(GetTotalCount(theme) / (float) pageSize);
        }

        public List<Lecture> GetLecturesByCourse(long courseID)
        {
            _connection.Open();
            var command = _connection.CreateCommand();

            command.CommandText = @"
                SELECT * FROM lectures
                WHERE course_id = $id;
            ";
            command.Parameters.AddWithValue("$id", courseID);

            var reader = command.ExecuteReader();
            List<Lecture> lectures = new List<Lecture>();
            while (reader.Read())
            {
                Lecture lecture = new Lecture();
                lecture.ID = long.Parse(reader.GetString(0));
                lecture.Theme = reader.GetString(1);
                lecture.Course = new Course {
                    ID = long.Parse(reader.GetString(2)),
                };
                lecture.IsImported = bool.Parse(reader.GetString(3));
                lectures.Add(lecture);
            }
            reader.Close();
            _connection.Close();

            return lectures;
        }
   }
}