﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Vision.Models
{
    public partial class AutoTaskStatus
    {
        public AutoTaskStatus()
        {
            ScrumStatusNavigation = new HashSet<ScrumStatus>();
            WorkflowStep = new HashSet<WorkflowStep>();
        }

        public int AutoTaskStatusId { get; set; }
        public string AutoTaskStatusText { get; set; }
        public int ScrumStatusId { get; set; }
        public int OrderIndex { get; set; }

        public virtual ScrumStatus ScrumStatus { get; set; }
        public virtual ICollection<ScrumStatus> ScrumStatusNavigation { get; set; }
        public virtual ICollection<WorkflowStep> WorkflowStep { get; set; }
    }
}