using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using System.Linq;
using Vision.Models;
using Vision.Services;
using Vision.Helpers;

namespace Vision.Pages
{
    public partial class Personal
    {

        [Inject]
        private ISnackbar Snackbar { get; set; }
        [Inject]
        private DataContextProvider contextProvider { get; set; }

        [Inject]
        public NavigationManager _navManager { get; set; }
        [Inject]
        private NavigationManager navigationManager { get; set; }

        [Inject]
        private EventGridService eventGridService { get; set; }
        [Inject]
        private PersonalContextProvider personalContext { get; set; }
        [Inject]
        private ILogger<Personal> log { get; set; }

        [Inject]
        public IJSRuntime? JSRuntime { get; set; }

        private List<ScrumStatus> Statusses { get; set; }

        private bool Loading { get; set; } = false;
        private Dictionary<int, bool> FullyExpanded { get; set; } = new Dictionary<int, bool>();
        private Dictionary<string, bool> FullyExpandedType { get; set; } = new Dictionary<string, bool>();
        private Dictionary<int, bool> FullyExpandedProjects { get; set; } = new Dictionary<int, bool>();
        private MudDropContainer<Vision.Models.WorkflowStep> _mudDropContainer { get; set; }
        private List<Company> Companies = new List<Company>();
        private List<Project> Projects = new List<Project>();
        //private List<TeamMember> ActiveTeamMembers = new();
        private List<WorkflowStep> CurrectSprintSteps { get; set; }
        private Dictionary<long, bool> IsEditCards { get; set; } = new Dictionary<long, bool>();

        protected async override System.Threading.Tasks.Task OnInitializedAsync()
        {
            try
            {
                Statusses = contextProvider.ScrumStatuses;

                // get logged in User
                var User = new User();
                User.Email = personalContext.MyEmail;


                // Construct swimlane collection

                LoadData();
                //var Swimlanes = Projects.Select(p => p.ProjectId).Intersect(CurrectSprintSteps.Where(w => w.ProjectId != null).Select(w => w.ProjectId).ToList());

                contextProvider.WorkflowStepsUpdated += _dataService_WorkflowStepsUpdated;
            }
            catch (Exception ex)
            {
                log.LogError(ex.InnerException.Message);
            }
        }
        private void LoadData()
        {
            //Snackbar.Add($"load start", Severity.Success, config => { config.ShowCloseIcon = false; });
            CurrectSprintSteps = contextProvider.WorkflowSteps.Where(w => w.SprintId == contextProvider.Current.SprintId && w.AssignedTeamMemberId == personalContext.MyId).ToList();
            // AutoTaskStatus status = CurrectSprintSteps.Where(w => w.WorkflowStepId == 251608).FirstOrDefault().AutotaskStatus; // RPS Tijdelijk test record status
            Companies = CurrectSprintSteps.Select(x => x.Company).DistinctBy(x => x.CompanyId).ToList();
            Projects = CurrectSprintSteps.Where(x => x.Project != null).Select(x => x.Project).DistinctBy(x => x.ProjectId).ToList();
            //Snackbar.Add($"load end", Severity.Success, config => { config.ShowCloseIcon = false; });
        }
        void OnError(Exception ex)
        {
            log.LogError(ex.Message, ex);
            if (ex is NullReferenceException)
            {
                _navManager.NavigateTo(_navManager.Uri, true);
            }
        }

        private void _dataService_WorkflowStepsUpdated(object? sender, EventArgs e)
        {
            //Snackbar.Add($"_dataService_WorkflowStepsUpdated start", Severity.Success, config => { config.ShowCloseIcon = false; });
            var args = (e as ObjectChangedEventArgs);
            if (args.PersonalGuid != personalContext.MyGuid)
            {
                Snackbar.Add($"{args.ChangeKey} updated", Severity.Info, config => { config.ShowCloseIcon = false; });
                LoadData();
                Refresh().GetAwaiter().GetResult();

            }
            else
            {
                // this is mine
                Snackbar.Add($"{args.ChangeKey} updated", Severity.Warning, config => { config.ShowCloseIcon = false; });
            }

            //Snackbar.Add($"_dataService_WorkflowStepsUpdated end", Severity.Success, config => { config.ShowCloseIcon = false; });
        }

        private async Task Refresh()
        {
            //Snackbar.Add($"Refresh start", Severity.Success, config => { config.ShowCloseIcon = false; });
            _mudDropContainer.Refresh();
            //await _mudTable.ReloadServerData();
            await InvokeAsync(StateHasChanged);
            //Snackbar.Add($"Refresh end", Severity.Success, config => { config.ShowCloseIcon = false; });
        }

        private async Task ItemHasBeenCommitted(object element)
        {
            if (element.GetType() == typeof(WorkflowStep))
            {
                var wfs = (WorkflowStep)element;
                await contextProvider.Changed("WorkflowStep", wfs.WorkflowStepId, personalContext.MyGuid);
            }
        }

        private async void ItemUpdated(MudItemDropInfo<Vision.Models.WorkflowStep> dropItem)
        {
            //Snackbar.Add( $"ItemUpdated start", Severity .Success,config => { config.ShowCloseIcon = false; });
            StateHasChanged();
            try
            {
                int scrumStatusId = Int32.Parse(dropItem.DropzoneIdentifier.Split("_").First());
                dropItem.Item.AutotaskStatus = contextProvider.AutoTaskStatuses.SingleOrDefault(s => s.ScrumStatus.ScrumStatusId == scrumStatusId && s.ScrumStatus.AutoTaskDefaultStatus == s.AutoTaskStatusId);
                dropItem.Item.AutotaskStatusId = dropItem.Item.AutotaskStatus.AutoTaskStatusId;
                //dropItem.Item.AutotaskStatus.AutoTaskStatusId = (int)contextProvider.ScrumStatuses.Where(s => s.ScrumStatusId == dropItem.Item.AutotaskStatusId).FirstOrDefault().AutoTaskDefaultStatusNavigation.AutoTaskStatusId;
                // aanpassen dropzoneidentifiers
                //dropItem.Item.CompanyId = int.Parse(dropItem.DropzoneIdentifier.Split("_")[1]);
                //var item = contextProvider.WorkflowSteps.Where(x => x.WorkflowStepId == dropItem.Item.WorkflowStepId);
                //Snackbar.Add( $"contextProvider.Changed start", Severity .Success,config => { config.ShowCloseIcon = false; });
                Task.Run(() => contextProvider.Changed("WorkflowStep", dropItem.Item.WorkflowStepId, personalContext.MyGuid));
                //Snackbar.Add( $"contextProvider.Changed end", Severity .Success,config => { config.ShowCloseIcon = false; });
            }
            catch (Exception ex)
            {
                log.LogError(ex.Message, ex, dropItem);
                throw (ex);
            }
            //Snackbar.Add($"ItemUpdated end", Severity.Success, config => { config.ShowCloseIcon = false; });

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
