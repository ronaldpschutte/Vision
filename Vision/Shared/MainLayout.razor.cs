using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using System.Net.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using Vision;
using Vision.Shared;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;
using Vision.Pages;
using Vision.Models;
using Synlogic.Autotask.Entities;

namespace Vision.Shared
{
    public partial class MainLayout
    {
        bool OpenFilterWnd;
        bool OpenTimeEntyWnd;
        bool OpenBillingCodeWnd;

        private bool DarkMode = true;
        private ErrorBoundary? errorBoundary;
        protected Exception? CurrentException { get; }

        private MudTheme _theme = new();
        private string PageSelected = "Personal";
        
        protected override void OnParametersSet()
        {
             errorBoundary?.Recover();
        }

        private void PageChanged(string pageName)
        {
            PageSelected = pageName;
        }
        void ToggleTimeEntryWnd()
        {
            OpenTimeEntyWnd = !OpenTimeEntyWnd;

        }
        void ToggleBillingCodeWnd()
        {

            OpenBillingCodeWnd = !OpenBillingCodeWnd;

        }
    }
}