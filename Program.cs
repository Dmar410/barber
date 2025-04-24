var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddControllers(); // Ensure this line exists

var app = builder.Build();

app.UseRouting();
app.UseAuthorization();
app.MapControllers(); // Ensure this is here
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
