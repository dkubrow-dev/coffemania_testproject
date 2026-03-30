using DistanceCalc.Abstractions;
using DistanceCalc.Services;
using Microsoft.Extensions.Options;
using DistanceWebApi.Services;

var builder = WebApplication.CreateBuilder(args);

// развёртывание в Windows и Linux
builder.Services.AddWindowsService();
builder.Services.AddSystemd();

// Читаем настройки, добавляем сервис калькулятора
builder.Services.Configure<CalculatorSettingsProvider>(builder.Configuration.GetRequiredSection(CalculatorSettingsProvider.SectionName));
builder.Services.AddSingleton<ISettingsProvider>(provider => provider.GetRequiredService<IOptions<CalculatorSettingsProvider>>().Value);
builder.Services.AddSingleton(provider =>
{
    var settings = provider.GetRequiredService<ISettingsProvider>();
    return CalculatorsFactory.GetInstance(settings);
});

// Добавляем контроллеры API
builder.Services.AddControllers();

// добавляем OpenAPI и вывод из xml summary разметки
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    string baseDir = AppContext.BaseDirectory;
    options.IncludeXmlComments(Path.Combine(baseDir, "xml-summary", "distanceCalc-WebApi.xml"));
    options.IncludeXmlComments(Path.Combine(baseDir, "xml-summary", "distanceCalc.xml"));
});

var app = builder.Build();

// Только для отладки, в проде не показываем
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();