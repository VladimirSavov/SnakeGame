using SnakeGame.Data.Models;
using SnakeGame.DataAccess;
using SnakeGame.ViewModels;
using System.Text.RegularExpressions;

namespace SnakeGame;

public partial class RegisterPage : ContentPage
{
    private readonly RegisterViewModel _viewModel;
    private readonly Regex _emailRegex = new(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$");

    // Конструктор с RegisterViewModel
    public RegisterPage(RegisterViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    private async void OnLoginBtnClicked(object sender, EventArgs e)
    {
        try
        {
            await Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Грешка", "Неуспешно връщане към страницата за вход.", "ОК");
        }
    }

    private async void OnRegisterBtnClicked(object sender, EventArgs e)
    {
        try
        {
            if (!ValidateInputs(out string errorMessage))
            {
                await DisplayAlert("Грешка", errorMessage, "ОК");
                return;
            }

            var username = UsernameEntry.Text.Trim();
            var email = EmailEntry.Text.Trim();
            var password = PasswordEntry.Text;
            var confirmPassword = ConfirmPasswordEntry.Text;

            // Извикваме метода от ViewModel за регистрация
            bool isSuccess = await _viewModel.Register(username, email, password, confirmPassword);

            if (!isSuccess)
            {
                await DisplayAlert("Грешка", "Възникна грешка при регистрацията. Моля, опитайте отново.", "ОК");
                return;
            }

            // След успешна регистрация, връщаме към LoginPage
            await DisplayAlert("Успех", "Успешна регистрация!", "ОК");
            await Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Грешка", "Възникна грешка при регистрацията. Моля, опитайте отново.", "ОК");
        }
    }

    private bool ValidateInputs(out string errorMessage)
    {
        errorMessage = string.Empty;

        var username = UsernameEntry?.Text?.Trim();
        var email = EmailEntry?.Text?.Trim();
        var password = PasswordEntry?.Text;
        var confirmPassword = ConfirmPasswordEntry?.Text;

        if (string.IsNullOrWhiteSpace(username) ||
            string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(password) ||
            string.IsNullOrWhiteSpace(confirmPassword))
        {
            errorMessage = "Всички полета са задължителни!";
            return false;
        }

        if (username.Length < 3)
        {
            errorMessage = "Потребителското име трябва да е поне 3 символа.";
            return false;
        }

        if (!_emailRegex.IsMatch(email))
        {
            errorMessage = "Невалиден имейл адрес.";
            return false;
        }

        if (password.Length < 6)
        {
            errorMessage = "Паролата трябва да е поне 6 символа.";
            return false;
        }

        if (password != confirmPassword)
        {
            errorMessage = "Паролите не съвпадат.";
            return false;
        }

        return true;
    }
}
