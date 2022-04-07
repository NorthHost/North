using ImageBed.Data.Access;
using ImageBed.Common;
using ImageBed.Data.Entity;

var builder = WebApplication.CreateBuilder(args);


/// <summary>
/// ���ر�������
/// </summary>
GlobalValues.appSetting = AppSetting.Parse();


/// <summary>
/// ��������ע�����
/// </summary>
builder.Services.AddAntDesign();
builder.Services.AddControllers();
builder.Services.AddRazorPages();
builder.Services.AddHttpClient();
builder.Services.AddServerSideBlazor();
builder.Services.AddDbContext<OurDbContext>();


var app = builder.Build();


app.UseHttpsRedirection();
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