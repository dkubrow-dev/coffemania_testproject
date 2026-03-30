using Microsoft.Extensions.Logging;
using System.Text;
using Xunit.Abstractions;

namespace DistanceCalc_Tests;

/// <summary>
/// Относительно простенький ILogger для того, чтобы можно было почитать логи после прогона тестов
/// </summary>
internal sealed class TestFileLogger : ILogger
{
    /// <summary>
    /// Имя категории логгирования
    /// </summary>
    private readonly string _categoryName;

    /// <summary>
    /// Путь к файлу логов
    /// </summary>
    private readonly string _filePath;

    /// <summary>
    /// Объект блокировки для потоков
    /// </summary>
    private readonly object _sync;

    /// <summary>
    /// Вывод логов в окно тестов
    /// </summary>
    private readonly ITestOutputHelper? _output;

    /// <summary>
    /// Возвращает файловый тестовый логгер
    /// </summary>
    /// <param name="categoryName">Имя категории логгирования</param>
    /// <param name="filePath">Путь к файлу логов</param>
    /// <param name="sync">Объект блокировки для потоков</param>
    /// <param name="output">Объект вывода лога в окно тестов</param>
    public TestFileLogger(string categoryName, string filePath, object sync, ITestOutputHelper? output)
    {
        _categoryName = categoryName;
        _filePath = filePath;
        _sync = sync;
        _output = output;
    }

    // излишество для тестового провайдера: скопы возвращаем null-ами
    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

    /// <summary>
    /// Факт того, что провайдер работает
    /// </summary>
    /// <param name="logLevel">Установленный уровень логгирования</param>
    /// <returns>Факт, что логгер не выключен</returns>
    public bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None;

    /// <summary>
    /// Выполняет фактическую запись сообщения в лог
    /// </summary>
    /// <typeparam name="TState">Тип объекта состояния, переданного в логгер</typeparam>
    /// <param name="logLevel">Уровень важности сообщения.</param>
    /// <param name="eventId">Идентификатор события логирования</param>
    /// <param name="state">Объект состояния, содержащий данные для форматирования записи.</param>
    /// <param name="exception">Исключение, связанное с сообщением (если было)</param>
    /// <param name="formatter">Функция форматирования вывода</param>
    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel))
            return;

        string message = formatter(state, exception);

        var sb = new StringBuilder();
        sb.Append('[').Append(DateTime.Now.ToString("O")).Append("] ");
        sb.Append('[').Append(logLevel).Append("] ");
        sb.Append('[').Append(_categoryName).Append("] ");
        sb.Append(message);

        if (exception is not null)
        {
            sb.AppendLine();
            sb.Append(exception);
        }

        string finalMessage = sb.ToString();

        lock (_sync)
        {
            File.AppendAllText(_filePath, finalMessage + Environment.NewLine, Encoding.UTF8);
            if (_output is not null)
            {
                try
                {
                    _output.WriteLine(finalMessage);
                }
                catch
                {
                    // не критично, если тут отвалится - игнорируем
                }
            }
        }
    }
}

/// <summary>
/// Фабрика для тестового логгера
/// </summary>
internal sealed class TestFileLoggerFactory : ILoggerFactory
{
    /// <summary>
    /// Путь к файлу лога
    /// </summary>
    private readonly string _filePath;

    /// <summary>
    /// Объект синхронизации блокирования ресурса для потоков
    /// </summary>
    private readonly object _sync = new();

    private readonly ITestOutputHelper? _output;

    /// <summary>
    /// Возвращает фабрику для создания лога
    /// </summary>
    /// <param name="filePath">Путь к файлу логов</param>
    /// <param name="output">объект вывода в окно тестов</param>
    public TestFileLoggerFactory(string filePath, ITestOutputHelper? output = null)
    {
        _filePath = filePath;

        string? dir = Path.GetDirectoryName(_filePath);

        if (!string.IsNullOrWhiteSpace(dir))
            Directory.CreateDirectory(dir);

        _output = output;
    }

    // слишком сложно для провайдера тестов
    public void AddProvider(ILoggerProvider provider) { }

    /// <summary>
    /// Возвращает логгер по запросу к фабрике
    /// </summary>
    /// <param name="categoryName">Название категории логгера</param>
    /// <returns>Логгер для тестов</returns>
    public ILogger CreateLogger(string categoryName) =>
        new TestFileLogger(categoryName, _filePath, _sync, _output);

    // высвобождать нечего
    public void Dispose() { }
}