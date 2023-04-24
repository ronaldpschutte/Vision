using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Vision.Models;
using Vision.Services;

namespace Vision.Components
{
    public partial class WorkFlowStepCard
    {
        [Parameter]
        public WorkflowStep Item { get; set; }
        [Inject]
        IJSRuntime JSRuntime { get; set; }
        [Inject]
        private DataContextProvider contextProvider { get; set; }
        [Inject]
        private PersonalContextProvider personalContext { get; set; }


        [Parameter]
        public EventCallback<WorkflowStep> ItemChanged { get; set; }
        

        [Parameter]
        public bool IsEdit { get; set; }
        [Parameter]
        public EventCallback<bool> IsEditChanged { get; set; }

        private async Task ChangeValue()
        {
            await ItemChanged.InvokeAsync(Item);
        }
        private void OpenAutotask()
        {
            JSRuntime.InvokeVoidAsync("OpenUrlInNewTab", $"https://ww4.autotask.net/Mvc/ServiceDesk/TicketDetail.mvc?ticketId={Item.WorkflowStepId}");
        }
        private string GetInitials(string firstname, string lastname)
        {
            // StringSplitOptions.RemoveEmptyEntries excludes empty spaces returned by the Split method
            string name = $"Lars van der Voort";
            //string name = $"{firstname} {lastname}";
            string[] nameSplit = name.Split(new string[] { ",", " " }, StringSplitOptions.RemoveEmptyEntries);

            string initials = "";

            foreach (string item in nameSplit)
            {
                initials += item.Substring(0, 1);
            }

            if (nameSplit.Length > 2)
            {
                initials = "";
                initials += nameSplit.First().Substring(0, 1) + nameSplit.Last().Substring(0, 1);
            }
            return initials;
        }
        private async Task ToggleEdit()
        {
            IsEdit = !IsEdit;
            if (IsEdit == false)
            {
                await contextProvider.Changed("WorkflowStep", Item.WorkflowStepId, personalContext.MyGuid);

            }
            await IsEditChanged.InvokeAsync(IsEdit);
        }

        /// <summary>
        /// Get all teamMemberRoles to select (excluding inactive and API users)
        /// </summary>
        /// <returns></returns>
        private List<TeamMemberRole> GetTeamMemberRoles()
        {
            var list = contextProvider.TeamMemberRoles.GroupBy(t => new { t.TeamMemberId, t.RoleId }).Select(grp => grp.First());
            var users = list.Where(x => x.TeamMember.UserType != 38 && x.TeamMember.UserType != 24 && x.TeamMember.UserType != 12);
            var activeItems = users.Where(x => x.TeamMember.IsActive == true && x.Role.IsActive == true && x.IsActive == true);
            var orderedItems = activeItems.OrderBy(x => x.TeamMemberId).ThenBy(x => x.RoleId).ToList();
            return orderedItems;
        }

        /// <summary>
        /// Get the value of the primary assigned resource
        /// </summary>
        /// <returns></returns>
        private long? GetPrimaryValue()
        {
            if (Item.AssignedTeamMemberId != null && Item.AssignedRoleId != null)
            {
                var list = GetTeamMemberRoles();
                TeamMemberRole? assigned = list.SingleOrDefault(x => x.RoleId == Item.AssignedRoleId && x.TeamMemberId == Item.AssignedTeamMemberId);
                return assigned.TeamMemberRoleId;
            }
            return null;
        }

        /// <summary>
        /// Save the new value of the primary assigned resource 
        /// </summary>
        /// <param name="value"></param>
        private async Task PrimaryValueChanged(long? value)
        {
            if (value == null)
            {
                Item.AssignedTeamMemberId = null;
                Item.AssignedTeamMember = null;
                Item.AssignedRoleId = null;
                Item.AssignedRole = null;
            }
            else
            {
                var selectedTeamMemberRole = contextProvider.TeamMemberRoles.SingleOrDefault(x => x.TeamMemberRoleId == value);

                Item.AssignedTeamMemberId = selectedTeamMemberRole.TeamMemberId;
                Item.AssignedTeamMember = selectedTeamMemberRole.TeamMember;
                Item.AssignedRoleId = selectedTeamMemberRole.RoleId;
                Item.AssignedRole = selectedTeamMemberRole.Role;
            }
            await contextProvider.Changed("WorkflowStep", Item.WorkflowStepId, personalContext.MyGuid);
        }

