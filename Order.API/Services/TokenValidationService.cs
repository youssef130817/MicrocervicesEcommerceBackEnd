using System.Collections.Concurrent;
using Confluent.Kafka;
using Confluent.Kafka.Admin;
using System.Text.Json;
using Order.API.Models;

namespace Order.API.Services;

public class TokenValidationService
{
    private readonly IProducer<string, string> _producer;
    private readonly IConsumer<string, string> _consumer;
    private readonly ILogger<TokenValidationService> _logger;
    private readonly string _bootstrapServers;
    private const string TokenValidationRequestTopic = "token-validation-request";
    private const string TokenValidationResponseTopic = "token-validation-response";
    private readonly ConcurrentDictionary<string, TaskCompletionSource<TokenValidationMessage>> _pendingValidations = new();

    public TokenValidationService(IConfiguration configuration, ILogger<TokenValidationService> logger)
    {
        _bootstrapServers = configuration["Kafka:BootstrapServers"] ?? "localhost:9092";
        _logger = logger;

        var producerConfig = new ProducerConfig
        {
            BootstrapServers = _bootstrapServers,
            MessageTimeoutMs = 10000 // 10 secondes pour le timeout des messages
        };

        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = _bootstrapServers,
            GroupId = "order-service-group",
            AutoOffsetReset = AutoOffsetReset.Latest,
            EnableAutoCommit = true
        };

        _producer = new ProducerBuilder<string, string>(producerConfig).Build();
        _consumer = new ConsumerBuilder<string, string>(consumerConfig).Build();

        // Démarrer le consommateur dans un thread séparé
        Task.Run(async () =>
        {
            try
            {
                await CreateTopicsIfNotExist();
                await ConsumeValidationResponses();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors du démarrage du service de validation");
            }
        });
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
            _logger.LogInformation("Topics Kafka créés avec succès");
        }
        catch (CreateTopicsException e)
        {
            if (!e.Message.Contains("already exists"))
            {
                _logger.LogError($"Erreur lors de la création des topics: {e.Message}");
                throw;
            }
            _logger.LogInformation("Les topics existent déjà");
        }
    }

    public async Task<TokenValidationMessage?> ValidateTokenAsync(string token)
    {
        try
        {
            var requestId = Guid.NewGuid().ToString();
            var tcs = new TaskCompletionSource<TokenValidationMessage>();
            _pendingValidations[requestId] = tcs;

            var request = new TokenValidationRequest
            {
                RequestId = requestId,
                Token = token,
                ServiceName = "Order.API"
            };

            var message = new Message<string, string>
            {
                Key = requestId,
                Value = JsonSerializer.Serialize(request)
            };

            _logger.LogInformation($"Envoi de la requête de validation {requestId} pour le token");
            var deliveryResult = await _producer.ProduceAsync(TokenValidationRequestTopic, message);
            _logger.LogInformation($"Message envoyé au topic {TokenValidationRequestTopic}, partition: {deliveryResult.Partition}, offset: {deliveryResult.Offset}");

            // Attendre la réponse avec un timeout de 15 secondes
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(15));
            var completedTask = await Task.WhenAny(tcs.Task, Task.Delay(-1, cts.Token));

            _pendingValidations.TryRemove(requestId, out _);

            if (completedTask == tcs.Task)
            {
                var result = await tcs.Task;
                _logger.LogInformation($"Réponse de validation reçue pour la requête {requestId}: IsValid={result.IsValid}");
                return result;
            }

            _logger.LogWarning($"Timeout de la validation du token pour la requête {requestId}");
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la validation du token");
            return null;
        }
    }

    private async Task ConsumeValidationResponses()
    {
        _consumer.Subscribe(TokenValidationResponseTopic);
        _logger.LogInformation($"Abonné au topic: {TokenValidationResponseTopic}");

        while (true)
        {
            try
            {
                var consumeResult = await Task.Run(() => _consumer.Consume());
                if (consumeResult?.Message?.Value == null) continue;

                _logger.LogInformation($"Message reçu du topic {TokenValidationResponseTopic}, partition: {consumeResult.Partition}, offset: {consumeResult.Offset}");

                var response = JsonSerializer.Deserialize<TokenValidationResponse>(consumeResult.Message.Value);
                if (response == null) continue;

                _logger.LogInformation($"Traitement de la réponse pour la requête {response.RequestId}");

                if (_pendingValidations.TryGetValue(response.RequestId, out var tcs))
                {
                    tcs.TrySetResult(response.Message);
                    _logger.LogInformation($"Validation de la requête {response.RequestId} terminée");
                }
                else
                {
                    _logger.LogWarning($"Aucune validation en attente trouvée pour la requête {response.RequestId}");
                }
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la consommation de la réponse de validation");
                await Task.Delay(1000); // Délai plus long en cas d'erreur
            }
        }
    }

    public void Dispose()
    {
        _producer?.Dispose();
        _consumer?.Dispose();
    }
}