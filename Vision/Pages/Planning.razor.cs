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
    public partial class Planning
    {

        [Inject]
        public IJSRuntime JSRuntime { get; set; }

        [Inject]
        public NavigationManager _navManager { get; set; }

        [Inject]
        private DataContextProvider contextProvider { get; set; }
        [Inject]
        private PersonalContextProvider personalContext { get; set; }
        [Inject]
        private ILogger<Planning> log { get; set; }
        public MudDropContainer<WorkflowStep> _mudDropContainer { get; set; }
        private List<WorkflowStep> CurrentPlanningSteps { get; set; }

        private string ErrorMessage = "";

        public int aantalBackLogItems = 0;
        public double dropindex = 0;
        public double fromindex = 0;


        public List<TeamMember> ActiveTeamMembers = new List<TeamMember>();


        protected async override System.Threading.Tasks.Task OnInitializedAsync()
        {
            try
            {
                CurrentPlanningSteps = contextProvider.WorkflowSteps.Where(w => w.StepType == 0 && (w.SprintId == contextProvider.Current.SprintId || w.SprintId == contextProvider.Current.SprintId + 1 || 
                                                            w.AutotaskStatusId == contextProvider.AutoTaskStatuses.SingleOrDefault(s => s.AutoTaskStatusText == "To be planned").AutoTaskStatusId)).ToList().OrderBy(x => x.OrderIndex).ToList();
                contextProvider.WorkflowStepsUpdated += _dataService_WorkflowStepsUpdated;


            }
            catch (Exception ex)
            {
                log.LogError(ex.Message, ex);
            }
        }

        private async Task ItemUpdated(MudItemDropInfo<WorkflowStep> dropItem)
        {
            try
            {
                if (dropItem.DropzoneIdentifier == "0")
                {

                    dropItem.Item.SprintId = 0;
                    dropItem.Item.AutotaskStatusId = contextProvider.AutoTaskStatuses.SingleOrDefault(s => s.AutoTaskStatusText == "To be planned").AutoTaskStatusId;
                }
                else
                {
                    dropItem.Item.SprintId = Int32.Parse(dropItem.DropzoneIdentifier);
                    if (dropItem.Item.AutotaskStatusId == contextProvider.AutoTaskStatuses.SingleOrDefault(s => s.AutoTaskStatusText == "To be planned").AutoTaskStatusId) 
                    { 
                        dropItem.Item.AutotaskStatusId = contextProvider.AutoTaskStatuses.SingleOrDefault(s => s.AutoTaskStatusText == "New").AutoTaskStatusId; 
                    }
                }

                //var existingItemWithSameIndexInZone = contextProvider.WorkflowSteps.FirstOrDefault(x => x.OrderIndex == dropItem.IndexInZone && x.SprintId == dropItem.Item.SprintId);             
                //existingItemWithSameIndexInZone.OrderIndex = dropItem.Item.OrderIndex;
                //var a = CurrentPlanningSteps.Where(x => x.SprintId.ToString() == dropItem.DropzoneIdentifier);
                //var b = _mudDropContainer.Items.FirstOrDefault(x => x.WorkflowStepId == 8419);

                //contextProvider.WorkflowSteps = _mudDropContainer.Items;
                //dropItem.Item.OrderIndex = dropItem.IndexInZone;

                await contextProvider.Changed("WorkflowStep", dropItem.Item.WorkflowStepId, personalContext.MyGuid);
            }
            catch (Exception ex)
            {
                log.LogError(ex.Message, ex);
            }
        }
        private void _dataService_WorkflowStepsUpdated(object? sender, EventArgs e)
        {
            Refresh().GetAwaiter().GetResult();
        }

        private async Task Refresh()
        {
            try
            {
                await InvokeAsync(StateHasChanged);
                _mudDropContainer.Refresh();
            }
            catch (Exception ex)
            {
                log.LogError(ex.Message, ex);
            }
        }
        protected override void OnAfterRender(bool firstRender)
        {
            try
            {
                if (firstRender)
                {
                    //_container.Refresh();
                    base.OnAfterRender(firstRender);
                }
            }
            catch (Exception ex)
            {
                log.LogError(ex.Message, ex);
            }
        }

        void OnError(Exception ex)
        {
            log.LogError(ex.Message, ex);
            if(ex is NullReferenceException)
            {
                _navManager.NavigateTo(_navManager.Uri, true);
            }
        }

        //private async Task TeamMemberChanged(ChangeEventArgs<int?, TeamMember> args, vwWorkflowStep wfs) 
        //{
        //    if (args != null)
        //    {
        //        StateHasChanged();
        //    }
        //}

    }
}