        /// <summary>
        /// Get the values of the secondary assigned resources
        /// </summary>
        /// <returns></returns>
        private List<long?> GetSecondaryValues()
        {
            return GetSecondaryValuesItems().Select(x => (long?)x.TeamMemberRoleId).ToList();
        }

        /// <summary>
        /// Get the values of the secondary assigned resources as items
        /// </summary>
        /// <returns></returns>
        private List<TeamMemberRole> GetSecondaryValuesItems()
        {
            if (Item.WorkflowStep2TeamMember.Any())
            {
                var list = GetTeamMemberRoles();
                //var test = list.Where(x => x.);
                List<TeamMemberRole> valuesTest = new List<TeamMemberRole>();
                foreach (var item in Item.WorkflowStep2TeamMember)
                {
                    valuesTest.Add(list.SingleOrDefault(x => x.RoleId == item.RoleId && x.TeamMemberId == item.TeamMemberId));
                }
                //return assigned;
                return valuesTest;
            }
            return new List<TeamMemberRole>();
        }

        /// <summary>
        /// Save the new values of the secondary assigned resources
        /// </summary>
        /// <param name="value"></param>
        private void SecondaryValuesChanged(IEnumerable<long?> value)
        {
            // remove items not in value list
            var tmrs = GetTeamMemberRoles().Where(x => value.Contains((long?)x.TeamMemberRoleId)).ToList();

            // remove local vars
            List<WorkflowStep2TeamMember>? items = Item.WorkflowStep2TeamMember.Where(x => !tmrs.Any(y => y.TeamMemberId == x.TeamMemberId && y.RoleId == x.RoleId)).ToList();
            foreach (var item in items)
            {
                Item.WorkflowStep2TeamMember.Remove(item);
            }

            // remove db
            items = contextProvider.WorkflowSteps2TeamMembers.Where(x => !tmrs.Any(y => y.TeamMemberId == x.TeamMemberId && y.RoleId == x.RoleId && x.WorkflowStepId == Item.WorkflowStepId)).ToList();
            foreach (var item in items)
            {
                contextProvider.WorkflowSteps2TeamMembers.Remove(item);
            }


            // add items in value list and not in item list
            tmrs = GetTeamMemberRoles().Where(x => value.Contains((long?)x.TeamMemberRoleId)).ToList();

            // add local vars
            List<TeamMemberRole>? addedItems = tmrs.Where(x => !Item.WorkflowStep2TeamMember.ToList().Any(y => y.TeamMemberId == x.TeamMemberId && y.RoleId == x.RoleId)).ToList();
            foreach (var item in addedItems)
            {
                var addedItem = new WorkflowStep2TeamMember()
                {
                    RoleId = item.RoleId,
                    TeamMemberId = item.TeamMemberId,
                    Role = item.Role,
                    TeamMember = item.TeamMember,
                    WorkflowStepId = Item.WorkflowStepId
                };
                Item.WorkflowStep2TeamMember.Add(addedItem);
            }

            // add db
            addedItems = tmrs.Where(x => !contextProvider.WorkflowSteps2TeamMembers.ToList().Any(y => y.TeamMemberId == x.TeamMemberId && y.RoleId == x.RoleId && y.WorkflowStepId == Item.WorkflowStepId)).ToList();
            foreach (var item in addedItems)
            {
                var addedItem = new WorkflowStep2TeamMember()
                {
                    RoleId = item.RoleId,
                    TeamMemberId = item.TeamMemberId,
                    Role = item.Role,
                    TeamMember = item.TeamMember,
                    WorkflowStepId = Item.WorkflowStepId
                };
                contextProvider.WorkflowSteps2TeamMembers.Add(addedItem);
            }
            contextProvider.Changed("WorkflowStep", Item.WorkflowStepId, personalContext.MyGuid);
        }
    }
}
