using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Xml;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Logging;

namespace SFA.DAS.LevyTransferMatching.Infrastructure.Legacy
{
    public class LegacyTopicMessagePublisher : ILegacyTopicMessagePublisher
    {
        private readonly ILogger<LegacyTopicMessagePublisher> _logger;
        private readonly string _connectionString;

        public LegacyTopicMessagePublisher(ILogger<LegacyTopicMessagePublisher> logger, string connectionString)
        {
            _logger = logger;
            _connectionString = connectionString;
        }

        public async Task PublishAsync<T>(T @event)
        {
            var messageGroupName = GetMessageGroupName(@event);

            ServiceBusClient client = null;
            try
            {
                client = new ServiceBusClient(_connectionString);
                var sender = client.CreateSender(messageGroupName);
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

        private static string GetMessageGroupName(object obj)
        {
            CustomAttributeData customAttributeData = obj.GetType().CustomAttributes.FirstOrDefault<CustomAttributeData>((Func<CustomAttributeData, bool>)(att => att.AttributeType.Name == "MessageGroupAttribute"));
            string str = customAttributeData != null ? (string)customAttributeData.ConstructorArguments.FirstOrDefault<CustomAttributeTypedArgument>().Value : (string)(object)null;
            if (!string.IsNullOrEmpty(str))
                return str;
            return obj.GetType().Name;
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
}
