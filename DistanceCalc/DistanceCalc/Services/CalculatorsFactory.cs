using DistanceCalc.Abstractions;
using DistanceCalc.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace DistanceCalc.Services;

/// <summary>
/// Фабрика по выбору конкретного сервиса по выполнению расчётов.
/// </summary>
/// <remarks>
/// В текущей реализации по большей части излишняя, но(!) по точно потребуется при доработке сервиса.
/// Для примера - просто по текущим настройкам из ini-файла, но потом фабрику можно сделать динамически настраиваемой
/// </remarks>
public static class CalculatorsFactory
{
    /// <summary>
    /// По текущим настройкам выбирает нужный для работы сервис и возвращает его интерфейс
    /// </summary>
    /// <param name="settings">Провайдер настроек калькулятора, предоставляемый вызывающим кодом</param>
    /// <param name="loggerFactory">Фабрика логгера, предоставляемая вызывающим кодом</param>
    /// <returns>Интерфейс взаимодействия с сервисом расчёта длины</returns>
    /// <exception cref="Exception">В случае проблем с настройками не сможет выполнить поиск сервиса</exception>
    public static IDistanceCalculationService GetInstance(ISettingsProvider settings, ILoggerFactory? loggerFactory = null)
    {
        loggerFactory ??= NullLoggerFactory.Instance;
        ILogger factoryLogger = loggerFactory.CreateLogger(typeof(CalculatorsFactory));

        if (factoryLogger.IsEnabled(LogLevel.Debug))
        {
            factoryLogger.LogDebug(
                "Creating distance calculation service. Mode={Mode}, SimulateDelayMs={SimulateDelayMs}",
                settings.Mode,
                settings.SimulateDelayMs);
        }

        return settings.Mode switch
        {
            CalculatorServiceModes.DirectLine =>
                new DirectLineCalculationService(settings, loggerFactory.CreateLogger<DirectLineCalculationService>()),

            CalculatorServiceModes.Undefined => throw CreateFactoryException(factoryLogger, settings.Mode),
            _ => throw CreateFactoryException(factoryLogger, settings.Mode)
        };
    }

    /// <summary>
    /// Создаёт и логгирует исключение, выкидываемое из-за невалидного режима работы калькулятора
    /// </summary>
    /// <param name="logger">Логгер для проведения записи лога об ошибке</param>
    /// <param name="mode">Запрошенный режим работы калькулятора</param>
    /// <returns>Исключение для дальнейшего выбрасывания выше</returns>
    private static InvalidOperationException CreateFactoryException(ILogger logger, CalculatorServiceModes mode)
    {
        var ex = new InvalidOperationException($"Check ISettingsProvider: invalid value of {nameof(ISettingsProvider.Mode)} = {mode}.");
        logger.LogError(ex, "Failed to create distance calculation service. Mode={Mode}", mode);
        return ex;
    }
}
