using NumberToWords.Core.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<NumberToWordsConverter>();

var app = builder.Build();
app.MapDefaultControllerRoute();

app.Run();
