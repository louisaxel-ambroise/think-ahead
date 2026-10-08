using ThinkAhead.Application.Data;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews(options =>
{
    options.OutputFormatters.Add(new HtmlViewFormatter());
    options.RespectBrowserAcceptHeader = true;
});
builder.Services.AddScoped<GameRepository>();

var app = builder.Build();

app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseExceptionHandler("/error");
app.MapControllers();
app.Run();
