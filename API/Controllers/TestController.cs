using Application.Features.UserUseCase;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace InterpriseChatService.Controllers;

[ApiController]
[Route("[controller]")]
public class TestController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<TestController> _logger;

    public TestController(ILogger<TestController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get()
    {
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();
    }
    [HttpGet("ping")]
    public async Task<IActionResult> Ping([FromServices] IMediator mediator)
    {
        var result = await mediator.Send(new PingCommand());
        return Ok(result); 
    }
}
