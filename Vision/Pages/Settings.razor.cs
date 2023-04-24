using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.ComponentModel;
using Vision.Services;
using Microsoft.AspNetCore.SignalR.Client;
using Vision.Models;
using Vision.Services;

namespace Vision.Pages
{
    public partial class Settings : IAsyncDisposable
    {

        [Inject]
        private NavigationManager navigationManager { get; set; }

        [Inject]
        private DataContextProvider dataContext { get; set; }

        [Inject]
        private ILogger<Settings> log { get; set; }

        [Inject]
        public IJSRuntime? JSRuntime { get; set; }


        private string ToastPosition = "Right";
        private string ToastContent = "Conference Room 01 / Building 135 10:00 AM-10:30 AM";
        private HubConnection? eventConnection;
        private List<string> messages = new List<string>();
        

        private bool Updated = false;
    

        public bool IsConnected => eventConnection?.State == HubConnectionState.Connected;



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

        private void SendOnSingleton()
        {
            try
            {
                //if (repositoryService != null)
                //{
                //   repositoryService.Upsert(Object)l
                //    StateHasChanged();
                //}
            }
            catch (Exception)
            {

                throw;
            }
        }
        private void SendOnSingleR()
        {
            try
            {
                //Get SignalR service
            }
            catch (Exception)
            {

                throw;
            }
        }
        private void SendOnEventGrid()
        {
            try
            {
                // Get EventGridService
            }
            catch (Exception)
            {

                throw;
            }
        }
        private void ResetUpdated()
        {
            Updated = false;
            StateHasChanged();
        }

        private async Task someSettingChanged(object sender, CustomPropertyChangedEventArgs args)
        {
            try
            {
              
                    await InvokeAsync(() => StateHasChanged());
               
            }
            catch(Exception ex)
            {
                log.LogError(ex.Message, ex);
            }
        }


     
        public async ValueTask DisposeAsync()
        {
            if (eventConnection is not null)
            {
                await eventConnection.DisposeAsync();
            }
        }
    }
}
