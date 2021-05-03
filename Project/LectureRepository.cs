using Microsoft.Data.Sqlite;
using System.Collections.Generic;

namespace Repositories
{
    public class LectureRepository
    {
        private SqliteConnection _connection;

        public LectureRepository(string dataBaseFileName)
        {
            _connection = new SqliteConnection($"Data Source={dataBaseFileName}");
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
            command.Parameters.AddWithValue("$course_id", lecture.course.ID);
            long id = (long)command.ExecuteScalar();
            _connection.Close();            

            return id;
        }

        public Lecture GetLection(long id)
        {
            _connection.Open();
            var command = _connection.CreateCommand();
            command.CommandText = @"
                SELECT * 
                FROM lectures
                WHERE id = $id;
            ";
            command.Parameters.AddWithValue("$id", id);
            
            SqliteDataReader reader = command.ExecuteReader();
            Lecture lection = new Lecture(); 

            if (reader.Read())
            {
                lection.ID = long.Parse(reader.GetString(0));
                lection.Theme = reader.GetString(1);
            }

            reader.Close();
            _connection.Close();

            return lection;
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
            command.Parameters.AddWithValue("$course_id", lecture.course.ID);
            
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
   
        public List<Lecture> GetLectionsByCourse(long courseID)
        {
            _connection.Open();
            var command = _connection.CreateCommand();

            command.CommandText = @"
                SELECT * FROM lectures
                WHERE course_id = $id;
            ";
            command.Parameters.AddWithValue("$id", courseID);

            var reader = command.ExecuteReader();
            List<Lecture> lections = new List<Lecture>();
            while (reader.Read())
            {
                Lecture lection = new Lecture();
                lection.ID = long.Parse(reader.GetString(0));
                lection.Theme = reader.GetString(1);
                lections.Add(lection);
            }
            reader.Close();
            _connection.Close();

            return lections;
        }
   }
}