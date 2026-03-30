using DistanceCalc.Abcstractions;
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
    private IDistanceCalculationService calculator;

    /// <summary>
    /// Возвращает контроллер определения пути
    /// </summary>
    /// <param name="calculator"></param>
    public DistanceController(IDistanceCalculationService calculator)
    {
        this.calculator = calculator;
    }

    /// <summary>
    /// Рассчитывает путь от переданных точек A и B. Возвращает расстояние в километрах.
    /// Ожидаемое время выполнения: 1 500 миллисекунды (искуственной задержки) + само время расчёта
    /// </summary>
    /// <param name="input">Входные данные для расчёта расстояния</param>
    /// <param name="cancellationToken">Токен отмены выполнения</param>
    /// <returns>
    /// Объект передачи данных о расстоянии: 
    /// Success - флаг успеха выполнения операции. 
    /// Message - сервисная информация по выполненной операции.
    /// Extption - информация об ошибке, если та произошла
    /// Distance - рассчитанное расстояние, если оно было посчитано.
    /// </returns>
    [HttpPost("GetDistance")]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status499ClientClosedRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Result>> GetDistanceAsync([FromBody] Input input, CancellationToken cancellationToken)
    {
        try
        {
            var result = await calculator.CalculateAsync(input, cancellationToken);
            if (result.Success == false)
            {
                if (result.Exception is TaskCanceledException)
                {
                    return Problem(
                        title: "Запрос отменён",
                        detail: "Операция расчёта была отменена.",
                        statusCode: 499);
                }

                return Problem(
                    title: "Внутренняя ошибка сервиса",
                    detail: result.Message,
                    statusCode: 500);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            return Problem(
                title: "Внутренняя ошибка сервиса",
                detail: ex.Message,
                statusCode: 500);
        }
    }
}
