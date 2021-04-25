using Microsoft.Data.Sqlite;

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
                INSERT INTO courses (author, name, number_of_subscribers, number_of_lections) 
                VALUES ($author, $name, $number_of_subscribers, $number_of_lections);
                SELECT last_insert_rowid();
            ";
            command.Parameters.AddWithValue("$author", course.Author);
            command.Parameters.AddWithValue("$name", course.Name);
            command.Parameters.AddWithValue("$number_of_subscribers", course.NumberOfSubscribers);
            command.Parameters.AddWithValue("$number_of_lections", course.NumberOfLections);
            long id = (long)command.ExecuteScalar();
            _connection.Close();            

            return id;
        }

        public Course GetCourse(int id)
        {
            _connection.Open();
            var command = _connection.CreateCommand();
            command.CommandText = @"SELECT * FROM courses WHERE id = $id";
            command.Parameters.AddWithValue("$id", id);
            
            SqliteDataReader reader = command.ExecuteReader();
            Course course = null;    

            if (reader.Read())
            {
                int cid = int.Parse(reader.GetString(0));
                string author = reader.GetString(1);
                string name = reader.GetString(2);
                int sbs = int.Parse(reader.GetString(3));
                int lecs = int.Parse(reader.GetString(4));

                course = new Course(author, name, sbs, lecs);
            }

            reader.Close();
            _connection.Close();

            return course;
        }

        public int Update(Course course, int ID)
        {
            _connection.Open();
            var command = _connection.CreateCommand();
            command.CommandText = 
            @"
                UPDATE courses 
                SET 
                author = $author, 
                name = $name,
                number_of_subscribers = $number_of_subscribers, 
                number_of_lections = $number_of_lections
                WHERE id = $id;
            ";
            command.Parameters.AddWithValue("$id", ID);
            command.Parameters.AddWithValue("$author", course.Author);
            command.Parameters.AddWithValue("$name", course.Name);
            command.Parameters.AddWithValue("$number_of_subscribers", course.NumberOfSubscribers);
            command.Parameters.AddWithValue("$number_of_lections", course.NumberOfLections);
            
            int count = command.ExecuteNonQuery();
            _connection.Close();            

            return count;
        }
    
        public int Delete(int id)
        {
            _connection.Open();
            var command = _connection.CreateCommand();
            command.CommandText = @"DELETE FROM courses WHERE id = $id";
            command.Parameters.AddWithValue("$id", id);
            int count = command.ExecuteNonQuery();
            _connection.Close();
            
            return count;
        }
    }
}