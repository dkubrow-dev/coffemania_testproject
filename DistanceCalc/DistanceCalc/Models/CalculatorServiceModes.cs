namespace DistanceCalc.Models;

/// <summary>
/// Режимы работы калькулятора: фабрика выбирает сервис выполнения запроса, исходя из этих возможных значений
/// </summary>
internal enum CalculatorServiceModes : byte
{
    /// <summary>
    /// Сервис неопределён (точно будет ошибкой)
    /// </summary>
    Undefined = 0,
    
    /// <summary>
    /// Сервис определения расстояния по прямой линии
    /// </summary>
    DirectLine = 1

    // сюда добавлять новые значения при добавлении новых сервисов
}
