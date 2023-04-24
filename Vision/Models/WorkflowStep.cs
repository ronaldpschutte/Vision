﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Vision.Models
{
    public partial class WorkflowStep
    {
        public WorkflowStep()
        {
            WorkflowStep2TeamMember = new HashSet<WorkflowStep2TeamMember>();
        }

        public DateTime? ChangedDateTime { get; set; }
        public int? ChangedBy { get; set; }
        public double? ActualHours { get; set; }
        public double? EstimatedHours { get; set; }
        public string Description { get; set; }
        public int? AssignedTeamMemberId { get; set; }
        public bool Planned { get; set; }
        public bool IsDeleted { get; set; }
        public int? QueueId { get; set; }
        public int? BillingCodeId { get; set; }
        public int? AssignedRoleId { get; set; }
        public int PercentComplete { get; set; }
        public int? Priority { get; set; }

        public virtual Role AssignedRole { get; set; }
        public virtual TeamMember AssignedTeamMember { get; set; }
        public virtual AutoTaskStatus AutotaskStatus { get; set; }
        public virtual Company Company { get; set; }
        public virtual Phase Phase { get; set; }
        public virtual Project Project { get; set; }
        public virtual Queue Queue { get; set; }
        public virtual Sprint Sprint { get; set; }
        public virtual ICollection<WorkflowStep2TeamMember> WorkflowStep2TeamMember { get; set; }
    }
}