using DocumentFormat.OpenXml.Wordprocessing;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SnakeGame.ViewModels
{
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        private bool _isBusy;
        private string? _title;
        private string? _errorMessage;

        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        public string Title
        {
            get => _title ?? throw new ArgumentNullException(nameof(_title));
            set => SetProperty(ref _title, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage ?? throw new ArgumentNullException(nameof(_errorMessage));
            set => SetProperty(ref _errorMessage, value);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
            {
                return false;
            }

            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        protected virtual Task ShowError(string message)
        {
            ErrorMessage = message;
            return Task.CompletedTask;
        }

        protected void ResetError()
        {
            ErrorMessage = string.Empty;
        }

        protected async Task ExecuteAsync(Func<Task> operation)
        {
            try
            {
                IsBusy = true;
                ResetError();
                await operation();
            }
            catch (Exception ex)
            {
                await ShowError(ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }

        protected async Task<T> ExecuteAsync<T>(Func<Task<T>> operation, T? defaultValue = default)
        {
            try
            {
                IsBusy = true;
                ResetError();
                return await operation();
            }
            catch (Exception ex)
            {
                await ShowError(ex.Message);
                return defaultValue ?? throw new ArgumentNullException(nameof(defaultValue));
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}