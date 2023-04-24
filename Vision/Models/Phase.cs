﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Vision.Models
{
    public partial class Phase
    {
        public Phase()
        {
            WorkflowStep = new HashSet<WorkflowStep>();
        }

        public int PhaseId { get; set; }
        public int? ParentId { get; set; }
        public int ProjectId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string PhaseNumber { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? DueDate { get; set; }
        public double? EstimatedHours { get; set; }
        public DateTime? LastActivityDateTime { get; set; }
        public int? ChangedBy { get; set; }

        public virtual Project Project { get; set; }
        public virtual ICollection<WorkflowStep> WorkflowStep { get; set; }
    }
}