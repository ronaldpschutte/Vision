﻿using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Vision.Services;
using Vision.Models;
using Vision.Helpers;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using Vision.Services;

namespace Vision.Pages
{
    public partial class Tickets
    {

        [Inject]
        public IJSRuntime JSRuntime { get; set; }

        [Inject]
        private DataContextProvider contextProvider { get; set; }
        [Inject]
        private PersonalContextProvider personalContext { get; set; }

        private string ErrorMessage = "";

        public int aantalBackLogItems = 0;
        private MudSelect<string> mudSelect { get; set; }

        private string searchString = "";
        private MudTable<Vision.Models.WorkflowStep> _mudTable { get; set; }
        private Vision.Models.WorkflowStep _selectedItem = null;
        private TableGroupDefinition<WorkflowStep> tableGroupDefinition { get; set; }
        public List<TeamMemberSelection> ActiveTeamMembers = new List<TeamMemberSelection>();
        private List<Vision.Models.WorkflowStep> CurrentSprintSteps { get; set; }

        private List<string> Statusses = new List<string>();
        protected async override System.Threading.Tasks.Task OnInitializedAsync()
        {
            try
            {
                Statusses.Add("New");
                Statusses.Add("In Progress");
                Statusses.Add("Hold");
                Statusses.Add("Complete");

                CurrentSprintSteps = contextProvider.WorkflowSteps.Where(w => w.StepType == 1 && w.AutotaskStatusId != contextProvider.AutoTaskStatuses.SingleOrDefault(s=>s.AutoTaskStatusText== "Complete").AutoTaskStatusId).ToList();
                ActiveTeamMembers = TeamMemberSelection.ToSelectList(contextProvider.TeamMembers.Where(t => t.IsActive == true).ToList());

                contextProvider.WorkflowStepsUpdated += _dataService_WorkflowStepsUpdated;
            }
            catch (Exception)
            {
                throw;
            }
        }
        //protected  async override System.Threading.Tasks.Task OnAfterRenderAsync(bool firstRender)
        //{

        //}
        private void GroupChanged(string e)
        {
                if (e.ToString() == "Company") { tableGroupDefinition = CompanyGroup; }
                else if (e.ToString() == "Queue") { tableGroupDefinition = QueueGroup; }
                else { tableGroupDefinition = null; }
            StateHasChanged();
         
        }
        private bool FilterFunc(WorkflowStep element)
        {
            if (string.IsNullOrWhiteSpace(searchString))
                return true;
            if (element.Company.CompanyNumber.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (element.CompanyName.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (element.Project != null && element.Project.ProjectNumber.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (element.Project != null && element.ProjectName.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (element.Phase != null && element.Phase.PhaseNumber != null && element.Phase.PhaseNumber.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (element.Phase != null && element.PhaseName != null && element.PhaseName.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (element.AutotaskNumber.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (element.Title.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            return false;
        }

        private TableGroupDefinition<WorkflowStep> CompanyGroup = new TableGroupDefinition<WorkflowStep>()
        {

            GroupName = "Company",
            Indentation = true,
            Expandable = true,
            Selector = (e) => e.Company.CompanyNumber + "|" + e.CompanyName,
            
        };
        private TableGroupDefinition<WorkflowStep> QueueGroup = new TableGroupDefinition<WorkflowStep>()
        {


                GroupName = "Queue",
                Expandable = true,
                Selector = (e) => e.QueueId,

                InnerGroup = new TableGroupDefinition<WorkflowStep>()
                {
                    GroupName = "Company",
                    Expandable = true,
                    Selector = (e) => e.Company.CompanyNumber + "|" + e.CompanyName
                }
        };

        private void _dataService_WorkflowStepsUpdated(object? sender, EventArgs e)
        {
            Refresh().GetAwaiter().GetResult();
        }
        private void ItemHasBeenCommitted(object element)
        {
            if (element.GetType() == typeof(WorkflowStep))
            {
                var wfs = (WorkflowStep)element;
                contextProvider.Changed("WorkflowStep", wfs.WorkflowStepId, personalContext.MyGuid);
            }
        }

        private async Task Refresh()
        {
            await _mudTable.ReloadServerData();
            await InvokeAsync(StateHasChanged);
        }

        //public void OnContextMenuClick(EventArgs<vwWorkflowStep> args)
        //{
        //    // https://ww[n].autotask.net/
        //    string webUrl = AutoTaskCredentials.GetWebUrl();

        //    switch (args.Item.Id)
        //    {
        //        case "Go2Customer":
        //            var urlString = $"{webUrl}Mvc/CRM/AccountDetail.mvc?AccountId={args.RowInfo.RowData.CompanyId}";
        //            JSRuntime.InvokeVoidAsync("OpenUrlInNewTab", $"{webUrl}Mvc/CRM/AccountDetail.mvc?AccountId={args.RowInfo.RowData.CompanyId}");
        //            break;
        //        case "Go2Project":
        //            JSRuntime.InvokeVoidAsync("OpenUrlInNewTab", $"{webUrl}Mvc/Projects/ProjectDetail.mvc/ProjectDetail?gridConfiguration=Outline&initialContentPage=Summary&projectId={args.RowInfo.RowData.ProjectId}");
        //            break;
        //        case "Go2Ticket":
        //            JSRuntime.InvokeVoidAsync("OpenUrlInNewTab", $"{webUrl}Mvc/ServiceDesk/TicketDetail.mvc?ticketId={args.RowInfo.RowData.WorkflowStepId}");
        //            break;
        //        case "Go2Task":
        //            JSRuntime.InvokeVoidAsync("OpenUrlInNewTab", $"{webUrl}Mvc/Projects/TaskDetail.mvc?taskId={args.RowInfo.RowData.WorkflowStepId}");
        //            break;
        //        case "Move2Top":
        //            Move2Top(args.RowInfo.RowData);
        //            this.ToastObj.ShowAsync(new ToastModel { Title = $"Move2Active: {args.RowInfo.RowData.AutotaskNumber} : {args.RowInfo.RowData.Title}", CssClass = "e-toast-info", Icon = "e-info toast-icons" });
        //            break;
        //        case "Move2Active":
        //            Move2Active(args.RowInfo.RowData);
        //            this.ToastObj.ShowAsync(new ToastModel { Title = $"Move2Active: {args.RowInfo.RowData.AutotaskNumber} : {args.RowInfo.RowData.Title}", CssClass = "e-toast-info", Icon = "e-info toast-icons" });
        //            break;
        //        case "Move2Next":
        //            Move2Next(args.RowInfo.RowData);
        //            this.ToastObj.ShowAsync(new ToastModel { Title = $"Move2Next: {args.RowInfo.RowData.AutotaskNumber} : {args.RowInfo.RowData.Title}", CssClass = "e-toast-info", Icon = "e-info toast-icons" });
        //            break;
        //        case "Move2Backlog":
        //            Move2Backlog(args.RowInfo.RowData);
        //            this.ToastObj.ShowAsync(new ToastModel { Title = $"Move2Backlog: {args.RowInfo.RowData.AutotaskNumber} : {args.RowInfo.RowData.Title}", CssClass = "e-toast-info", Icon = "e-info toast-icons" });
        //            break;
        //        default:
        //            break;
        //    }

        //}
    }
}
