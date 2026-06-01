using CommunityToolkit.Mvvm.ComponentModel;

namespace DEInHome.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        [ObservableProperty]
        private ViewModelBase _currentViewModel = new AuthViewModel();

        public static MainWindowViewModel Instance = null!;

        public MainWindowViewModel()
        {
            Instance = this;
        }
    }
}
