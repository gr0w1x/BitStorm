using Blazored.LocalStorage;
using Fluxor;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using WebClient;
using WebClient.Services;
using WebClient.Store.User;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddFluxor(options =>
    options
        .AddMiddleware<UserMiddleware>()
        .ScanAssemblies(typeof(Program).Assembly)
        .UseReduxDevTools()
);

builder.Services.AddScoped<UserMiddleware>();

builder.Services.AddBlazoredLocalStorage();

builder.Services.AddScoped<ApiClient>(sp =>
    new ApiClient(sp.GetRequiredService<IState<UserState>>(), sp.GetRequiredService<IDispatcher>())
    {
        BaseAddress = new Uri(builder.Configuration["ApiUri"]!)
    }
);

builder.Services.AddSingleton<Navigation>();

builder.Services.AddScoped<AuthService>();

var app = builder.Build();

await app.RunAsync();
