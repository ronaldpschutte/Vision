using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using Newtonsoft.Json;
using Vision.Models;
using Vision.Services;
using Vision.Services;



namespace Vision.Pages
{
    public partial class Team
    {
        [Inject]
        private DataContextProvider contextProvider { get; set; }
        [Inject]
        private PersonalContextProvider personalContext { get; set; }
        [Inject]
        private NavigationManager navigationManager { get; set; }
        [Inject]
        private ISnackbar Snackbar { get; set; }
        [Inject]
        private EventGridService eventGridService { get; set; }

        [Inject]
        private ILogger<Team> log { get; set; }

        [Inject]
        public IJSRuntime? JSRuntime { get; set; }

        private List<ScrumStatus> Statusses { get; set; }
        private bool Loading { get; set; } = false;
        private Dictionary<int, bool> FullyExpanded { get; set; } = new Dictionary<int, bool>();
        private MudDropContainer<Vision.Models.WorkflowStep> _mudDropContainer { get; set; }
        private List<TeamMember> ActiveTeamMembers = new();
        private List<WorkflowStep> CurrectSprintSteps { get; set; }
        private HashSet<string> SelectedValues { get; set; } = new HashSet<string>();
        private HashSet<string> SelectedValues2 { get; set; } = new HashSet<string>();

        private Dictionary<long, bool> IsEditCards { get; set; } = new Dictionary<long, bool>();
        //private MudTreeView<string> MudTreeView1;
        //private MudTreeView<string> MudTreeView2;
        protected async override System.Threading.Tasks.Task OnInitializedAsync()
        {
            try
            {
                Statusses = contextProvider.ScrumStatuses.ToList();
                var unassigned = new TeamMember() { UserName = "unassigned", TeamMemberId = 0 };
                ActiveTeamMembers.Add(unassigned);
                foreach (var teamMember in contextProvider.TeamMembers.Where(t => t.IsActive == true))
                {
                    ActiveTeamMembers.Add(teamMember);
                }
                await LoadDataAsync();
                contextProvider.WorkflowStepsUpdated += _dataService_WorkflowStepsUpdated;
            }
            catch (Exception)
            {
                throw;
            }
        }
        private async Task LoadDataAsync()
        {
            // CurrectSprintSteps = contextProvider.WorkflowSteps.Where(w => w.SprintId == contextProvider.Current.SprintId && !(w.AutotaskStatusId == 5 && w.ChangedDateTime < DateTime.Now.AddDays(-1))).ToList();

            CurrectSprintSteps = contextProvider.WorkflowSteps.Where(w => w.SprintId == contextProvider.Current.SprintId).ToList();
        }

        private void _dataService_WorkflowStepsUpdated(object? sender, EventArgs e)
        {

            InvokeAsync(async () => await LoadDataAsync());
            InvokeAsync(async () => await Refresh());
            
        }

        private async Task Refresh()
        {
            _mudDropContainer.Refresh();
           //await _mudTable.ReloadServerData();
            await InvokeAsync(StateHasChanged);
        }

        private void ItemHasBeenCommitted(object element)
        {
            if (element.GetType() == typeof(WorkflowStep))
            {
                var wfs = (WorkflowStep)element;
                contextProvider.Changed("WorkflowStep", wfs.WorkflowStepId, personalContext.MyGuid);
            }
        }

        private async void ItemUpdated(MudItemDropInfo<Vision.Models.WorkflowStep> dropItem)
        {
            try
            {
                dropItem.Item.AutotaskStatusId =contextProvider.ScrumStatuses.SingleOrDefault(x=>x.ScrumStatusId== Int32.Parse(dropItem.DropzoneIdentifier.Split("_").First())).AutoTaskDefaultStatus;
                //dropItem.Item.AutotaskStatus.AutoTaskStatusId = (int)contextProvider.ScrumStatuses.Where(s => s.ScrumStatusText == dropItem.Item.Status).FirstOrDefault().AutoTaskDefaultStatusNavigation.AutoTaskStatusId;

                //dropItem.Item.AutotaskStatus = contextProvider.AutoTaskStatuses.Where(a => a.ScrumStatus.ScrumStatusId == dropItem.Item.AutotaskStatusId).FirstOrDefault();

                dropItem.Item.AssignedTeamMemberId = int.Parse(dropItem.DropzoneIdentifier.Split("_")[1]);
                await contextProvider.Changed("WorkflowStep", dropItem.Item.WorkflowStepId, personalContext.MyGuid);
            }
            catch (Exception ex)
            {
                log.LogError(ex.Message, ex);
            }
        }

        private bool IsCardInEditMode(int id)
        {
            try
            {
                if (!IsEditCards.ContainsKey(id))
                {
                    IsEditCards.Add(id, false);
                }
                return IsEditCards[id];
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
