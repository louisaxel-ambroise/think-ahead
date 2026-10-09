using Sqids;
using ThinkAhead.Application.Data;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews(options =>
{
    options.OutputFormatters.Add(new HtmlViewFormatter());
    options.RespectBrowserAcceptHeader = true;
});
builder.Services.AddScoped(_ => new GameRepository("Data Source=Chess.db"));
builder.Services.AddSingleton(new SqidsEncoder<int>(new() { MinLength = 7, Alphabet = "5Np2JXvREImi74geBw8jha1QKtbD3VAGHkqZn9zdxuo0MlyTsrOUfcFYSPW6LC" }));

var app = builder.Build();

app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseExceptionHandler("/error");
app.MapControllers();
app.Run();
