using Microsoft.Data.Sqlite;

namespace Repositories
{
    public class LectionRepository
    {
        private SqliteConnection _connection;

        public LectionRepository(string dataBaseFileName)
        {
            _connection = new SqliteConnection($"Data Source={dataBaseFileName}");
        }    

        public long Insert(Lection lection, int courseID)
        {
            _connection.Open();
            var command = _connection.CreateCommand();
            command.CommandText = 
            @"
                INSERT INTO lections (theme, course_id) 
                VALUES ($theme, $course_id);
                SELECT last_insert_rowid();
            ";
            command.Parameters.AddWithValue("$theme", lection.Theme);
            command.Parameters.AddWithValue("$course_id", courseID);
            long id = (long)command.ExecuteScalar();
            _connection.Close();            

            return id;
        }

        public Lection GetLection(int id)
        {
            _connection.Open();
            var command = _connection.CreateCommand();
            command.CommandText = @"
            SELECT * 
            FROM lections CROSS JOIN courses 
            WHERE lections.course_id = courses.id AND lections.id = $id;
            ";
            command.Parameters.AddWithValue("$id", id);
            
            SqliteDataReader reader = command.ExecuteReader();
            Lection lection = null;    

            if (reader.Read())
            {
                int lid = int.Parse(reader.GetString(0));
                string theme = reader.GetString(1);
                int cid = int.Parse(reader.GetString(3));
                string author = reader.GetString(4);
                string name = reader.GetString(5);
                int sbs = int.Parse(reader.GetString(6));
                int lecs = int.Parse(reader.GetString(7));

                // Course course = new Course(cid, author, name, sbs, lecs);
                lection = new Lection(theme);
            }

            reader.Close();
            _connection.Close();

            return lection;
        }

        public int Update(Lection lection, int lectionID, int courseID)
        {
            _connection.Open();
            var command = _connection.CreateCommand();
            command.CommandText = 
            @"
                UPDATE lections 
                SET 
                theme = $theme, 
                course_id = $course_id
                WHERE id = $id
            ";
            command.Parameters.AddWithValue("$id", lectionID);
            command.Parameters.AddWithValue("$theme", lection.Theme);
            command.Parameters.AddWithValue("$course_id", courseID);
            
            int count = command.ExecuteNonQuery();
            _connection.Close();            

            return count;
        }
    
        public int Delete(int id)
        {
            _connection.Open();
            var command = _connection.CreateCommand();
            command.CommandText = @"DELETE FROM lections WHERE id = $id";
            command.Parameters.AddWithValue("$id", id);
            int count = command.ExecuteNonQuery();
            _connection.Close();
            
            return count;
        }
   }
}