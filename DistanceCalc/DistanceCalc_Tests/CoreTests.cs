using DistanceCalc.Abcstractions;
using DistanceCalc.Models;
using DistanceCalc.Services;

namespace DistanceCalc_Tests;

/// <summary>
/// Тесты библиотеки калькулятора на её основной функционал
/// </summary>
public class CoreTests
{
    /// <summary>
    /// Базовые проверки на основной функционал
    /// </summary>
    [Fact]
    public async Task GeneralFunctionalityIsOkAsync()
    {
        // Калькулятор
        IDistanceCalculationService calculator = CalculatorsFactory.GetInstance();

        // Токен отмены
        using CancellationTokenSource cancellationTokenSource = new();
        CancellationToken cancellationToken = cancellationTokenSource.Token;

        // Проверка 1: правильный расчёт египетского треугольника
        Input calcInput = new() { PointA = new(0, 0), PointB = new(3, 4) };
        Result result = await calculator.CalculateAsync(calcInput, cancellationToken);
        Assert.True(result.Success);
        Assert.Equal(5, result.Distance);

        // Проверка 2: неверный расчёт египетского треугольника
        calcInput = new() { PointA = new(0, 1), PointB = new(3, 4) };
        result = await calculator.CalculateAsync(calcInput, cancellationToken);
        Assert.True(result.Success);
        Assert.NotEqual(5, result.Distance);
    }

    /// <summary>
    /// Проверяет работу токена.
    /// </summary>
    /// <remarks>Под виртуальной нагрузкой на объекте-исполнителе токен отмены должен успеть отменить задание</remarks>
    [Fact]
    public async Task CancelationIsOkAsync()
    {
        // Калькулятор
        IDistanceCalculationService calculator = CalculatorsFactory.GetInstance();

        // Токен отмены
        using CancellationTokenSource cancellationTokenSource = new();
        CancellationToken cancellationToken = cancellationTokenSource.Token;
        Input calcInput = new() { PointA = new(0, 0), PointB = new(0, 0) };

        // проверяем работу токена 
        Task<Result> task = calculator.CalculateAsync(calcInput, cancellationToken);
        cancellationTokenSource.Cancel();
        Result result = await task;

        Assert.False(result.Success);
        Assert.IsType<TaskCanceledException>(result.Exception);
    }

    /// <summary>
    /// Проверяет безошибочную работу в многопоточном режиме одного сервиса.
    /// Убеждается, что множество потоков не мешают друг другу, и все из них возвращаются с успехом
    /// </summary>
    [Fact]
    public async Task MultithreadIsOk()
    {
        // Калькулятор
        IDistanceCalculationService calculator = CalculatorsFactory.GetInstance();

        // Токен отмены
        using CancellationTokenSource cancellationTokenSource = new();

        Task<Result>[] tasks = Enumerable.Range(0, 100)
            .Select(task =>
            {
                Input calcInput = new() { PointA = new(0, 0), PointB = new(0, 0) };
                return calculator.CalculateAsync(calcInput, cancellationTokenSource.Token);
            })
            .ToArray();

        Result[] results = await Task.WhenAll(tasks);
        Assert.Equal(100, results.Length);
        Assert.All(results, r => Assert.True(r.Success));

        for (int i = 1; i <= 100; i++) // характерная страка для абстрактного класса с указанием номера вызова
        {
            Assert.Contains(results, r => r.Message?.StartsWith("Thread " + i) ?? false);
        }
    }
}
