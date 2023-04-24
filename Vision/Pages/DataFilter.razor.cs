

using Microsoft.AspNetCore.Components;
using Vision.Models;
using Vision.Services;

namespace Vision.Pages
{
    public partial class DataFilter
    {
        [Inject]
        public DataContextProvider dataContextProvider { get; set; }
        [Inject]
        private PersonalContextProvider personalContext { get; set; }

        List<UserFilter> userFilters { get; set; }
        List<int> SelectedProjects { get; set; } = new List<int>();
        private int value { get; set; } = 0;
        protected async override System.Threading.Tasks.Task OnInitializedAsync()
        {
            try
            {

            }
            catch (Exception)
            {
                throw;
            }
        }
        private bool FilterFunc(WorkflowStep element)
        {
            //if (string.IsNullOrWhiteSpace(searchString))
            //    return true;
            //if (element.Sign.Contains(searchString, StringComparison.OrdinalIgnoreCase))
            //    return true;
            //if (element.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase))
            //    return true;
            //if ($"{element.Number} {element.Position} {element.Molar}".Contains(searchString))
            //    return true;
            return false;
        }
        // Func<int, string> converter = p => p.ProjectId.ToString();
    }


}
