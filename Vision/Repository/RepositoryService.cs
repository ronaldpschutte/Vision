using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using Vision.Models;
using Vision.Services;
using Microsoft.AspNetCore.SignalR.Client;

namespace Vision.Repository
{
    public partial class RepositoryService //: IUpdatableService
    {

        public event EventHandler<CustomPropertyChangedEventArgs> PropertyChangedInRepo;

        [Inject]
        private IDbContextFactory<SynscrumxxlDevSqlContext> _contextFactory { get; set; }
        private EventGridService _eventGridService { get; set; }
        private HubConnection? hubConnection;
        private List<string> messages = new List<string>();

        [Inject]
        private NavigationManager navigationManager { get; set; }

        public bool FinishedLoading = false;

        public List<Sprint> Sprints { get; set; }

        public Guid RepoGuid { get; set; }

        public RepositoryService(IDbContextFactory<SynscrumxxlDevSqlContext> contextFactory, EventGridService eventGridService)
        {

            _contextFactory = contextFactory;
            _eventGridService = eventGridService;


            hubConnection = new HubConnectionBuilder()
            .WithUrl(navigationManager.ToAbsoluteUri("/sync"))
            .Build();

            hubConnection.On<string, string>("ReceiveMessage", (user, message) =>
            {
                var encodedMsg = $"{user}: {message}";
                messages.Add(encodedMsg);
            });

            Initialize();
        }


        public void Initialize()
        {
            using (var ctx = _contextFactory.CreateDbContext())
            {
                foreach (var sprint in ctx.Sprint)
                {
                    Sprints.Add(sprint);
                }
            }


            
          //  Task.Run(() => hubConnection.StartAsync());

            FinishedLoading = true;
        }

    }
}
