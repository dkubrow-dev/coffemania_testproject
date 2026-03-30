using DistanceCalc.Abcstractions;
using DistanceCalc.Models;

namespace DistanceCalc.Services;

/// <summary>
/// Фабрика по выбору конкретного сервиса по выполнению расчётов.
/// </summary>
/// <remarks>
/// В текущей реализации излишняя, но(!) по точно потребуется при доработке сервиса.
/// Для примера - просто по текущим настройкам из ini-файла, но потом фабрику можно сделать динамически настраиваемой
/// </remarks>
public class CalculatorsFactory
{
    /// <summary>
    /// По текущим настройкам выбирает нужный для работы сервис и возвращает его интерфейс
    /// </summary>
    /// <returns>Интерфейс взаимодействия с сервисом расчёта длины</returns>
    /// <exception cref="Exception">В случае проблем с настройками не сможет выполнить поиск сервиса</exception>
    public static IDistanceCalculationService GetInstance()
    {
        if (!Enum.TryParse(Settings.GetValue("CalculationServiceMode"), out CalculatorServiceModes mode))
        {
            throw new Exception("No valid \"CalculationServiceMode\" value in Settings.");
        }

        return mode switch
        {
            CalculatorServiceModes.DirectLine => new DirectLineCalculationService(),
            // добавлять сюда новые значения и при реализации новых сервисов

            CalculatorServiceModes.Undefined => throw new Exception("Check Settings: \"CalculationServiceMode\" is Undefined"),
            _ => throw new Exception("Check Settings: unexpected value \"CalculationServiceMode\" " + mode.ToString()),
        };
    }
}
