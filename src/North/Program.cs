using Microsoft.AspNetCore.Authentication.Cookies;
using MudBlazor;
using MudBlazor.Services;
using North.Data.Access;
using North.Models.Auth;
using North.Services.Storage;

class Program
{
    public static void Main(string[] args)
    {
        // ����������ע�����
        var builder = WebApplication.CreateBuilder(args);

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
        builder.Services.AddControllers();
        builder.Services.AddRazorPages();
        builder.Services.AddHttpClient();
        builder.Services.AddServerSideBlazor(option =>
        {
            option.DetailedErrors = false;
        });
        builder.Services.AddDbContext<OurDbContext>();
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                        .AddCookie(options =>
                        {
                            options.ExpireTimeSpan = TimeSpan.FromDays(3);
                        });
        builder.Services.AddSingleton<IStorage<UnitLoginIdentify>, MemoryStorage<UnitLoginIdentify>>(identifies => new MemoryStorage<UnitLoginIdentify>());

        // ����Ӧ��
        var app = builder.Build();

        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseStaticFiles();
        app.MapBlazorHub();
        app.MapControllers();
        app.MapFallbackToPage("/_Host");
        // TODO �޸İ󶨵� URL
        app.Urls.Add("http://0.0.0.0:12122");

        app.Run();
    }
}