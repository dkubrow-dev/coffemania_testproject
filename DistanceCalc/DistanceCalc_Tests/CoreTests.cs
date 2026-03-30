using DistanceCalc.Abstractions;
using DistanceCalc.Models;
using DistanceCalc.Services;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace DistanceCalc_Tests;

/// <summary>
/// Экспресс объявление объекта микро-провайдера настроек для юнит-тестов
/// </summary>
/// <param name="Mode">Режим расчёта расстояний</param>
/// <param name="SimulateDelayMs">Режим задержки в расчётах</param>
sealed record SettingsProvider(CalculatorServiceModes Mode, int SimulateDelayMs) : ISettingsProvider;

/// <summary>
/// Тесты библиотеки калькулятора на её основной функционал
/// </summary>
public class CoreTests
{
    /// <summary>
    /// Путь для файла логгирования
    /// </summary>
    private static readonly string LogPath = Path.Combine(AppContext.BaseDirectory,
        "logs", $"tests-{DateTime.Now:yyyyMMdd-HHmmss}.log");

    /// <summary>
    /// объект, куда будет писаться дополнительно лог
    /// </summary>
    private readonly ITestOutputHelper _output;

    /// <summary>
    /// Конструктор запрашивает сервис вывода информации в тестовое окно
    /// </summary>
    /// <param name="output">объект, куда будет писаться дополнительно лог</param>
    public CoreTests(ITestOutputHelper output)
    {
        _output = output;
    }

    /// <summary>
    /// Базовые проверки на основной функционал
    /// </summary>
    [Fact]
    public async Task GeneralFunctionality()
    {
        // Калькулятор с настройками и логгированием
        ISettingsProvider settings = new SettingsProvider(CalculatorServiceModes.DirectLine, -1);
        using ILoggerFactory loggerFactory = new TestFileLoggerFactory(LogPath, _output);
        IDistanceCalculationService calculator = CalculatorsFactory.GetInstance(settings, loggerFactory);

        // Токен отмены
        using CancellationTokenSource cancellationTokenSource = new();
        CancellationToken cancellationToken = cancellationTokenSource.Token;

        // Проверка 1: Верные расчёты
        HashSet<(Input input, double expectedVaue)> testData = new()
        {
            {(new Input() {PointA = new(0, 0), PointB = new(3, 4)}, 5) },
            {(new Input() {PointA = new(1, 2), PointB = new(4, 6)}, 5) },
            {(new Input() {PointA = new(-1, -1), PointB = new(2, 3)}, 5) },
            {(new Input() {PointA = new(1, 1), PointB = new(1, 1)}, 0) },
            {(new Input() {PointA = new(1, 0), PointB = new(-1, 0)}, 2) }
        };

        foreach ((Input input, double expectedValue) in testData)
        {
            Result result = await calculator.CalculateAsync(input, cancellationToken);
            Assert.True(result.Success);
            Assert.Equal(expectedValue, result.Distance);
        }

        // Проверка 2: Неверный расчёт
        Input errInput = new() { PointA = new(0, 1), PointB = new(3, 4) };
        Result errResult = await calculator.CalculateAsync(errInput, cancellationToken);
        Assert.True(errResult.Success);
        Assert.NotEqual(5, errResult.Distance);
    }

    /// <summary>
    /// Проверяет работу токена с симуляцией медленного исполнения
    /// </summary>
    /// <remarks>Под виртуальной нагрузкой на объекте-исполнителе токен отмены должен успеть отменить задание</remarks>
    [Fact]
    public async Task CancellationToken()
    {
        // Калькулятор с настройками и логгированием
        ISettingsProvider settings = new SettingsProvider(CalculatorServiceModes.DirectLine, 1500);
        using ILoggerFactory loggerFactory = new TestFileLoggerFactory(LogPath, _output);
        IDistanceCalculationService calculator = CalculatorsFactory.GetInstance(settings, loggerFactory);

        // Токен отмены
        using CancellationTokenSource cancellationTokenSource = new();
        CancellationToken cancellationToken = cancellationTokenSource.Token;

        // проверяем работу токена 
        Input calcInput = new() { PointA = new(0, 0), PointB = new(0, 0) };
        Task<Result> task = calculator.CalculateAsync(calcInput, cancellationToken);
        cancellationTokenSource.Cancel();
        Result result = await task;

        Assert.False(result.Success);
        Assert.Equal(ErrorCodes.Canceled, result.ErrorCode);
    }

    /// <summary>
    /// Проверяет безошибочную работу в многопоточном режиме одного сервиса.
    /// Убеждается, что множество потоков не мешают друг другу, и все из них возвращаются с успехом
    /// </summary>
    [Fact]
    public async Task MultithreadLoad()
    {
        // Калькулятор
        ISettingsProvider settings = new SettingsProvider(CalculatorServiceModes.DirectLine, -1);
        using ILoggerFactory loggerFactory = new TestFileLoggerFactory(LogPath, _output);
        IDistanceCalculationService calculator = CalculatorsFactory.GetInstance(settings, loggerFactory);

        // Токен отмены
        using CancellationTokenSource cancellationTokenSource = new();

        const int loadCount = 1000;
        TimeSpan deadline = TimeSpan.FromSeconds(0.5);

        Task<Result>[] tasks = Enumerable.Range(0, loadCount)
            .Select(task =>
            {
                Input calcInput = new() { PointA = new(0, 0), PointB = new(0, 0) };
                return calculator.CalculateAsync(calcInput, cancellationTokenSource.Token);
            })
            .ToArray();

        Result[] results = await Task.WhenAll(tasks);

        Assert.Equal(loadCount, results.Length);
        Assert.All(results, r =>
        {
            Assert.True(r.Success);
            Assert.Equal(ErrorCodes.NoErrors, r.ErrorCode);
        });
    }
}
