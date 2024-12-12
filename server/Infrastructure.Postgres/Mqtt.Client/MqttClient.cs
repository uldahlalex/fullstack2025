// using System.Text.Json;
// using Commons;
// using Externalities.QueryModels;
// using MediatR;
// using MQTTnet;
// using MQTTnet.Client;
// using MQTTnet.Formatter;
// using service.Interfaces;
// using service.Types;
//
// namespace Externalities;
//
// public class MqttClient(IRepoLogic repoLogic//, IMediator mediator
// )
// {
//     public async Task Handle_Received_Application_Message()
//     {
//         var mqttFactory = new MqttFactory();
//         var mqttClient = mqttFactory.CreateMqttClient();
//
//         var isDocker = File.Exists("/.dockerenv"); //todo configure with options pattern
//         var mqttServer = isDocker ? "mqtt-broker" : "localhost";
//         var mqttClientOptions = new MqttClientOptionsBuilder()
//             .WithTcpServer(mqttServer, 1883)
//             .WithProtocolVersion(MqttProtocolVersion.V500)
//             .Build();
//
//         await mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);
//
//         var mqttSubscribeOptions = mqttFactory.CreateSubscribeOptionsBuilder()
//             .WithTopicFilter(f => f.WithTopic("TimeSeries"))
//             .Build();
//
//         await mqttClient.SubscribeAsync(mqttSubscribeOptions, CancellationToken.None);
//
//         mqttClient.ApplicationMessageReceivedAsync += async e =>
//         {
//             try
//             {
//                 var message = e.ApplicationMessage.ConvertPayloadToString();
//                 var ts = JsonSerializer.Deserialize<TimeSeries>(message, new JsonSerializerOptions()
//                 {
//                     PropertyNameCaseInsensitive = true
//                 });
//                 ts.timestamp = DateTimeOffset.UtcNow;
//                 var insertionResult = repoLogic.PersistTimeSeriesDataPoint(ts);
//                 await mediator.Publish(new MqttClientWantsToPersistTimeSeriesDataDto
//                 {
//                     TimeSeriesData = insertionResult
//                 });
//                 
//
//                 var pongMessage = new MqttApplicationMessageBuilder()
//                     .WithTopic("response_topic")
//                     .WithPayload("yes we received the message, thank you very much, " +
//                                  "the websocket client(s) also has the data")
//                     .WithQualityOfServiceLevel(e.ApplicationMessage.QualityOfServiceLevel)
//                     .WithRetainFlag(e.ApplicationMessage.Retain)
//                     .Build();
//                 await mqttClient.PublishAsync(pongMessage, CancellationToken.None);
//             }
//             catch (Exception exc)
//             {
//                 Console.WriteLine(exc.Message);
//                 Console.WriteLine(exc.InnerException);
//                 Console.WriteLine(exc.StackTrace);
//             }
//         };
//     }
// }
//
// public class MqttClientWantsToPersistTimeSeriesDataDto : INotification
// {
//     public TimeSeries TimeSeriesData { get; set; }
// }