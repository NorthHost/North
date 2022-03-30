using ImageBed.Data;
using Tewr.Blazor.FileReader;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

var builder = WebApplication.CreateBuilder(args);


/// <summary>
/// ��������ע�����
/// </summary>
builder.Services.AddAntDesign();
builder.Services.AddControllers();
builder.Services.AddRazorPages();
builder.Services.AddHttpClient();
builder.Services.AddServerSideBlazor();
builder.Services.AddFileReaderService();


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