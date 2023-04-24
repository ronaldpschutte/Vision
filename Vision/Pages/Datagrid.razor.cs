using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Vision.Services;
using Vision.Models;
using Vision.Helpers;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using Vision.Services;

namespace Vision.Pages
{
    public partial class Datagrid
    {

        [Inject]
        public IJSRuntime JSRuntime { get; set; }

        [Inject]
        private DataContextProvider contextProvider { get; set; }
        [Inject]
        private PersonalContextProvider personalContext { get; set; }
        private string ErrorMessage = "";

        public int aantalBackLogItems = 0;
    //    private MudTable<Vision.Models.WorkflowStep> _mudTable { get; set; }
        private MudDataGrid<Vision.Models.WorkflowStep> _mudTable { get; set; }

        private Vision.Models.WorkflowStep _selectedItem = null;

        public List<TeamMemberSelection> ActiveTeamMembers = new List<TeamMemberSelection>();
        private List<Vision.Models.WorkflowStep> CurrectSprintSteps { get; set; }

        private List<string> Statusses = new List<string>();
        protected async override System.Threading.Tasks.Task OnInitializedAsync()
        {
            try
            {
                Statusses.Add("New");
                Statusses.Add("In Progress");
                Statusses.Add("Hold");
                Statusses.Add("Complete");

                CurrectSprintSteps = contextProvider.WorkflowSteps.Where(w => w.SprintId == 0 && w.AutotaskStatusId != contextProvider.AutoTaskStatuses.SingleOrDefault(s => s.AutoTaskStatusText == "To be planned").AutoTaskStatusId).ToList();
                ActiveTeamMembers = TeamMemberSelection.ToSelectList(contextProvider.TeamMembers.Where(t => t.IsActive == true).ToList());

                contextProvider.WorkflowStepsUpdated += _dataService_WorkflowStepsUpdated;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void _dataService_WorkflowStepsUpdated(object? sender, EventArgs e)
        {
            Refresh().GetAwaiter().GetResult();
        }
        private async Task ItemHasBeenCommitted(object element)
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
