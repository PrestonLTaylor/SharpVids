using SharpVids.Data;
using SharpVids.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DatabaseContext>();
builder.Services.AddDefaultIdentity<UserModel>().AddEntityFrameworkStores<DatabaseContext>();

builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	await app.SeedDatabaseAsync();
}
else
{
	app.UseExceptionHandler("/Home/Error");
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
