using Npgsql;
using InbentorySystem.Components;
using InbentorySystem.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// appsettings.jsonからPostgreSQLの接続文字列を取得
var connectionString = builder.Configuration.GetConnectionString("PostgreConnection");

// NpgsqlConnectionをDIコンテナに登録
// アプリケーション内でDB接続が必要なときにインスタンスが提供される
builder.Services.AddScoped<NpgsqlConnection>(_ =>
    new NpgsqlConnection(connectionString));

builder.Services.AddScoped<ShohinRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
