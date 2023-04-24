using Microsoft.EntityFrameworkCore;
using Vision.Models;
using Vision.Helpers;
using System.Collections.Concurrent;

namespace Vision.Services
{
    public class DataContextProvider
    {
        private IDbContextFactory<SynscrumxxlDevSqlContext> _contextFactory;
        private EventGridService _gridService;
        private ILogger<DataContextProvider> log;
        private Guid ATGuid = Guid.NewGuid();
        //private AutoResetEvent _resetCtx = new AutoResetEvent(false);
        //private long threadCount = 0;

        public EventHandler WorkflowStepsUpdated;

        public Sprint Current;       
        public List<ScrumStatus> ScrumStatuses = new List<ScrumStatus>();
        public List<AutoTaskStatus> AutoTaskStatuses = new List<AutoTaskStatus>();
        public List<WorkflowStep> WorkflowSteps = new List<WorkflowStep>();
        public List<Sprint> Sprints = new List<Sprint>();
        public List<User> Users = new List<User>();
        public List<Project> Projects = new List<Project>();
        public List<Phase> Phases = new List<Phase>();
        public List<TeamMember> TeamMembers = new List<TeamMember>();
        public List<TeamMemberRole> TeamMemberRoles = new List<TeamMemberRole>();
        public List<Role> Roles = new List<Role>();
        public List<Company> Companies = new List<Company>();
        public List<Department> Departments = new List<Department>();
        public List<Queue> Queues = new List<Queue>();
        public List<BillingCode> BillingCodes = new List<BillingCode>();
        public List<WorkflowStep2TeamMember> WorkflowSteps2TeamMembers = new List<WorkflowStep2TeamMember>();


        public DataContextProvider(IDbContextFactory<SynscrumxxlDevSqlContext> _contextDBFactory,EventGridService _eventGridService, ILogger<DataContextProvider> log)
        {
            try
            {
                _gridService = _eventGridService;
                _contextFactory = _contextDBFactory;

                // using new ctx from factory
                using (var _ctx = _contextFactory.CreateDbContext())
                {
                    Sprints = _ctx.Sprint.ToList();
                    Current = Sprints.FirstOrDefault(s => s.Current == true);
                    ScrumStatuses = _ctx.ScrumStatus.AsNoTracking().Include(s => s.AutoTaskStatus).AsNoTracking().ToList();
                    AutoTaskStatuses = _ctx.AutoTaskStatus.AsNoTracking().Include(a => a.ScrumStatus).AsNoTracking().ToList();

                    Departments = _ctx.Department.ToList();
                    Queues = _ctx.Queue.ToList();            
                    TeamMembers = _ctx.TeamMember.Include(t => t.TeamMemberRole).ThenInclude(t => t.Role).ToList();
                    TeamMemberRoles = _ctx.TeamMemberRole.Include(t => t.Role).Include(r => r.Queue).Include(r => r.Department).Include(r => r.TeamMember).ToList();
                    Roles = _ctx.Role.ToList();

                    Projects = _ctx.Project.Include(p => p.Company)
                        .Include(p => p.WorkflowStep).ToList();
                    Companies = _ctx.Company.ToList();
                    WorkflowSteps = _ctx.WorkflowStep.AsNoTracking()
                        .Include(w => w.AssignedTeamMember).AsNoTracking()
                        .Include(x=>x.AssignedRole).AsNoTracking()
                        .Include(w => w.AutotaskStatus).ThenInclude(w => w.ScrumStatus).AsNoTracking()
                        .Include(w => w.Project).AsNoTracking()
                        .Include(w => w.Phase).AsNoTracking()
                        .Include(w => w.Company).AsNoTracking()
                        .Include(x => x.WorkflowStep2TeamMember).ThenInclude(x => x.TeamMember).AsNoTracking()
                        .Include(x => x.WorkflowStep2TeamMember).ThenInclude(x => x.Role).AsNoTracking()
                        .Include(x=> x.Queue).AsNoTracking()
                        .ToList();

                    BillingCodes = _ctx.BillingCode.Where(b => b.IsActive).ToList();
                   // TimeEntries = _ctx.TimeEntry.ToList(); alleen in personalContext opnemen
                }                
            }
            catch (Exception ex)
            {
                log.LogError(ex.InnerException.Message);
            }
        }

        public async Task Changed(string type, int key, Guid myGuid)
        {
            try
            {

                if (type == "WorkflowStep")
                {

                    using (var _ctx = _contextFactory.CreateDbContext())
                    {
                        var memEntity = WorkflowSteps.Where(w => w.WorkflowStepId == key).First();
                        var dbEntity = _ctx.WorkflowStep.AsNoTracking()
                            .Include(w => w.AssignedTeamMember).AsNoTracking()
                            .Include(x => x.AssignedRole).AsNoTracking()
                            .Include(w => w.AutotaskStatus).ThenInclude(w => w.ScrumStatus).AsNoTracking()
                            .Include(w => w.Project).AsNoTracking()
                            .Include(w => w.Phase).AsNoTracking()
                            .Include(w => w.Company).AsNoTracking()
                            .Include(x => x.WorkflowStep2TeamMember).ThenInclude(x => x.TeamMember).AsNoTracking()
                            .Include(x => x.WorkflowStep2TeamMember).ThenInclude(x => x.Role).AsNoTracking()
                            .Include(x => x.Queue).AsNoTracking()
                            .Where(w => w.WorkflowStepId == key).First();

                            _ctx.WorkflowStep.Update(memEntity);
                            await _ctx.SaveChangesAsync();

                        ObjectChangedEventArgs eventArgs = new ObjectChangedEventArgs(type, key, myGuid);
                        WorkflowStepsUpdated?.Invoke(null, eventArgs);

                        _gridService.SendDataSyncedEvent(memEntity);
                        // log.LogInformation("Succes update: " + type + key.ToString() + " in tread# " + threadCount.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                log.LogError(ex.Message);
            }
            finally
            {

            }
        }


        public async Task ExternalUpdate(string type, int key)
        {
            try
            {
                //threadCount++;
                //if(threadCount > 1)
                {
                    //_resetCtx.WaitOne();

                    // using new ctx from factory to update list from new select
                    using (var _ctx = _contextFactory.CreateDbContext())
                    {
                        //Type t = DataSyncedEventType.GetType(type);
                        switch (type)
                        {
                            case "WorkflowStep":

                                var memEntity = WorkflowSteps.Where(w => w.WorkflowStepId == key).First();
                                var dbEntity = _ctx.WorkflowStep.Where(w => w.WorkflowStepId == key).First();

                                if (memEntity == null) 
                                {
                                    WorkflowSteps.Add(dbEntity); 
                                }
                                else
                                {
                                    memEntity = dbEntity;
                                }
                                ObjectChangedEventArgs eventArgs = new ObjectChangedEventArgs("WorkflowStep", key, ATGuid);

                                WorkflowStepsUpdated?.Invoke(null, eventArgs);

                                break;
                            case "Phase":

                                break;
                            case "Project":

                                break;
                            case "TeamMember":

                                break;
                            case "Queue":

                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.LogError(ex.InnerException.Message);
            }
            finally
            {
                //threadCount--;
                //_resetCtx.Set();
            }
        }
    }
}
