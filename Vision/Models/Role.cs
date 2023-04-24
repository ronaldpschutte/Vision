﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Vision.Models
{
    public partial class Role
    {
        public Role()
        {
            TeamMemberRole = new HashSet<TeamMemberRole>();
            WorkflowStep = new HashSet<WorkflowStep>();
            WorkflowStep2TeamMember = new HashSet<WorkflowStep2TeamMember>();
        }

        public int RoleId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }

        public virtual ICollection<TeamMemberRole> TeamMemberRole { get; set; }
        public virtual ICollection<WorkflowStep> WorkflowStep { get; set; }
        public virtual ICollection<WorkflowStep2TeamMember> WorkflowStep2TeamMember { get; set; }
    }
}