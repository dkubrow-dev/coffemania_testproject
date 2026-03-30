using DistanceCalc.Abstractions;
using DistanceCalc.Models;
using Microsoft.AspNetCore.Mvc;

namespace DistanceWebApi.Controllers;

/// <summary>
/// Контроллер, определяющий путь до точек
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class DistanceController : ControllerBase
{
    /// <summary>
    /// Сервис расчётов дистанции.
    /// </summary>
    private readonly IDistanceCalculationService _calculator;

    /// <summary>
    /// Логгер контроллера
    /// </summary>
    private readonly ILogger<DistanceController> _logger;

    /// <summary>
    /// Возвращает контроллер определения пути
    /// </summary>
    /// <param name="calculator">Сервис расчётов расстояния</param>
    /// <param name="logger">Логгер контроллера</param>
    public DistanceController(IDistanceCalculationService calculator,
        ILogger<DistanceController> logger)
    {
        _calculator = calculator;
        _logger = logger;
    }

    /// <summary>
    /// Рассчитывает путь от переданных точек A и B. Возвращает расстояние в километрах.
    /// </summary>
    /// <param name="input">Входные данные для расчёта расстояния</param>
    /// <param name="cancellationToken">Токен отмены выполнения</param>
    /// <returns>
    /// Объект передачи данных о расстоянии: 
    /// Success - флаг успеха выполнения операции. 
    /// Message - сервисная информация по выполненной операции.
    /// ErrorCode - код ошибки, если та произошла, либо 0, если всё в порядке.
    /// Distance - рассчитанное расстояние, если оно было посчитано.
    /// </returns>
    [HttpPost("Calculate")]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status499ClientClosedRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Result>> Calculate([FromBody] Input input, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _calculator.CalculateAsync(input, cancellationToken);
            if (result.Success == false)
            {
                if (result.ErrorCode == ErrorCodes.Canceled)
                {
                    _logger.LogWarning("Distance calculation canceled. RequestId={RequestId}, ErrorCode={ErrorCode}",
                        HttpContext.TraceIdentifier, result.ErrorCode);

                    return Problem(
                        title: "Запрос отменён",
                        detail: "Операция расчёта была отменена.",
                        statusCode: 499);
                }

                _logger.LogError(
                    "Distance calculation returned internal failure. RequestId={RequestId}, ErrorCode={ErrorCode}, Message={Message}",
                    HttpContext.TraceIdentifier,
                    result.ErrorCode,
                    result.Message);

                return Problem(
                    title: "Внутренняя ошибка сервиса",
                    detail: result.Message,
                    statusCode: 500);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Distance controller failed. RequestId={RequestId}",
                HttpContext.TraceIdentifier);

            return Problem(
                title: "Внутренняя ошибка сервиса",
                detail: ex.Message,
                statusCode: 500);
        }
    }
}
