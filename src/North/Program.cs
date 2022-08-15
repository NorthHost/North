using Microsoft.AspNetCore.Authentication.Cookies;
using MudBlazor;
using MudBlazor.Services;
using NLog.Extensions.Logging;
using North.Common;
using North.Core.Data.Access;
using North.Services.Logger;
using ILogger = North.Services.Logger.ILogger;

class Program
{
    private static WebApplication? app;
    private static WebApplicationBuilder? builder;
    private static readonly ILogger logger = new NLogger(GlobalValues.AppSettings.Log);

    static void Main(string[] args)
    {
        try
        {
            logger.Info("Application launching...");

            // ��Ӧ�ó������¼�
            logger.Info("AppDomain event binding...");
            AppDomain.CurrentDomain.ProcessExit += OnProcessExit;
            AppDomain.CurrentDomain.UnhandledException += OnHandleExpection;
            logger.Info("AppDomain event bind success");

            // ��������
            builder = WebApplication.CreateBuilder(args);

            logger.Info("Application building...");
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
            builder.Services.AddScoped(context => new OurDbContext(GlobalValues.AppSettings.Storage.DataBase.ConnStr));
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
            logger.Info("Application build success");

            // ���� web Ӧ��
            app = builder.Build();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseStaticFiles();
            app.MapBlazorHub();
            app.MapControllers();
            app.MapFallbackToPage("/_Host");
            // TODO �޸İ󶨵� URL
            app.Urls.Add("http://0.0.0.0:12121");

            logger.Info("Application launch success");

            app.Run();
        }
        catch(Exception e)
        {
            logger.Error("Application abort", e);
        }
    }


    /// <summary>
    /// ����δ��������쳣
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    static void OnHandleExpection(object sender, UnhandledExceptionEventArgs args)
    {
        
    }


    /// <summary>
    /// ����Ӧ���˳��¼�
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    static void OnProcessExit(object? sender, EventArgs args)
    {
        // �˴�����ע��ķ�����ᱻ�ͷţ�����޷���ȡ ILogger ʵ����ӡ��־
        // ���´��� NLogger ʵ��Ҳ�޷�ʹ�ã���Ϊ���Ǵ� LogManager ��ȡ��
        // KLogger ���� FileStream ʵ�֣�����Ӱ�� 
        using var logger = new KLogger(GlobalValues.AppSettings.Log);

        // ͬ���������ݿ�
        logger.Info("Database syncing...");
        GlobalValues.MemoryDatabase.SyncDatabase(GlobalValues.AppSettings.Storage.DataBase.ConnStr);
        logger.Info("Database sync success");

        logger.Info("Application exit");
    }
}