using Microsoft.AspNetCore.Authentication.Cookies;
using MudBlazor;
using MudBlazor.Services;
using NLog.Extensions.Logging;
using North.Common;
using North.Models.Auth;
using North.Services.Logger;
using ILogger = North.Services.Logger.ILogger;

class Program
{
    private static WebApplication? app;
    private static WebApplicationBuilder? builder;

    static void Main(string[] args)
    {
        try
        {
            // ��Ӧ�ó������¼�
            AppDomain.CurrentDomain.ProcessExit += OnProcessExit;
            AppDomain.CurrentDomain.UnhandledException += OnHandleExpection;

            // ��������
            builder = WebApplication.CreateBuilder(args);

            builder.Services.AddRazorPages();
            builder.Services.AddHttpClient();
            builder.Services.AddControllers();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddLogging(logger =>
            {
                logger.ClearProviders();
                logger.AddDebug();
                logger.AddEventSourceLogger();
                logger.AddNLog();
            });
            builder.Services.AddMudServices(config =>
            {
                // Snackbar ����
                config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.TopCenter;
                config.SnackbarConfiguration.PreventDuplicates = true;
                config.SnackbarConfiguration.SnackbarVariant = Variant.Text;
                config.SnackbarConfiguration.ShowCloseIcon = false;
                config.SnackbarConfiguration.VisibleStateDuration = 1500;
                config.SnackbarConfiguration.HideTransitionDuration = 200;
                config.SnackbarConfiguration.ShowTransitionDuration = 200;
            });
            builder.Services.AddSingleton(identifyes => new List<UnitLoginIdentify>());
            builder.Services.AddSingleton<ILogger, NLogger>(logger => new NLogger(GlobalValues.AppSettings.Log));
            builder.Services.AddServerSideBlazor(option =>
            {
                option.DetailedErrors = false;
            });
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                            .AddCookie(options =>
                            {
                                options.ExpireTimeSpan = TimeSpan.FromDays(3);
                            });

            // ���� web Ӧ��
            app = builder.Build();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseStaticFiles();
            app.MapBlazorHub();
            app.MapControllers();
            app.MapFallbackToPage("/_Host");
            app.Urls.Add("http://0.0.0.0:12121");

            app.Run();
        }
        catch(Exception e)
        {
            app?.Services.GetService<ILogger>()?.Error("Application abort", e);
        }
    }


    /// <summary>
    /// ����δ��������쳣
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    static void OnHandleExpection(object sender, UnhandledExceptionEventArgs args)
    {
        using var logger = new KLogger(GlobalValues.AppSettings.Log);
        logger.Error($"Unhandled expection, {args}");
    }


    /// <summary>
    /// ����Ӧ���˳��¼�
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    static void OnProcessExit(object? sender, EventArgs args)
    {
        new KLogger(GlobalValues.AppSettings.Log).Info("Application exit");
    }
}