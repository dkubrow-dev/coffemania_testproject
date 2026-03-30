namespace DistanceCalc.Models;

/// <summary>
/// Режимы работы калькулятора: фабрика выбирает сервис выполнения запроса, исходя из этих возможных значений
/// </summary>
public enum CalculatorServiceModes : byte
{
    /// <summary>
    /// Сервис неопределён (точно будет ошибкой)
    /// </summary>
    Undefined = 0,
    
    /// <summary>
    /// Сервис определения расстояния по прямой линии
    /// </summary>
    DirectLine = 1

}
