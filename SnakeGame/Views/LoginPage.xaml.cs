using SnakeGame.Data.Models;
using SnakeGame.DataAccess;
using SnakeGame.ViewModels;

namespace SnakeGame;

public partial class LoginPage : ContentPage
{
    private readonly LoginViewModel _viewModel;
    private bool _isProcessing;

    // Преобразуваме конструктора да приема LoginViewModel
    public LoginPage(LoginViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel; // Свързваме ViewModel със страницата
    }

    private async void OnRegisterButtonClicked(object sender, EventArgs e)
    {
        if (_isProcessing) return;

        try
        {
            var registerPage = ActivatorUtilities.CreateInstance<RegisterPage>(Application.Current.Handler.MauiContext.Services);

            await Navigation.PushAsync(registerPage);
        }
        catch (Exception ex)
        {
            await ShowError("Неуспешно навигиране към страницата за регистрация.");
        }
    }

    private async void OnLoginButtonClicked(object sender, EventArgs e)
    {
        if (_isProcessing) return;

        if (!ValidateInputs(out string errorMessage))
        {
            await ShowError(errorMessage);
            return;
        }

        await ProcessLogin();
    }

    private async Task ProcessLogin()
    {
        try
        {
            SetProcessingState(true);

            var username = UsernameEntry.Text.Trim();
            var password = PasswordEntry.Text;

            // Използваме метода Login от ViewModel
            bool isSuccess = await _viewModel.Login(username, password);

            if (!isSuccess)
            {
                await ShowError("Невалидно потребителско име или парола");
                return;
            }

            // След успешен вход, запазваме данните за потребителя и навигираме
            await SaveUserDataAndNavigate(username);
        }
        catch (Exception ex)
        {
            await ShowError("Грешка при опит за вход");
        }
        finally
        {
            SetProcessingState(false);
        }
    }

    private async Task SaveUserDataAndNavigate(string username)
    {
        try
        {
            // Записваме данните за потребителя в SecureStorage
            await SecureStorage.SetAsync("username", username);

            await DisplayAlert("Успех", "Добре дошли в играта!", "ОК");

            // Заменяме текущата страница с MainPage
            Application.Current.MainPage = new NavigationPage(new MainPage());
        }
        catch (Exception ex)
        {
            await ShowError("Грешка при запазване на данните. Моля, опитайте отново.");
            // Изтриваме всички записани данни при грешка
            SecureStorage.Default.RemoveAll();
        }
    }

    private bool ValidateInputs(out string errorMessage)
    {
        errorMessage = string.Empty;

        var username = UsernameEntry?.Text?.Trim();
        var password = PasswordEntry?.Text;

        if (string.IsNullOrWhiteSpace(username))
        {
            errorMessage = "Моля, въведете потребителско име.";
            return false;
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            errorMessage = "Моля, въведете парола.";
            return false;
        }

        return true;
    }

    private void SetProcessingState(bool isProcessing)
    {
        _isProcessing = isProcessing;
        LoadingIndicator.IsVisible = isProcessing;
        LoadingIndicator.IsRunning = isProcessing;
        LoginButton.IsEnabled = !isProcessing;
        RegisterButton.IsEnabled = !isProcessing;
        UsernameEntry.IsEnabled = !isProcessing;
        PasswordEntry.IsEnabled = !isProcessing;
    }

    private async Task ShowError(string message)
    {
        await DisplayAlert("Грешка", message, "ОК");
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Проверяваме дали вече има влязъл потребител
        var existingUsername = await SecureStorage.GetAsync("username");
        if (!string.IsNullOrEmpty(existingUsername))
        {
            Application.Current.MainPage = new NavigationPage(new MainPage());
        }
    }
}
