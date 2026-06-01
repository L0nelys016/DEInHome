using Avalonia.Controls.Notifications;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DEInHome.Models;
using DEInHome.Views;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace DEInHome.ViewModels
{
    public partial class AuthViewModel : ViewModelBase
    {
        [ObservableProperty]
        private string? _loginValue;

        [ObservableProperty]
        private string? _passwordValue;

        public AuthViewModel() { }

        [RelayCommand]
        private async Task Auth()
        {
            if(string.IsNullOrEmpty(LoginValue) || string.IsNullOrEmpty(PasswordValue))
            {
                MainWindow.NotificationManager?.Show(new Notification("Предупреждение!", "Одно из полей не заполнено", NotificationType.Warning));
            }

            User? result = await _db.Users.FirstOrDefaultAsync(u => u.Login == LoginValue && u.Password == PasswordValue);

            if(result is null)
            {
                MainWindow.NotificationManager?.Show(new Notification("Ошибка!", "Неверный логин или пароль!", NotificationType.Error));
            }
            else
            {
                _loginedUser = result;
                MainWindowViewModel.Instance.CurrentViewModel = new ProductsViewModel();
            }
        }

        [RelayCommand]
        private async Task LogInGuest()
        {
            _loginedUser = null;
            MainWindowViewModel.Instance.CurrentViewModel = new ProductsViewModel();
        }
    }
}
