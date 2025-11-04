using Dapper;
using InbentorySystem.Components;
// ShohinRepositoryの名前空間
using InbentorySystem.Data;
using InbentorySystem.Services;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Data;




// Webアプリケーションを構築するためのホストビルダーを作成
var builder = WebApplication.CreateBuilder(args);

// BlazorコンポーネントのサービスをDIコンテナに登録（Blazor Web Appを使うテンプレ）
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Dapperの設定: C#のPascalCaseプロパティをPostgreSQLのsnake_case列名に自動マッピングする
DefaultTypeMap.MatchNamesWithUnderscores = true;

// データベース接続文字列を読み込む
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("データベース接続文字列'DefaultConnection'が設定ファイルに見つかりません。");
}

// IDbConnectionFactory の実装を登録
// NpgsqlConnectionFactory は、IDbConnectionFactory を実装し、
// コンストラクタで接続文字列（connectionString）を受け取ることを想定しています。
builder.Services.AddSingleton<IDbConnectionFactory>(
    sp => new NpgsqlConnectionFactory(connectionString));

// ISqlExecutor の実装を登録
// 例: SqlExecutor クラス
builder.Services.AddScoped<ISqlExecutor, SqlExecutor>();

// ShohinRepositoryをDIコンテナへ登録
builder.Services.AddScoped<IShohinRepository, ShohinRepository>();

// Program.cs の DI 登録セクション

// ... (以前の登録) ...
// ShohinRepositoryをDIコンテナへ登録
builder.Services.AddScoped<IShohinRepository, ShohinRepository>();

// ShohinService
builder.Services.AddScoped<IShohinService, ShohinService>();

// 仕入RepositoryをDIコンテナへ登録
builder.Services.AddScoped<IShiireRepository, ShiireRepository>();

builder.Services.AddScoped<ISqlExecutor, SqlExecutor>();

builder.Services.AddScoped<IShiireService, ShiireService>();

// ... (続くコード) ...

// アプリケーションインスタンスの構築
var app = builder.Build();

// HTTPリクエストパイプラインの設定
// もし開発環境以外の場合はエラーハンドラー
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);

    // Hsts(HTTP Strict Transport Security)を使用し、ブラウザにHTTPS通信を強制
    app.UseHsts();
}

// HTTPリクエストをHTTPSにリダイレクト
app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
namespace InbentorySystem // ★★★ ここはあなたのメインプロジェクトの名前空間に合わせること ★★★
{
    public partial class Program { }
}