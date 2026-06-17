using C971project.Services;
using C971project.Views;

namespace C971project
{
    public partial class App : Application
    {
        public App(NotificationPermissionService permissionService, LoginPage loginPage)
        {
            InitializeComponent();

            _ = permissionService.RequestAsync();

            MainPage = loginPage;
        }
    }
}