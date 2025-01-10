using SnakeGame.Data.Models;
using SnakeGame.DataAccess;
using SnakeGame.ViewModels;

namespace SnakeGame;

public partial class LoginPage : ContentPage
{
    private readonly LoginViewModel _viewModel;
    private bool _isProcessing;

    // ������������� ������������ �� ������ LoginViewModel
    public LoginPage(LoginViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel; // ��������� ViewModel ��� ����������
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
            await ShowError("��������� ���������� ��� ���������� �� �����������.");
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

            // ���������� ������ Login �� ViewModel
            bool isSuccess = await _viewModel.Login(username, password);

            if (!isSuccess)
            {
                await ShowError("��������� ������������� ��� ��� ������");
                return;
            }

            // ���� ������� ����, ��������� ������� �� ����������� � ����������
            await SaveUserDataAndNavigate(username);
        }
        catch (Exception ex)
        {
            await ShowError("������ ��� ���� �� ����");
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
            // ��������� ������� �� ����������� � SecureStorage
            await SecureStorage.SetAsync("username", username);

            await DisplayAlert("�����", "����� ����� � ������!", "��");

            // �������� �������� �������� � MainPage
            Application.Current.MainPage = new NavigationPage(new MainPage());
        }
        catch (Exception ex)
        {
            await ShowError("������ ��� ��������� �� �������. ����, �������� ������.");
            // ��������� ������ �������� ����� ��� ������
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
            errorMessage = "����, �������� ������������� ���.";
            return false;
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            errorMessage = "����, �������� ������.";
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
        await DisplayAlert("������", message, "��");
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // ����������� ���� ���� ��� ������ ����������
        var existingUsername = await SecureStorage.GetAsync("username");
        if (!string.IsNullOrEmpty(existingUsername))
        {
            Application.Current.MainPage = new NavigationPage(new MainPage());
        }
    }
}
