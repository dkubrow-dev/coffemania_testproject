using DistanceCalc.Abstractions;
using DistanceCalc.Models;

namespace DistanceCalc.Services;

/// <summary>
/// Настройки сервиса для калькулятора.
/// </summary>
internal class CalculatorSettingsProvider : ISettingsProvider
{
    /// <summary>
    /// Имя секции в настройках appsettings.json
    /// </summary>
    public const string SectionName = "Calculator";

    /// <summary>
    /// Режим, определяющий фактического исполнителя для расчёта растояния
    /// </summary>
    public CalculatorServiceModes Mode {  get; set; }

    /// <summary>
    /// Симуляция задержки для калькулятора
    /// </summary>
    public int SimulateDelayMs { get; set; }
}
