using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

using Vision.Models;

namespace Vision.Services
{
     
    public class PersonalContextProvider
    {
        private readonly AuthenticationStateProvider _authProvider;
        private readonly DataContextProvider _contextProvider;
        private readonly ILogger<PersonalContextProvider> _log;
        public Guid MyGuid { get; set; } = Guid.NewGuid();
        public int MyId { get; set; } = 0;
        public string MyEmail { get; set; }
        public TeamMember MyTeamMember { get; set; }
        public List<Role> MyRoles { get; set; } = new List<Role>();
        public List<TeamMemberRole> MyTeamMemberRoles { get; set; }
        public List<Project> MyProjects = new List<Project>();
        public List<Queue> MyQueues = new List<Queue>();
        public List<Department> MyDepartments = new List<Department>();

        public List<WorkflowStep> MyWorkFlowSteps = new List<WorkflowStep>();

        public List<WorkflowStep> MyTeamWorkFlowSteps = new List<WorkflowStep>();
        public PersonalContextProvider(DataContextProvider contextProvider, AuthenticationStateProvider authProvider, ILogger<PersonalContextProvider> log)
        {
            _authProvider = authProvider;
            _contextProvider = contextProvider;
            _log = log;
            GetUserEmailFromLogin();
            GetUserFromDb();

            MyWorkFlowSteps = _contextProvider.WorkflowSteps.Where(w => w.AssignedTeamMemberId == MyId).ToList();
            // MyTeamWorkFlowSteps = _contextProvider.WorkflowSteps.Where(w => w.) Forumpost gedaan op DATTO om dit te achterhalen.
            MyProjects = MyWorkFlowSteps.GroupBy(w => w.ProjectId).Select(p => p.FirstOrDefault().Project).ToList();
            
        }
        private async void GetUserEmailFromLogin()
        {
            var authState = await _authProvider.GetAuthenticationStateAsync();
            ClaimsPrincipal user = authState.User;
            
            MyEmail = user.Claims.Where(c => c.Type == "preferred_username").FirstOrDefault()?.Value;
            
        }
        private void GetUserFromDb()
        {
            MyTeamMember = _contextProvider.TeamMembers.Where(u => u.Email == MyEmail).FirstOrDefault();
            MyId = MyTeamMember.TeamMemberId;
            MyTeamMemberRoles = MyTeamMember.TeamMemberRole.ToList();
            foreach (var role in MyTeamMemberRoles)
            {
                MyRoles.Add(role.Role);
            }
            foreach(var dept in MyTeamMemberRoles.Where(t => t.Department != null).Distinct())
            {
                MyDepartments.Add(dept.Department);
            }
            MyDepartments = MyDepartments.Distinct().ToList();
            _log.LogInformation($"{MyTeamMember.TeamMemberId} - {MyTeamMember.UserName}");
        }
    }
}
