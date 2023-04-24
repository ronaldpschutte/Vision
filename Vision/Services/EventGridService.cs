using Azure;
using Azure.Messaging;
using Azure.Messaging.EventGrid;
using Vision.Helpers;

namespace Vision.Services
{
    public class EventGridService
    {
        private EventGridPublisherClient _client;
        private ILogger<EventGridService> log;

        public EventGridService(IConfiguration config, ILogger<EventGridService> log)
        {
//#if !DEBUG

//            _client = new EventGridPublisherClient(new Uri(config["SyncTopic_DEV:TopicEndpoint"]), new AzureKeyCredential(config["SyncTopic_DEV:TopicKey"]));
//#else
            _client = new EventGridPublisherClient(new Uri(config["SyncTopic_PROD:TopicEndpoint"]), new AzureKeyCredential(config["SyncTopic_PROD:TopicKey"]));
            this.log = log;
//#endif
        }

        public async Task<bool> SendDataSyncedEvent(object data)
        {
            var typeName = data.GetType().ToString(); // "WorkflowStep"; // DataSyncedEventType.GetEventTypeName(data);
            typeName = typeName.Substring(typeName.LastIndexOf('.') + 1);
            string Entiteit= Newtonsoft.Json.JsonConvert.SerializeObject(data);
            string Id = Entiteit.Substring(Entiteit.IndexOf(':') + 1,6);
            return await SendCommonEvent(Id, typeName, Entiteit);
        }

        private async Task<bool> SendCommonEvent(string subject, string type, object data)
        {
            try
            {
               // List<EventGridEvent> cloudEvents =  new List<EventGridEvent>();
                List<CloudEvent> cloudEvents = new List<CloudEvent>()
                {
                  //  new EventGridEvent(subject, type, dataVersion, data);
                    new CloudEvent("Vision", type, data) { Subject = subject }
                };
                // return true; Tijdens testen met produktiedata RPS !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                var response = await _client.SendEventsAsync(cloudEvents);
    
                return (response.Status == 200);
            }
            catch (Exception ex)
            {
                log.LogError(ex.Message, ex);
                return false;
            }

        }
    }
}
