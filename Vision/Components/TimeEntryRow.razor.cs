using Microsoft.AspNetCore.Components;
using MudBlazor;
using Synlogic.Autotask.Entities;
using Vision.Models;
using Vision.Services;
using Task = System.Threading.Tasks.Task;

namespace Vision.Components
{
    public partial class TimeEntryRow
    {
        [Parameter]
        public TimeEntry dbEntry { get; set; }

        [Parameter]
        public DateTime datum { get; set; }

        [Inject]
        private TimeEntryService timeEntryService { get; set; }

        [Inject]
        private ISnackbar Snackbar { get; set; }
        [Inject]
        private IDialogService MudDialog { get; set; }

        private bool EntrySynced { get; set; } = false;

        private bool ShowComment { get; set; } = false;

        public TimeSpan? tsStart
        {
            get
            {
                if (dbEntry.StartDateTime == null && dbEntry.HoursWorked != 0) {
                    dbEntry.StartDateTime = DateTime.Now;
                }
                return dbEntry.StartDateTime.Value.TimeOfDay;
            }
            set
            {
                dbEntry.StartDateTime = datum.Date + value;
            }
        }
        public TimeSpan? tsEnd { 
            get
            {
                if (dbEntry.EndDateTime == null && dbEntry.HoursWorked != 0)
                {
                    dbEntry.EndDateTime = DateTime.Now;
                }
                return dbEntry.EndDateTime.Value.TimeOfDay;
            }
            set
            {
                dbEntry.EndDateTime = datum.Date + value;
            } 
        }
        public async Task CreateTimeEntry()
        {
            try
            {

                // Deze hele functie moet naar de service
                // timeEntryService.ActiveTimeEntry
                //EntrySynced = toggled;

                // var hours = ((DateTime)dbEntry.EndDateTime).Subtract(((DateTime)dbEntry.StartDateTime)).Hours;

                //timeEntryService.UpdateActiveTimeEntry() = apiEntry; // RPS is nog niet volledig

                var timeEntry = await Synlogic.Autotask.Controllers.TimeEntryController.CreateAsync(dbEntry);
                Snackbar.Add($"{dbEntry.Id.ToString()} : {dbEntry.StartDateTime.Value.ToString()} : {dbEntry.EndDateTime.Value.ToString()} updated", Severity.Info, config => { config.ShowCloseIcon = false; });

                //return timeEntry;
                // set ToggledChanged="SetTimeEntry" op basis van ID

            }
            catch (Exception ex)
            {
                Snackbar.Add($"Time entry failed: {ex.Message}", Severity.Error, config => { config.ShowCloseIcon = true; });
                if (ex.Message == "{\"errors\":[\"This roleID is not valid for this Task and Resource.\"]}")
                {
                    Snackbar.Add($"Time entry failed: {ex.Message}", Severity.Success, config => { config.ShowCloseIcon = true; });

                }
                var parameters = new DialogParameters();
                parameters.Add("NewTimeEntry", dbEntry);
                parameters.Add("Errors", ex.Message);
                var options = new DialogOptions() { CloseButton = false, MaxWidth = MaxWidth.ExtraExtraLarge };

                MudDialog.Show<TimeEntryEdit>("Delete", parameters, options);

                //return dbEntry;
            }
        }
        public void ShowCommentField()
        {
            ShowComment = !ShowComment;

        }

    }

    //public async Task<TimeEntry> SetTimeEntry(bool toggled)
    //    {
    //        try
    //        {
                

    //            // Deze hele functie moet naar de service
    //            // timeEntryService.ActiveTimeEntry
    //            EntrySynced = toggled;


                
    //           // var hours = ((DateTime)dbEntry.EndDateTime).Subtract(((DateTime)dbEntry.StartDateTime)).Hours;
               
    //            //timeEntryService.UpdateActiveTimeEntry() = apiEntry; // RPS is nog niet volledig

    //            var timeEntry = await Synlogic.Autotask.Controllers.TimeEntryController.CreateAsync(dbEntry);
    //            Snackbar.Add($"{dbEntry.Id.ToString()} : {dbEntry.StartDateTime.Value.ToString()} : {dbEntry.EndDateTime.Value.ToString()} updated", Severity.Info, config => { config.ShowCloseIcon = false; });

    //            return timeEntry;
    //            // set ToggledChanged="SetTimeEntry" op basis van ID

    //        }
    //        catch (Exception ex)
    //        {
    //            Snackbar.Add($"Time entry failed: {ex.Message}", Severity.Error, config => { config.ShowCloseIcon = true; });
    //            if (ex.Message == "{\"errors\":[\"This roleID is not valid for this Task and Resource.\"]}")
    //            {
    //                Snackbar.Add($"Time entry failed: {ex.Message}", Severity.Success, config => { config.ShowCloseIcon = true; });

    //            }
    //            var parameters = new DialogParameters();
    //            parameters.Add("NewTimeEntry", dbEntry);
    //            parameters.Add("Errors", ex.Message);
    //            var options = new DialogOptions() { CloseButton = false, MaxWidth = MaxWidth.ExtraExtraLarge };

    //            MudDialog.Show<TimeEntryEdit>("Delete", parameters, options);

    //            return dbEntry;
    //        }
    //    }
    //    public void ShowCommentField()
    //    {
    //        ShowComment = !ShowComment;

    //    }

    //}
}
