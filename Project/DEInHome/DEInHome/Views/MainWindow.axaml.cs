using Avalonia.Controls;
using Avalonia.Controls.Notifications;

namespace DEInHome.Views
{
    public partial class MainWindow : Window
    {
        public static INotificationManager? NotificationManager;

        public MainWindow()
        {
            InitializeComponent();

            NotificationManager = new WindowNotificationManager(this)
            {
                Position = NotificationPosition.TopRight,
                MaxItems = 3
            };
        }
    }
}