using Microsoft.Data.Sqlite;
using System.Collections.Generic;

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
                INSERT INTO users (login, password, fullname)
                VALUES ($login, $password, $fullname);
                SELECT last_insert_rowid();
            ";
            command.Parameters.AddWithValue("$login", user.Login);
            command.Parameters.AddWithValue("$password", user.Password);
            command.Parameters.AddWithValue("$fullname", user.Fullname);

            long id = (long)command.ExecuteScalar();
            _connection.Close();            

            return id;
        }

        public User GetUser(long id)
        {
            _connection.Open();
            var command = _connection.CreateCommand();
            command.CommandText = @"SELECT * FROM users WHERE id = $id";
            command.Parameters.AddWithValue("$id", id);
            
            SqliteDataReader reader = command.ExecuteReader();
            User user = new User();

            if (reader.Read())
            {
                user.ID = int.Parse(reader.GetString(0));
                user.Login = reader.GetString(1);
                user.Password = reader.GetString(2);
                user.Fullname = reader.GetString(3);
            }

            reader.Close();
            _connection.Close();

            return user;
        }

        public bool Update(long ID, User user)
        {
            _connection.Open();
            var command = _connection.CreateCommand();
            command.CommandText = 
            @"
                UPDATE users 
                SET 
                login = $login, 
                password = $password,
                fullname = $fullname
                WHERE id = $id;
            ";
            command.Parameters.AddWithValue("$login", user.Login);
            command.Parameters.AddWithValue("$password", user.Password);
            command.Parameters.AddWithValue("$fullname", user.Fullname);
            command.Parameters.AddWithValue("$id", ID);
            
            int count = command.ExecuteNonQuery();
            _connection.Close();            

            return count == 1;
        }
    
        public bool Delete(long id)
        {
            _connection.Open();
            var command = _connection.CreateCommand();
            command.CommandText = @"DELETE FROM users WHERE id = $id";
            command.Parameters.AddWithValue("$id", id);
            int count = command.ExecuteNonQuery();
            _connection.Close();
            
            return count == 1;
        }
    
        public List<long> GetAllIDs()
        {
            _connection.Open();
            var command = _connection.CreateCommand();

            command.CommandText = @"SELECT id FROM users";

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
    }
}