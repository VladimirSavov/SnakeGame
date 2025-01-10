using SnakeGame.Data.Models;
using SnakeGame.DataAccess;
using SnakeGame.ViewModels;
using System.Text.RegularExpressions;

namespace SnakeGame;

public partial class RegisterPage : ContentPage
{
    private readonly RegisterViewModel _viewModel;
    private readonly Regex _emailRegex = new(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$");

    // ����������� � RegisterViewModel
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
            await DisplayAlert("������", "��������� ������� ��� ���������� �� ����.", "��");
        }
    }

    private async void OnRegisterBtnClicked(object sender, EventArgs e)
    {
        try
        {
            if (!ValidateInputs(out string errorMessage))
            {
                await DisplayAlert("������", errorMessage, "��");
                return;
            }

            var username = UsernameEntry.Text.Trim();
            var email = EmailEntry.Text.Trim();
            var password = PasswordEntry.Text;
            var confirmPassword = ConfirmPasswordEntry.Text;

            // ��������� ������ �� ViewModel �� �����������
            bool isSuccess = await _viewModel.Register(username, email, password, confirmPassword);

            if (!isSuccess)
            {
                await DisplayAlert("������", "�������� ������ ��� �������������. ����, �������� ������.", "��");
                return;
            }

            // ���� ������� �����������, ������� ��� LoginPage
            await DisplayAlert("�����", "������� �����������!", "��");
            await Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("������", "�������� ������ ��� �������������. ����, �������� ������.", "��");
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
            errorMessage = "������ ������ �� ������������!";
            return false;
        }

        if (username.Length < 3)
        {
            errorMessage = "��������������� ��� ������ �� � ���� 3 �������.";
            return false;
        }

        if (!_emailRegex.IsMatch(email))
        {
            errorMessage = "��������� ����� �����.";
            return false;
        }

        if (password.Length < 6)
        {
            errorMessage = "�������� ������ �� � ���� 6 �������.";
            return false;
        }

        if (password != confirmPassword)
        {
            errorMessage = "�������� �� ��������.";
            return false;
        }

        return true;
    }
}
