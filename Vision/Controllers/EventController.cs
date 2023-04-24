using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;
using Vision.Helpers;
using Vision.Hubs;
using Vision.Models;
using Vision.Repository;
using Vision.Services;

namespace Vision.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class EventController : Controller
    {
        
        private readonly IHubContext<EventGridHub> _hubContext;
        private readonly ILogger<EventController> log;
        private readonly DataContextProvider dataContextProvider;
        //private RepositoryService _repo;
        private bool EventTypeSubcriptionValidation => HttpContext.Request.Headers["aeg-event-type"].FirstOrDefault() == "SubscriptionValidation";
        private bool EventTypeNotification => HttpContext.Request.Headers["aeg-event-type"].FirstOrDefault() == "Notification";

        public EventController(IHubContext<EventGridHub> eventGridHubContext, ILogger<EventController> log, DataContextProvider dataContextProvider)
        {
            _hubContext = eventGridHubContext;
            this.log = log;
            this.dataContextProvider = dataContextProvider;
        }

        [HttpOptions]
        public async Task<IActionResult> Options()
        {
            //Request.Headers.TryGetValue("WebHook-Request-Origin", out var requestHeaderValue);
            //Response.Headers.Add("WebHook-Allowed-Origin", requestHeaderValue); // TODO, add topic urls for security reasons
            Response.Headers.Add("WebHook-Allowed-Origin", "*");
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Post()
        {
            using (var reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                var jsonContent = await reader.ReadToEndAsync();
                if (EventTypeSubcriptionValidation)
                {
                    return await HandleValidation(jsonContent);
                }
                else if (EventTypeNotification)
                {
                    if (IsCloudEvent(jsonContent))
                    {
                        return await HandleCloudEvent(jsonContent);
                    }

                    return await HandleGridEvents(jsonContent);
                }
              
                return BadRequest();
            }
        }

        private async Task<JsonResult> HandleValidation(string jsonContent)
        {
            var gridEvent = JsonConvert.DeserializeObject<List<GridEvent<Dictionary<string, string>>>>(jsonContent).First();

            await this._hubContext.Clients.All.SendAsync(
                "gridupdate",
                gridEvent.Id,
                gridEvent.EventType,
                gridEvent.Subject,
                gridEvent.EventTime.ToLongTimeString(),
                jsonContent.ToString());

            var validationCode = gridEvent.Data["validationCode"];
            return new JsonResult(new
            {
                validationResponse = validationCode
            });
        }

        private async Task<IActionResult> HandleGridEvents(string jsonContent)
        {
            var events = JArray.Parse(jsonContent);
            foreach (var e in events)
            {
                var details = JsonConvert.DeserializeObject<GridEvent<dynamic>>(e.ToString());
                await this._hubContext.Clients.All.SendAsync(
                    "gridupdate",
                    details.Id,
                    details.EventType,
                    details.Subject,
                    details.EventTime.ToLongTimeString(),
                    e.ToString());
            }

            return Ok();
        }
        
        private async Task<IActionResult> HandleCloudEvent(string jsonContent)
        {
            try
            {

           
                var details = JsonConvert.DeserializeObject<CloudEvent<dynamic>>(jsonContent);
                
                if(details.Source == "Vision")
                {
                    return Ok();

                }
                var key = Int32.Parse(details.Subject);

                await dataContextProvider.ExternalUpdate(details.Type, key);
                //if(details.Type == "WorkflowStep")
                //{

                    // var updateStep = dataContextProvider.WorkflowSteps.Where(w => w.WorkflowStepId == WorkflowStepId).FirstOrDefault();
                    //if (updateStep != null)
                    //{

                        //updateStep = dataContextProvider._ctx.WorkflowStep.Where(w => w.WorkflowStepId == WorkflowStepId).FirstOrDefault(); // RPS Dit gaat verkeerd. Niet het record uit DB maar uit geheugen,
                    //}
                    //else
                    //{
                    //    //updateStep = dataContextProvider._ctx.WorkflowStep.Where(w => w.WorkflowStepId == WorkflowStepId).FirstOrDefault();
                    //    dataContextProvider.WorkflowSteps.Add(updateStep);
                    //}
                //}

                //var eventData = JObject.Parse(jsonContent);
                //string formattedObject = JsonConvert.SerializeObject(details.Data);
                //var dataObject = JsonConvert.DeserializeObject<WorkflowStep>(formattedObject);
                //WorkflowStep step = dataObject as WorkflowStep;
                //var newStep = dataContextProvider.WorkflowSteps.Where(w => w.WorkflowStepId == step.WorkflowStepId).FirstOrDefault();
                //newStep = dataContextProvider._ctx.WorkflowStep.Where(w => w.WorkflowStepId == step.WorkflowStepId).FirstOrDefault();
                // dataContextProvider.WorkflowStepsUpdated?.Invoke(null, null); beter om vanuit datacontextprovider zelf te doen. RPS
                
                return Ok(); 
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
              return NotFound();
            }
        }

        private bool IsCloudEvent(string jsonContent)
        {
            try
            {
                var eventData = JObject.Parse(jsonContent);
                var version = eventData["specversion"].Value<string>();
                if (!string.IsNullOrEmpty(version)) return true;
            }
            catch (Exception e)
            {
                log.LogError(e.Message, e, jsonContent);
            }

            return false;
        }
    }
}
