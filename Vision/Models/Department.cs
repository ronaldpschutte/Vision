﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Vision.Models
{
    public partial class Department
    {
        public Department()
        {
            TeamMemberRole = new HashSet<TeamMemberRole>();
        }

        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public bool? AutotaskSync { get; set; }

        public virtual ICollection<TeamMemberRole> TeamMemberRole { get; set; }
    }
}