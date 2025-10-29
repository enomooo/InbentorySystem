using Npgsql;
using InbentorySystem.Components;
// ShohinRepositoryの名前空間
using InbentorySystem.Data;

// Webアプリケーションを構築するためのホストビルダーを作成
var builder = WebApplication.CreateBuilder(args);

// BlazorコンポーネントのサービスをDIコンテナに登録（Blazor Web Appを使うテンプレ）
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// ShohinRepositoryをDIコンテナへ登録
builder.Services.AddScoped<IShohinRepository, ShohinRepository>();

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
