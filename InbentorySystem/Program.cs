using Dapper;
using InbentorySystem.Data;
using InbentorySystem.Infrastructure.Interfaces;
using InbentorySystem.Services;
using InbentorySystem.Services.Interfaces;
using InbentorySystem.Ui;

// Webアプリケーションを構築するためのホストビルダーを作成
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorComponents().AddInteractiveServerComponents();
DefaultTypeMap.MatchNamesWithUnderscores = true;

// データベース接続文字列を読み込む
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("データベース接続文字列'DefaultConnection'が設定ファイルに見つかりません。");
}

// IDbConnectionFactory の実装を登録
builder.Services.AddSingleton<IDbConnectionFactory>(
    sp => new NpgsqlConnectionFactory(connectionString));

// DIコンテナへ登録
builder.Services.AddScoped<IShohinRepository, ShohinRepository>();
builder.Services.AddScoped<IShohinService, ShohinService>();
builder.Services.AddScoped<IShiireRepository, ShiireRepository>();
builder.Services.AddScoped<IShiireService, ShiireService>();
builder.Services.AddScoped<ISqlExecutor, SqlExecutor>();
builder.Services.AddScoped<IZaikoService, ZaikoService>();

var app = builder.Build();

// HTTPリクエストパイプラインの設定
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();
app.Run();

namespace InbentorySystem
{
    public partial class Program { }
}