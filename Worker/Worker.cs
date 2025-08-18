using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Cache;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace Worker.Services
{
    public class WorkerService : BackgroundService
    {
        private readonly ILogger<WorkerService> _logger;
        private readonly IUsuarioRepository _repo;
        private readonly RedisService _cache;
        private readonly IConfiguration _config;

        public WorkerService(
            ILogger<WorkerService> logger,
            IUsuarioRepository repo,
            RedisService cache,
            IConfiguration config)
        {
            _logger = logger;
            _repo = repo;
            _cache = cache;
            _config = config;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var rabbitHost = _config.GetValue<string>("RabbitMQ:Host") ?? "localhost";

            var factory = new ConnectionFactory() { HostName = rabbitHost };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue: "cadastro-usuario",
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var usuario = JsonSerializer.Deserialize<Usuario>(message);

                if (usuario != null)
                {
                    var created = await _repo.CreateAsync(usuario);
                    await _cache.SetUsuarioAsync(created);
                    _logger.LogInformation($"[Worker] Usuário processado: {created.Id} - {created.Nome}");
                }
            };

            channel.BasicConsume(queue: "cadastro-usuario", autoAck: true, consumer: consumer);

            _logger.LogInformation("[Worker] Escutando fila 'cadastro-usuario'...");
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
    }
}
