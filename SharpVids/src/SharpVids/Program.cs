using SharpVids.Components;
using SharpVids.Data;
using SharpVids.Options;
using SharpVids.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddRawVideoDbServices();
builder.Services.AddSharpVidsServices();

builder.Services.Configure<UploadOptions>(builder.Configuration.GetSection(UploadOptions.Section));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(SharpVids.Client._Imports).Assembly);

app.Run();
