using AISample.Components;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using OllamaSharp;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Register the chat client with middleware
builder.Services.AddChatClient(services =>
{    
    return ((IChatClient)new OllamaApiClient(
        "http://localhost:11434", "llava"))
        .AsBuilder()
        .UseDistributedCache(new MemoryDistributedCache(
            Options.Create(new MemoryDistributedCacheOptions())))
        .Build();
});
builder.Services.AddEmbeddingGenerator<string, Embedding<float>>(services =>
{
    return new OllamaApiClient(new Uri("http://localhost:11434"), "all-minilm");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
