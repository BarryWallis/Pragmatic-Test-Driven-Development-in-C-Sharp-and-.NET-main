using AdamTibi.OpenWeather;

using Uqs.Weather;
using Uqs.Weather.Wrappers;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<IClient>(_ =>
{
    bool isLoad = bool.Parse(builder.Configuration["LoadTest:IsActive"]
                             ?? throw new KeyNotFoundException("LoadTest:IsActive value not found."));
    if (isLoad)
    {
        return new ClientStub();
    }
    else
    {
        string apiKey = builder.Configuration["OpenWeather:Key"]
                        ?? throw new KeyNotFoundException("Could not find OpenWeather:Key");
        HttpClient httpClient = new();
        return new Client(apiKey, httpClient);
    }
});
builder.Services.AddSingleton<INowWrapper>(_ => new NowWrapper());
builder.Services.AddTransient<IRandomWrapper>(_ => new RandomWrapper());

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    _ = app.UseSwagger();
    _ = app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
