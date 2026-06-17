using Microsoft.Extensions.Logging;
using C971project.Services;
using C971project.ViewModels;
using C971project.Views;
using Plugin.LocalNotification;

namespace C971project
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseLocalNotification()
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif
            builder.Services.AddSingleton<DatabaseService>();

            builder.Services.AddTransient<TermListViewModel>();
            builder.Services.AddTransient<TermListPage>();

            builder.Services.AddTransient<TermEditViewModel>();
            builder.Services.AddTransient<TermEditPage>();

            builder.Services.AddTransient<TermDetailViewModel>();
            builder.Services.AddTransient<TermDetailPage>();

            builder.Services.AddTransient<CourseEditViewModel>();
            builder.Services.AddTransient<CourseEditPage>();
            builder.Services.AddSingleton<NotificationPermissionService>();
            builder.Services.AddTransient<AssessmentListViewModel>();
            builder.Services.AddTransient<AssessmentListPage>();

            builder.Services.AddTransient<AssessmentEditViewModel>();
            builder.Services.AddTransient<AssessmentEditPage>();
            builder.Services.AddSingleton<AuthService>();
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<SearchPage>();
            builder.Services.AddTransient<ReportsPage>();

            return builder.Build();
        }
    }
}
