using EntitiesProcessingLib.Repositories;
using EntitiesProcessingLib.Entities;
using System.Security.Cryptography;
using System.Text;
using System;

namespace EntitiesProcessingLib.Authentication
{
    public class Authenticator
    {
        private UserRepository _rep;

        public Authenticator(UserRepository rep)
        {
            this._rep = rep;
        }

        public User Register(User user)
        {
            string login = user.Login;
            string password = user.Password;

            if (_rep.GetUser(login) != null)
            {
                return null;
            }

            SHA256 sha256Hash = SHA256.Create();
            string hashed = GetHash(sha256Hash, password);
            User result = new User {
                Login = login, 
                Password = hashed,
                Fullname = user.Fullname,
            };
            _rep.Insert(result);
            sha256Hash.Dispose();

            return result;
        }

        public User UpdateUser(User user)
        {
            _rep.GetUser(user.ID);

            SHA256 sha256Has = SHA256.Create();
            string hashed = GetHash(sha256Has, user.Password);
            User result = new User {
                ID = user.ID,
                Login = user.Login,
                Password = hashed,
                Fullname = user.Fullname,
            };
            _rep.Update(user.ID, result);

            return result;
        }

        public User Login(User user)
        {
            var searched = _rep.GetUser(user.Login);
            if (searched == null)
            {
                return null;
            }
            string hashedPassword = searched.Password;
            SHA256 sha256Hash = SHA256.Create();
            if (VerifyHash(sha256Hash, user.Password, hashedPassword))
            {
                return searched;
            }
            sha256Hash.Dispose();
            return null;
        }

        private static string GetHash(HashAlgorithm hashAlgorithm, string input)
        {
            byte[] data = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(input));

            var sBuilder = new StringBuilder();

            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            return sBuilder.ToString();
        }

        private static bool VerifyHash(HashAlgorithm hashAlgorithm, string input, string hash)
        {
            var hashOfInput = GetHash(hashAlgorithm, input);

            StringComparer comparer = StringComparer.OrdinalIgnoreCase;

            return comparer.Compare(hashOfInput, hash) == 0;
        }

    }
}