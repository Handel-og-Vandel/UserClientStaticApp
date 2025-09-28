using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Options;
using UserClient;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

using var fetchClient = new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) };
try
{
    using var response = await fetchClient.GetAsync("config.json", HttpCompletionOption.ResponseHeadersRead);
    if (response.IsSuccessStatusCode)
    {
        using var stream = await response.Content.ReadAsStreamAsync();
        builder.Configuration.AddJsonStream(stream);
    }
    else
    {
        Console.WriteLine($"config.json not found or not OK (status {response.StatusCode}). Proceeding without it.");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Failed to load config.json: {ex.Message}");
}

// Bind a strongly-typed options object
builder.Services.Configure<RuntimeConfig>(builder.Configuration.GetSection("Runtime"));

// HttpClient will use the runtime ApiBase if present; otherwise fallback to app base
builder.Services.AddScoped(sp =>
{
    var cfg = sp.GetRequiredService<IOptions<RuntimeConfig>>().Value;
    var baseUrl = !string.IsNullOrWhiteSpace(cfg.ClientServiceApiBase)
        ? cfg.ClientServiceApiBase
        : builder.HostEnvironment.BaseAddress;

    return new HttpClient { BaseAddress = new Uri(baseUrl) };
});

builder.Services.AddHttpClient("RemoteUserService", client =>
{
    var cfg = builder.Configuration.GetSection("Runtime").Get<RuntimeConfig>();
    client.BaseAddress = new Uri(cfg.ClientServiceApiBase ?? builder.HostEnvironment.BaseAddress); 
});

await builder.Build().RunAsync();


// Strongly-typed runtime config
public sealed class RuntimeConfig
{
    public string? ClientServiceApiBase { get; set; }
    public string? Environment { get; set; }
}