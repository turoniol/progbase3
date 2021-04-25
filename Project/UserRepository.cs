using Microsoft.Data.Sqlite;

namespace Repositories
{
    public class UserRepository
    {
        private SqliteConnection _connection;

        public UserRepository(string dataBaseFileName)
        {
            _connection = new SqliteConnection($"Data Source={dataBaseFileName}");
        }    

        public long Insert(User user)
        {
            _connection.Open();
            var command = _connection.CreateCommand();
            command.CommandText = 
            @"
                INSERT INTO users (login, password, status) 
                VALUES ($login, $password, $status);
                SELECT last_insert_rowid();
            ";
            command.Parameters.AddWithValue("$login", user.Login);
            command.Parameters.AddWithValue("$password", user.Password);
            command.Parameters.AddWithValue("$status", user.Status);

            long id = (long)command.ExecuteScalar();
            _connection.Close();            

            return id;
        }

        public User GetCourse(int id)
        {
            _connection.Open();
            var command = _connection.CreateCommand();
            command.CommandText = @"SELECT * FROM users WHERE id = $id";
            command.Parameters.AddWithValue("$id", id);
            
            SqliteDataReader reader = command.ExecuteReader();
            User user = null;    

            if (reader.Read())
            {
                int uid = int.Parse(reader.GetString(0));
                string login = reader.GetString(1);
                string password = reader.GetString(2);
                string status = reader.GetString(3);

                user = new User(login, password, status);
            }

            reader.Close();
            _connection.Close();

            return user;
        }

        public int Update(User user, int ID)
        {
            _connection.Open();
            var command = _connection.CreateCommand();
            command.CommandText = 
            @"
                UPDATE users 
                SET 
                login = $login, 
                password = $password,
                status = $status
                WHERE id = $id;
            ";
            command.Parameters.AddWithValue("$login", user.Login);
            command.Parameters.AddWithValue("$password", user.Password);
            command.Parameters.AddWithValue("$status", user.Status);
            
            int count = command.ExecuteNonQuery();
            _connection.Close();            

            return count;
        }
    
        public int Delete(int id)
        {
            _connection.Open();
            var command = _connection.CreateCommand();
            command.CommandText = @"DELETE FROM users WHERE id = $id";
            command.Parameters.AddWithValue("$id", id);
            int count = command.ExecuteNonQuery();
            _connection.Close();
            
            return count;
        }
    }
}