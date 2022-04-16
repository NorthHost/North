using ImageBed.Common;
using ImageBed.Data.Access;
using ImageBed.Data.Entity;
using NLog.Extensions.Logging;


var builder = WebApplication.CreateBuilder(args);


/// <summary>
/// ���ر������ã�����ϵͳ��Դ��¼��ʱ��
/// </summary>
GlobalValues.appSetting = AppSetting.Parse();
GlobalValues.InitSysRecordTimer();


/// <summary>
/// ��������ע�����
/// </summary>
builder.Services.AddAntDesign();
builder.Services.AddControllers();
builder.Services.AddRazorPages();
builder.Services.AddHttpClient();
builder.Services.AddServerSideBlazor();
builder.Services.AddDbContext<OurDbContext>();
builder.Services.AddLogging(logger =>
{
    logger.ClearProviders();
    logger.AddConsole();
    logger.AddDebug();
    logger.AddEventSourceLogger();
    logger.AddNLog();
});

var app = builder.Build();

app.Urls.Add("http://0.0.0.0:12121");
app.UseStaticFiles();


/// <summary>
/// ����·��
/// </summary>
app.UseRouting();
app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();