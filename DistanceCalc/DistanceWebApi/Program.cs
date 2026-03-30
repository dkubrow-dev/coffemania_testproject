using DistanceCalc.Services;

var builder = WebApplication.CreateBuilder(args);

// добавим в контейнер сервис калькуляции
//   Сейчас достаточно синглтона - он умеет рабоать в многопоточном режиме,
//   к тому же он вызывается 1 раз на вызов контроллера: затраты на создание объекта через фабрику не окупятся
builder.Services.AddSingleton(_ => CalculatorsFactory.GetInstance());
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