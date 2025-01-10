using SnakeGame;
using SnakeGame.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnakeGame.DataAccess
{
    public interface IUserRepo
    {
        Task<User> GetUserByUsername(string username);
        Task<User> GetUserByEmail(string email);
        Task<User> Login(string username, string password);
        Task<User> Register(User user);
    }
}
