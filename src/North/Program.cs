using Microsoft.AspNetCore.Authentication.Cookies;
using MudBlazor;
using MudBlazor.Services;
using NLog.Extensions.Logging;
using North.Common;
using North.Controllers;
using North.Core.Helpers;
using North.Core.Models.Auth;
using North.Core.Services.Logger;
using North.Core.Services.Poster;
using North.Data.Access;
using ILogger = North.Core.Services.Logger.ILogger;

class Program
{
    private static WebApplication? app;
    private static WebApplicationBuilder? builder;
    private static readonly KLogger logger = new(GlobalValues.AppSettings.Log);

    public static void Main(string[] args)
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
            builder.Services.AddDbContext<OurDbContext>();
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
                // ��δ������쳣����ʱ���Ƿ�����ϸ�Ĵ�����Ϣ�� Javascript
                // ��������Ӧ���رգ�������ܱ�©������Ϣ
                option.DetailedErrors = false;
                // JS �����ó�ʱ�¼�����
                option.JSInteropDefaultCallTimeout = TimeSpan.FromSeconds(10);
            });
            builder.Services.AddAuthentication(options =>
                            {
                                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                                options.RequireAuthenticatedSignIn = false;
                            })
                            .AddCookie(options =>
                            {
                                // ȷ�����ڴ��� Cookie ������
                                options.Cookie = new CookieBuilder()
                                {
                                    // SameSite �����û����Ƶ����� Cookie�����ٰ�ȫ���գ���������ֵ���ο���http://www.ruanyifeng.com/blog/2019/09/cookie-samesite.html��
                                    // Strict: ��ȫ��ֹ������ Cookie����վ�㣨����������������ͬ��ʱ���ᷢ�� Cookie
                                    // Lax: ��������Ҳ�ǲ����͵����� Cookie�����ǵ�����Ŀ����ַ�� Get �������
                                    // None: ��ʽ�ر� SameSite ���ԣ�������ͬʱ���� Secure ���ԣ�Cookie ֻ��ͨ�� HTTPS Э�鷢�ͣ�
                                    SameSite = SameSiteMode.Lax,
                                    // ����Ϊ false ����ֹ JS ���֡��޸� Cookie
                                    HttpOnly = true,
                                    // �����ṩ Cookie �� URI �����ͣ�HTTP/HTTPS��������������ʱ������ʱЯ�� Cookie����������ֵ
                                    // Always: Ҫ���¼ҳ��֮��������Ҫ�����֤��ҳ���Ϊ HTTPS
                                    // None: ��¼ҳΪ HTTPS�������� HTTP ҳҲ��Ҫ�����֤��Ϣ
                                    // SameAsRequest: ���ṩ Cookie �� URI Ϊ HTTPS����ֻ���ں��� HTTPS �����Ͻ� Cookie ���ط����������ṩ Cookie �� URI Ϊ HTTP������ں��� HTTP �� HTTPS �����Ͻ� Cookie ���ط�������
                                    SecurePolicy = CookieSecurePolicy.SameAsRequest,
                                };
                                // ���� Cookie �����֤�ڼ䷢�����¼�
                                options.Events = new CookieAuthenticationEvents
                                {
                                    // ��¼��ɺ����
                                    // ��ʱʵ���ϲ�û����� Cookie д�룬����޷�ͨ�� context.HttpContext.User.Claims ��ȡ�û���Ϣ
                                    OnSignedIn = (context) =>
                                    {
                                        var user = app?.Services
                                                      ?.GetService<List<UnitLoginIdentify>>()
                                                      ?.FirstOrDefault(i => i.Id == context.Request.Query.First(q => q.Key == "id").Value)
                                                      ?.ClaimsIdentity.GetUserClaimEntity();
                                        if(user is not null)
                                        {
                                            logger.Info($"{user.Role} {user.Email} login");
                                        }
                                        return Task.CompletedTask;
                                    },
                                    // ע��ʱ����
                                    OnSigningOut = (context) =>
                                    {
                                        var user = context.HttpContext.User.Identities.First().GetUserClaimEntity();
                                        logger.Info($"{user.Role} {user.Email} logout");
                                        return Task.CompletedTask;
                                    }
                                };
                            });
            builder.Services.AddSingleton<IPoster, MineKitPoster>(poster => new MineKitPoster());

            // ���� web Ӧ��
            app = builder.Build();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseStaticFiles();
            app.MapBlazorHub();
            app.MapControllers();
            app.MapFallbackToPage("/_Host");
            app.Urls.Add("http://*:12121");

            app.Run();
        }
        catch(Exception e)
        {
            logger.Error($"Application abort", e);
        }
    }


    /// <summary>
    /// ����δ��������쳣
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    private static void OnHandleExpection(object sender, UnhandledExceptionEventArgs args)
    {
        logger.Error($"Unhandled expection, {args}");
    }


    /// <summary>
    /// Ӧ���˳�ʱ����
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    private static void OnProcessExit(object? sender, EventArgs args)
    {
        logger.Info("Application exit");
        logger.Dispose();
    }
}