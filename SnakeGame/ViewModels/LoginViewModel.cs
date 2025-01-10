using SnakeGame.DataAccess;
using System.Threading.Tasks;

namespace SnakeGame.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly IUserRepo _userRepo;

        // Индикатор за текущото състояние (например, дали входът се извършва в момента)
        private bool _isProcessing;
        public bool IsProcessing
        {
            get => _isProcessing;
            set => SetProperty(ref _isProcessing, value);
        }

        private string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public LoginViewModel(IUserRepo userRepo)
        {
            _userRepo = userRepo;
        }

        // Основен метод за вход
        public async Task<bool> Login(string username, string password)
        {
            // Проверка дали входът е в процес
            if (IsProcessing)
                return false;

            // Валидация на входните данни
            if (!ValidateInputs(username, password))
            {
                ErrorMessage = "Моля, въведете валидни данни.";
                return false;
            }

            try
            {
                IsProcessing = true; // Започваме процеса на вход

                // Опитваме да се логнем
                var user = await _userRepo.Login(username, password);
                if (user == null)
                {
                    ErrorMessage = "Невалидно потребителско име или парола";
                    return false;
                }

                // Входът е успешен
                return true;
            }
            catch (Exception ex)
            {
                ErrorMessage = "Възникна грешка при опит за вход: " + ex.Message;
                return false;
            }
            finally
            {
                IsProcessing = false; // Завършваме процеса на вход
            }
        }

        // Валидация на входните данни
        private bool ValidateInputs(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ErrorMessage = "Потребителското име и паролата не могат да бъдат празни.";
                return false;
            }

            return true;
        }
    }
}
