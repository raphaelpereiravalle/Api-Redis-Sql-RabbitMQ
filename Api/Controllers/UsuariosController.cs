using Application.Services;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;


namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsuariosController : ControllerBase
{
    private readonly UsuarioService _service;

    public UsuariosController(UsuarioService service)
    {
        _service = service;
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var usuario = await _service.GetByIdAsync(id);
        return usuario == null ? NotFound() : Ok(usuario);
    }

    [HttpPost]
    public IActionResult Post([FromBody] Usuario usuario)
    {
        var factory = new ConnectionFactory() { HostName = "rabbitmq" };
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        channel.QueueDeclare(queue: "cadastro-usuario",
                            durable: false,
                            exclusive: false,
                            autoDelete: false,
                            arguments: null);

        var json = JsonSerializer.Serialize(usuario);
        var body = Encoding.UTF8.GetBytes(json);

        channel.BasicPublish(exchange: "",
                            routingKey: "cadastro-usuario",
                            basicProperties: null,
                            body: body);

        return Accepted("Cadastro enviado para processamento.");
    }
}