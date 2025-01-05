using Confluent.Kafka;
using System.Text.Json;
using Authentication.API.Models;

namespace Authentication.API.Services;

public class KafkaProducerService
{
    private readonly IProducer<string, string> _producer;
    private const string TokenValidationResponseTopic = "token-validation-response";

    public KafkaProducerService(IConfiguration configuration)
    {
        var config = new ProducerConfig
        {
            BootstrapServers = configuration["Kafka:BootstrapServers"]
        };

        _producer = new ProducerBuilder<string, string>(config).Build();
    }

    public async Task PublishValidationResponse(string requestId, TokenValidationMessage message)
    {
        try
        {
            var jsonMessage = JsonSerializer.Serialize(new
            {
                RequestId = requestId,
                Message = message
            });

            var kafkaMessage = new Message<string, string>
            {
                Key = requestId,
                Value = jsonMessage
            };

            await _producer.ProduceAsync(TokenValidationResponseTopic, kafkaMessage);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error publishing validation response: {ex.Message}");
            throw;
        }
    }

    public void Dispose()
    {
        _producer?.Dispose();
    }
}