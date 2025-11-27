using fs_2025_assessment_1_74491.BlazorApp.Components;
using fs_2025_assessment_1_74491.BlazorApp.Services;

var builder = WebApplication.CreateBuilder(args);

var apiBaseUrl = builder.Configuration["Api:BaseUrl"];

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Typed HttpClient for calling the Dublin Bikes API (V2)
builder.Services.AddHttpClient<StationsApiClient>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl!);
});

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
