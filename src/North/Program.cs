using MudBlazor.Services;

class Program
{
    public static void Main(string[] args)
    {
        // ����������ע�����
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddRazorPages();
        builder.Services.AddMudServices();
        builder.Services.AddServerSideBlazor();

        // ����Ӧ��
        var app = builder.Build();

        app.UseRouting();
        app.UseStaticFiles();
        
        app.MapBlazorHub();
        app.MapFallbackToPage("/_Host");
        app.UseExceptionHandler("/Error");
        app.Urls.Add("http://0.0.0.0:12121");

        app.Run();
    }
}