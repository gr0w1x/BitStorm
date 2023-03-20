using Blazored.LocalStorage;
using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;
using Fluxor;
using Markdig;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using WebClient;
using WebClient.Constants.CodeThemes;
using WebClient.Services;
using WebClient.Store.Theme;
using WebClient.Store.User;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddFluxor(options =>
    options
        .AddMiddleware<UserMiddleware>()
        .AddMiddleware<ThemeMiddleware>()
        .ScanAssemblies(typeof(Program).Assembly)
        .UseReduxDevTools()
);

builder.Services
    .AddBlazorise(options => options.Immediate = true)
    .AddBootstrapProviders()
    .AddFontAwesomeIcons();

builder.Services.AddScoped<UserMiddleware>();

builder.Services.AddBlazoredLocalStorage();

builder.Services.AddScoped<ApiClient>(sp =>
    new ApiClient(sp.GetRequiredService<IState<UserState>>(), sp.GetRequiredService<IDispatcher>())
    {
        BaseAddress = new Uri(builder.Configuration["ApiUri"]!)
    }
);

builder.Services.AddSingleton<Navigation>();

builder.Services.AddSingleton<CodeThemes>();

builder.Services.AddSingleton(
    new MarkdownPipelineBuilder()
        .UseAdvancedExtensions()
        .Build()
);

builder.Services.AddScoped<AuthService>();

builder.Services.AddScoped<UsersService>();
builder.Services.AddScoped<TasksService>();

var app = builder.Build();

await app.RunAsync();
