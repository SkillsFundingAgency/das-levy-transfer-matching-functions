﻿using System;
using System.IO;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Xml;
using Azure.Identity;
using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using Microsoft.Extensions.Logging;
using SFA.DAS.LevyTransferMatching.Infrastructure.Configuration;
using SFA.DAS.LevyTransferMatching.Infrastructure.Extensions;

namespace SFA.DAS.LevyTransferMatching.Infrastructure.Legacy;

public class LegacyTopicMessagePublisher : ILegacyTopicMessagePublisher
{
    private readonly ILogger<LegacyTopicMessagePublisher> _logger;
    private readonly string _legacyServiceBusNamespace;

    public LegacyTopicMessagePublisher(ILogger<LegacyTopicMessagePublisher> logger, LevyTransferMatchingFunctions config)
    {
        _logger = logger;
        _legacyServiceBusNamespace = config.SharedServiceBusNamespace;
    }

    public async Task PublishAsync<T>(T @event)
    {
        _logger.LogInformation($"Publishing {@event.GetType()}");
        _logger.LogInformation($"NS is {_legacyServiceBusNamespace.Length} length");

        ServiceBusClient client = null;
        try
        {
            var topicName = GetTopicName(@event);
            var subscriptionName = GetSubscriptionName(@event);

            await CreateTopic(topicName);
            await CreateSubscription(topicName, subscriptionName);

            client = new ServiceBusClient(_legacyServiceBusNamespace, new DefaultAzureCredential());
            var sender = client.CreateSender(topicName);
            var messageBody = Serialize(@event);
            var message = new ServiceBusMessage(messageBody);
            await sender.SendMessageAsync(message);

            _logger.LogInformation($"Sent Message {typeof(T).Name} to Azure ServiceBus ");
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"Error sending Message {typeof(T).Name} to Azure ServiceBus");
            throw;
        }
        finally
        {
            if (client != null && !client.IsClosed)
            {
                _logger.LogDebug("Closing legacy topic message publisher");
                await client.DisposeAsync();
            }
        }
    }

    private async Task CreateTopic(string topicName)
    {
        var client = new ServiceBusAdministrationClient(_legacyServiceBusNamespace, new DefaultAzureCredential());
        var exists = await client.TopicExistsAsync(topicName);
        if (exists) return;

        await client.CreateTopicAsync(topicName);
    }

    private async Task CreateSubscription(string topicName, string subscriptionName)
    {
        var client = new ServiceBusAdministrationClient(_legacyServiceBusNamespace, new DefaultAzureCredential());
        var exists = await client.SubscriptionExistsAsync(topicName, subscriptionName);
        if (exists) return;

        var subscriptionOptions = new CreateSubscriptionOptions(topicName, subscriptionName);
        await client.CreateSubscriptionAsync(subscriptionOptions);
    }

    private static string GetSubscriptionName(object obj)
    {
        return $"Task_{obj.GetType().Name}";
    }

    private static string GetTopicName(object obj)
    {
        return obj.GetType().Name.ToUnderscoreCase();
    }

    private static byte[] Serialize<T>(T obj)
    {
        var serializer = new DataContractSerializer(typeof(T));
        var stream = new MemoryStream();
        using (var writer = XmlDictionaryWriter.CreateBinaryWriter(stream))
        {
            serializer.WriteObject(writer, obj);
        }
        return stream.ToArray();
    }
}