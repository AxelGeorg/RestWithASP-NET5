using RestWithASPNETUdemy.Data.VO;
using RestWithASPNETUdemy.Model;
using RestWithASPNETUdemy.Model.Context;
using System;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace RestWithASPNETUdemy.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly MySQLContext _context;

        public UserRepository(MySQLContext context)
        {
            _context = context;
        }

        public User ValidateCredentials(UserVO user)
        {
            string pass = ComputeHash(user.Password, new SHA256CryptoServiceProvider());

            return _context.Users.FirstOrDefault(u => (u.UserName == user.UserName) && (u.Password == pass));
        }

        public User ValidateCredentials(string userName)
        {
            return _context.Users.SingleOrDefault(u => (u.UserName == userName));
        }

        public bool RevokeToken(string userName)
        {
            User user = _context.Users.SingleOrDefault(u => (u.UserName == userName));
            if (user is null)
                return false;

            user.RefreshToken = null;
            _context.SaveChanges();

            return true;
        }

        public User RefreshUserInfo(User user)
        {
            if (!Exists(user.Id))
                return null;

            User result = _context.Users.SingleOrDefault(p => p.Id.Equals(user.Id));
            if (result == null)
                return result;

            try
            {
                _context.Entry(result).CurrentValues.SetValues(user);
                _context.SaveChanges();

                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private string ComputeHash(string input, SHA256CryptoServiceProvider algorithm)
        {
            Byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            Byte[] hashedBytes = algorithm.ComputeHash(inputBytes);

            return BitConverter.ToString(hashedBytes);
        }

        public bool Exists(long id)
        {
            return _context.Users.Any(u => u.Id.Equals(id));
        }
    }
}
