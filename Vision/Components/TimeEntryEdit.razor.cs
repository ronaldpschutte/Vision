using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using MudBlazor;
using Synlogic.Autotask;
using Synlogic.Autotask.Controllers;
using Synlogic.Autotask.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Vision.Models;
using Vision.Services;

namespace Vision.Components
{
    public partial class TimeEntryEdit
    {
        [Parameter]
        public TimeEntry NewTimeEntry { get; set; }
        [CascadingParameter]
        public MudDialogInstance MudDialog { get; set; }
        [Parameter]
        public string Errors { get; set; } = "";
        [Inject]
        private DataContextProvider contextProvider { get; set; }
        [Inject]
        public TimeEntryService timeEntryService { get; set; }
        //[Inject]
        //public ITeamMemberService teamMemberService { get; set; }
        [Inject]
        public AuthenticationStateProvider AuthenticationStateProvider { get; set; }
        [Inject]
        private ISnackbar Snackbar { get; set; }
        [Inject]
        public IJSRuntime JSRuntime { get; set; }

        public TimeSpan? tsStart { get; set; }
        public TimeSpan? tsEnd { get; set; }


        private bool CreationDialog { get; set; } = false;
        private List<Synlogic.Autotask.Entities.BillingCode> BillingCodes { get; set; } = new List<Synlogic.Autotask.Entities.BillingCode>();
        private List<Synlogic.Autotask.Entities.Role> Roles { get; set; } = new List<Synlogic.Autotask.Entities.Role>();
        private Models.TeamMember CurrentUser { get; set; }


        protected async override System.Threading.Tasks.Task OnInitializedAsync()
        {
            try
            {
                AuthenticationState authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
                System.Security.Claims.ClaimsPrincipal user = authState.User;
                CurrentUser = contextProvider.TeamMembers.SingleOrDefault(x => x.Email == user.Identity.Name);

                //CreateNewTimeEntry();
                BillingCodes = await BillingCodeController.GetBillingCodesByUseTypeAsync(1);
                Roles = await RoleController.GetRolesByResourceIdAsync(CurrentUser.TeamMemberId);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        private void CreateNewTimeEntry()
        {
            //NewTimeEntry = new TimeEntry();
            //NewTimeEntry.CreatorUserId = CurrentUser.TeamMemberId;
            //NewTimeEntry.ResourceId = CurrentUser.TeamMemberId;
            //if (workflowStep.StepType == 0)
            //{
            //    NewTimeEntry.TimeEntryType = 6;

            //    NewTimeEntry.TaskId = workflowStep.WorkflowStepId;
            //}
            //else if (workflowStep.StepType == 1)
            //{
            //    NewTimeEntry.TimeEntryType = 2;
            //    NewTimeEntry.TicketId = workflowStep.WorkflowStepId;
            //}
        }
        private void DialogOpen(Object args)
        {
            this.CreationDialog = true;
        }
        private void DialogClose(Object args)
        {
            this.CreationDialog = false;
        }

        private async System.Threading.Tasks.Task SaveTimeEntry(Object args)
        {
            // validate input data
            bool valid = true;
            if (NewTimeEntry.BillingCodeID == null)
            {
                Snackbar.Add($"invalid value for: BillingCodeId", Severity.Warning, config => { config.ShowCloseIcon = true; });
                valid = false;
            }
            if (NewTimeEntry.RoleID == 0)
            {
                Snackbar.Add($"invalid value for: RoleId", Severity.Warning, config => { config.ShowCloseIcon = true; });
                valid = false;
            }
            //if (NewTimeEntry.DateWorked == null)
            //{
            //    Snackbar.Add($"invalid value for: DateWorked", Severity.Warning, config => { config.ShowCloseIcon = true; });
            //    valid = false;
            //}
            //if (NewTimeEntry.HoursWorked == null)
            //{
            //    Snackbar.Add($"invalid value for: HoursWorked", Severity.Warning, config => { config.ShowCloseIcon = true; });
            //    valid = false;
            //}
            if (NewTimeEntry.StartDateTime == null)
            {
                Snackbar.Add($"invalid value for: StartDateTime", Severity.Warning, config => { config.ShowCloseIcon = true; });
                valid = false;
            }
            if (NewTimeEntry.EndDateTime == null)
            {
                Snackbar.Add($"invalid value for: EndDateTime", Severity.Warning, config => { config.ShowCloseIcon = true; });
                valid = false;
            }
            //if (NewTimeEntry.OffsetHours == null)
            //{
            //    await this.ToastObj.ShowAsync(new ToastModel { Title = $"invalid value for: OffsetHours", CssClass="e-toast-danger", Icon="e-error toast-icons" });
            //    valid = false;
            //}
            // save Time Entry
            if (valid)
            {
                try
                {
                    //var atCreatedTimeEntry = await TimeEntryController.CreateAsync(Convert(NewTimeEntry));
                    
                    // RPS Moet naar de TimeEntry service
                    
                    // MudDialog.Close(DialogResult.Ok(atCreatedTimeEntry));
                }
                catch (Exception ex)
                {
                    Errors = ex.Message;
                }
                //CloseTimeEntry(args);
            }

        }
        private void CloseTimeEntry(Object args)
        {
            //CreationDialog = false;
            //CreateNewTimeEntry();
            //StateHasChanged();
            MudDialog.Cancel();
        }

        private void OpenTimeEntry()
        {
            JSRuntime.InvokeVoidAsync("OpenUrlInNewTab", GetAtUrl());
        }
        private string GetAtUrl()
        {
            string url = $"{AutoTaskCredentials.GetWebUrl()}Mvc/";

            if (NewTimeEntry.TaskID != null)
            {
                url += $"Projects/TimeEntry.mvc/NewTaskTimeEntryPage?taskId={NewTimeEntry.TaskID}";
            }
            else if (NewTimeEntry.TicketID != null)
            {
                url += $"ServiceDesk/TimeEntry.mvc/NewTicketTimeEntryPage?ticketId={NewTimeEntry.TicketID}";
            }
            else
            {
                url = "";
            }

            return url;
        }
    }
}
