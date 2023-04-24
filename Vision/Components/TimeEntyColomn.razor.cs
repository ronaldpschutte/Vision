using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using Synlogic.Autotask;
using Synlogic.Autotask.Entities;
using Vision.Helpers;
using Vision.Models;
using Vision.Services;

namespace Vision.Components
{
    public partial class TimeEntyColomn
    {

        [Inject]
        private DataContextProvider contextProvider { get; set; }
        [Inject]
        private TimeEntryService timeEntryService { get; set; }
        [Inject]
        private ILogger<TimeEntyColomn> log { get; set; }
        private List<TimeEntry> timeEntries { get; set; } = new List<TimeEntry>(); // Moet naar timeEntry Service voor refresh RPS

        private DateTime datum { get; set; } = DateTime.Now.Date;



        private string Args = "";

        
        
        protected async override System.Threading.Tasks.Task OnInitializedAsync()
        {
            try
            {

                timeEntryService.TimeEntriesUpdated += SetTimeEntry;
                await timeEntryService.GetTimeEntriesFromAPItoDB();
                Args = AutoTaskCredentials.GetWebUrl();
            }
            catch (Exception ex)
            {
                log.LogError(ex.Message, ex);
                Console.WriteLine(ex.Message);
            }
        }
        public void SetTimeEntry(object sender, TimeEntryEventArgs args)
        {

            Args = JsonConvert.SerializeObject(timeEntryService.ActiveTimeEntry, Formatting.Indented);
            StateHasChanged();
            
        }

    }
}
