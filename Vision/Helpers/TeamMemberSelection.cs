using Vision.Models;

namespace Vision.Helpers
{
    public class TeamMemberSelection
    {
        public int? TeamMemberId { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public bool? IsActive { get; set; }
        public string Title { get; set; }
        public string UserName { get; set; }
        public int? UserType { get; set; }

        public TeamMemberSelection() { }

        public TeamMemberSelection(TeamMember tm)
        {
            TeamMemberId = tm.TeamMemberId;
            Email = tm.Email;
            FirstName = tm.FirstName;
            LastName = tm.LastName;
            MiddleName = tm.MiddleName;
            IsActive = tm.IsActive;
            Title = tm.Title;
            UserName = tm.UserName;
            UserType = tm.UserType;
        }
        public static List<TeamMemberSelection> ToSelectList(List<TeamMember> tms)
        {
            var teamMembersSelections = new List<TeamMemberSelection>();
            teamMembersSelections.Add(new TeamMemberSelection()
            {
                TeamMemberId = null,
                UserName = "Verwijder TeamMember"
            });
            foreach (var teamMember in tms)
            {
                teamMembersSelections.Add(new TeamMemberSelection(teamMember));
            }
            return teamMembersSelections;
        }
    }
}
