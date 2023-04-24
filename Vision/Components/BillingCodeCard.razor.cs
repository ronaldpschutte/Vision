using Microsoft.AspNetCore.Components;
using Vision.Models;
using Vision.Services;
using Vision.Helpers;
using MudBlazor;

namespace Vision.Components
{
    public partial class BillingCodeCard
    {
        [Parameter]
        public BillingCode Code { get; set; }
        [Inject]
        private DataContextProvider contextProvider { get; set; }
        [Inject]
        private TimeEntryService timeEntryService { get; set; }
        [Inject]
        private ILogger<BillingCodeCard> log { get; set; }


        private MudToggleIconButton TimerActiveToggle;
        private string Note { get; set; }
        private bool TimerActive { get; set; } = false;


        protected async override System.Threading.Tasks.Task OnInitializedAsync()
        {
            timeEntryService.TimeEntriesUpdated += SetTimeEntry;
        }



        public void SetTimeEntry(object sender, TimeEntryEventArgs args)
        {

            if (args.BillingCodeId != Code.BillingCodeId)
            {
                TimerActive = false;
                //Note = "not this one: " + Code.Name + " : " + TimerActive.ToString();
            }
            else
            {
                TimerActive = !TimerActive;
                //Note = "this one: " + Code.Name + " : " + TimerActive.ToString();
            }
            log.LogInformation(Code.Name + ":" + TimerActive.ToString());
            StateHasChanged();

        }
        public void ActiveBillingCode()
        {
             
            timeEntryService.UpdateActiveTimeEntry(Code.BillingCodeId, TimerActive, Code.Name, Note,5, 0);
            StateHasChanged();
        }

    }
}
