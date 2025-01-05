using Confluent.Kafka;
using Confluent.Kafka.Admin;
using System.Text.Json;
using System.Security.Claims;
using Authentication.API.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Authentication.API.Services;

public class KafkaConsumerService : BackgroundService
{
    private readonly IConsumer<string, string> _consumer;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly KafkaProducerService _producer;
    private readonly ILogger<KafkaConsumerService> _logger;
    private readonly string _bootstrapServers;
    private const string TokenValidationRequestTopic = "token-validation-request";
    private const string TokenValidationResponseTopic = "token-validation-response";

    public KafkaConsumerService(
        IConfiguration configuration,
        IServiceScopeFactory scopeFactory,
        KafkaProducerService producer,
        ILogger<KafkaConsumerService> logger)
    {
        _bootstrapServers = configuration["Kafka:BootstrapServers"] ?? "localhost:9092";

        var config = new ConsumerConfig
        {
            BootstrapServers = _bootstrapServers,
            GroupId = "auth-service-group",
            AutoOffsetReset = AutoOffsetReset.Latest
        };

        _consumer = new ConsumerBuilder<string, string>(config).Build();
        _scopeFactory = scopeFactory;
        _producer = producer;
        _logger = logger;
    }

    private async Task CreateTopicsIfNotExist()
    {
        using var adminClient = new AdminClientBuilder(new AdminClientConfig { BootstrapServers = _bootstrapServers }).Build();
        try
        {
            var topics = new TopicSpecification[]
            {
                new TopicSpecification { Name = TokenValidationRequestTopic, ReplicationFactor = 1, NumPartitions = 1 },
                new TopicSpecification { Name = TokenValidationResponseTopic, ReplicationFactor = 1, NumPartitions = 1 }
            };

            await adminClient.CreateTopicsAsync(topics);
            _logger.LogInformation("Kafka topics created successfully");
        }
        catch (CreateTopicsException e)
        {
            if (!e.Message.Contains("already exists"))
            {
                _logger.LogError($"Error creating topics: {e.Message}");
                throw;
            }
            _logger.LogInformation("Topics already exist");
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Créer les topics au démarrage
        await CreateTopicsIfNotExist();

        _consumer.Subscribe(TokenValidationRequestTopic);
        _logger.LogInformation($"Subscribed to topic: {TokenValidationRequestTopic}");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var consumeResult = _consumer.Consume(stoppingToken);
                if (consumeResult != null)
                {
                    var request = JsonSerializer.Deserialize<TokenValidationRequest>(consumeResult.Message.Value);
                    if (request != null)
                    {
                        using var scope = _scopeFactory.CreateScope();
                        var authService = scope.ServiceProvider.GetRequiredService<AuthService>();

                        var isRevoked = await authService.IsTokenRevoked(request.Token);
                        var claims = authService.GetTokenClaims(request.Token);

                        var validationMessage = new TokenValidationMessage
                        {
                            Token = request.Token,
                            IsValid = !isRevoked,
                            UserId = claims?.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                            Role = claims?.FindFirst(ClaimTypes.Role)?.Value,
                            Message = isRevoked ? "Token révoqué" : "Token valide"
                        };

                        await _producer.PublishValidationResponse(request.RequestId, validationMessage);
                        _logger.LogInformation($"Token validation response sent for request {request.RequestId}");
                    }
                }
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error processing validation request: {ex.Message}");
                await Task.Delay(1000, stoppingToken); // Attendre un peu avant de réessayer
            }
        }
    }

    public override void Dispose()
    {
        _consumer?.Dispose();
        base.Dispose();
    }
}