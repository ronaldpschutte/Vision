using Microsoft.AspNetCore.Components;
using Vision.Models;
using Vision.Services;

namespace Vision.Components
{
    public partial class BillingCodeBar
    {
        [Inject]
        private DataContextProvider contextProvider { get; set; }

        [Inject]
        private ILogger<BillingCodeBar> log { get; set; }

        private List<BillingCode> billingCodes { get; set; } = new List<BillingCode>();
        protected async override System.Threading.Tasks.Task OnInitializedAsync()
        {
            try
            {
                billingCodes = contextProvider.BillingCodes.ToList();
            }
            catch (Exception ex)
            {
                log.LogError(ex.Message, ex);
            }
        }
    }
}
