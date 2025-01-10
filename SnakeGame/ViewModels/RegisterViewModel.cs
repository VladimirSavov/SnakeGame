using SnakeGame.Data.Models;
using SnakeGame.DataAccess;
using System.Threading.Tasks;

namespace SnakeGame.ViewModels
{
    public class RegisterViewModel : BaseViewModel
    {
        private readonly IUserRepo _userRepo;

        public RegisterViewModel(IUserRepo userRepo)
        {
            _userRepo = userRepo ?? throw new ArgumentNullException(nameof(userRepo));
        }

        public async Task<bool> Register(string username, string email, string password, string confirmPassword)
        {
            try
            {
                var existingUser = await _userRepo.GetUserByUsername(username);
                if (existingUser != null)
                {
                    return false; // Потребителското име вече съществува
                }

                var existingEmail = await _userRepo.GetUserByEmail(email);
                if (existingEmail != null)
                {
                    return false; // Имейлът вече е регистриран
                }

                var user = new User
                {
                    Username = username,
                    Email = email,
                    Password = password
                };

                // Регистрираме новия потребител
                var registeredUser = await _userRepo.Register(user);
                return registeredUser != null; // Успешна регистрация
            }
            catch (Exception)
            {
                return false; // Грешка при регистрация
            }
        }
    }
}
