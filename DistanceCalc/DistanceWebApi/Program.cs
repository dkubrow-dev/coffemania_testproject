using DistanceCalc.Abstractions;
using DistanceCalc.Services;
using Microsoft.Extensions.Options;
using DistanceWebApi.Services;
using NLog.Web;
using DistanceWebApi.Middleware;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// NLog будет единым провайдером на всё
builder.Logging.ClearProviders();
builder.Host.UseNLog();

// развёртывание в Windows и Linux
builder.Services.AddWindowsService();
builder.Services.AddSystemd();

// Читаем настройки, добавляем сервис калькулятора
builder.Services.Configure<CalculatorSettingsProvider>(builder.Configuration.GetRequiredSection(CalculatorSettingsProvider.SectionName));
builder.Services.AddSingleton<ISettingsProvider>(provider => provider.GetRequiredService<IOptions<CalculatorSettingsProvider>>().Value);
builder.Services.AddSingleton<IDistanceCalculationService>(provider =>
{
    ISettingsProvider settings = provider.GetRequiredService<ISettingsProvider>();
    ILoggerFactory loggerFactory = provider.GetRequiredService<ILoggerFactory>();
    return CalculatorsFactory.GetInstance(settings, loggerFactory);
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

WebApplication app = builder.Build();

// Только для отладки, в проде не показываем
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Логирование HTTP-запросов/ответов
app.UseMiddleware<RequestLoggingMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();