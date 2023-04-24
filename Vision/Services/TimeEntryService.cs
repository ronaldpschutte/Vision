using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using Vision.Helpers;
using Synlogic.Autotask;
using Vision.Models;
using Synlogic.Autotask.Controllers;
using Synlogic.Autotask.Entities;
using System.Data;
using System.Collections.Generic;

namespace Vision.Services
{

    public class TimeEntryService
    {

        private readonly PersonalContextProvider PersonalContext;

        private readonly DataContextProvider DataContext;

        private readonly ILogger<TimeEntryService> log;

        private DateTime datum { get; set; } = DateTime.Now.Date;
        public List<TimeEntry> DayTimeEntries { get; set; } = new List<TimeEntry>();
        public TimeEntry? ActiveTimeEntry { get; set; }
        public int ActiveBillingCodeId = 0;

        public EventHandler<TimeEntryEventArgs> TimeEntriesUpdated;

        public TimeEntryService(DataContextProvider DataContextProvider, PersonalContextProvider PersonalContextProvider, ILogger<TimeEntryService> log)
        {
            DataContext = DataContextProvider;
            PersonalContext = PersonalContextProvider;
            //DayTimeEntries = DataContext.TimeEntries.Where(t => t.StartDateTime.Date.Equals(datum)).ToList();
            
            this.log = log;
        }
        public async System.Threading.Tasks.Task GetTimeEntriesFromAPItoDB()
        {
            var dayTimeEntries = await Synlogic.Autotask.Controllers.TimeEntryController.GetAllTimeEntriesPerResourceOnDateAsync(PersonalContext.MyId, DateTime.Now);
            
            if(dayTimeEntries.Count > 0) { DayTimeEntries.Clear(); }
            
            foreach (var entry in dayTimeEntries)
            {

                if (entry.TimeEntryType == 2)
                {
                    entry.InternalNotes = DataContext.WorkflowSteps.FirstOrDefault(w => w.WorkflowStepId == entry.TicketID).Title;
                }
                if (entry.TimeEntryType == 6)
                {
                    entry.InternalNotes = DataContext.WorkflowSteps.FirstOrDefault(w => w.WorkflowStepId == entry.TaskID).Title;
                }
                if (entry.TimeEntryType == 10)
                {
                    entry.InternalNotes = DataContext.BillingCodes.FirstOrDefault(b => b.BillingCodeId == entry.BillingCodeID).Name;
                }
                
                DayTimeEntries.Add(entry);
            }
        }

        private void EndActiveTimeEntry()
        {

            //ActiveTimeEntry.SummaryNotes = Note;
            ActiveTimeEntry.EndDateTime = DateTime.Now;
            //ActiveTimeEntry.tsEnd = DateTime.Now.TimeOfDay;

            DayTimeEntries.Add(ActiveTimeEntry);
            ActiveTimeEntry = null;

        }

        //public TimeSpan RoundToNearest(this TimeSpan a, TimeSpan roundTo)
        //{
        //    long ticks = (long)(Math.Round(a.Ticks / (double)roundTo.Ticks) * roundTo.Ticks);
        //    return new TimeSpan(ticks);
        //}

        public void UpdateActiveTimeEntry(int BillingCodeId, bool IsStarting, string Title, string Note, int AutotaskType, int WorkflowStepId)
        {
            try
            {
                if (!IsStarting)
                {
                    if (ActiveTimeEntry != null && ActiveTimeEntry.BillingCodeID != BillingCodeId && (ActiveTimeEntry.TaskID != WorkflowStepId && ActiveTimeEntry.TicketID != WorkflowStepId))
                    {
                        ActiveTimeEntry.SummaryNotes = Note;

                        EndActiveTimeEntry();
                    }

                    var newTE = new TimeEntry();
                    switch (AutotaskType)
                    {
                        case 0: { newTE.TaskID = WorkflowStepId; break; }
                        case 1: { newTE.TicketID = WorkflowStepId; break; }
                        default:
                            break;
                    }
                    
                    newTE.InternalNotes = Title;
                    newTE.SummaryNotes = Note;
                    newTE.BillingCodeID = BillingCodeId;
                    newTE.ResourceID = PersonalContext.MyId;
                    newTE.RoleID = PersonalContext.MyRoles.FirstOrDefault().RoleId;
                    ActiveTimeEntry = newTE;
                }
                else
                {
                    ActiveTimeEntry.SummaryNotes = Note;
                    EndActiveTimeEntry();
                }

                TimeEntryEventArgs te = new TimeEntryEventArgs();
                te.BillingCodeId = BillingCodeId;
                te.WorkflowStepId = WorkflowStepId;
                te.IsActive = !IsStarting;
                te.Note = Note;
                TimeEntriesUpdated?.Invoke(this, te);
            }
            catch (Exception ex)
            {
                log.LogError(ex.InnerException.Message);
            }
        }
    }
}
