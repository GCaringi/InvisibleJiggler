using InvisibleJiggler.Options;
using InvisibleJiggler.Services;
using InvisibleJiggler.Windows.Api;
using InvisibleJiggler.Windows.Api.Interface;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSingleton<IWindowsApiService, WindowsApiService>();
builder.Services.AddHostedService<MouseJigglerService>();

builder.Services.Configure<JigglerOptions>
(
    builder.Configuration.GetSection("JigglerOptions")
);

var host = builder.Build();
await host.RunAsync();

