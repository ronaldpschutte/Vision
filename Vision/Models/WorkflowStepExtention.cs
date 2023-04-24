using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Vision.Helpers;

namespace Vision.Models
{
    [DataContract]
    public partial class WorkflowStep
    {
        [DataMember]
        public int WorkflowStepId { get; set; }
        public int? OrderIndex { get; set; }
        [DataMember]
        public int CompanyId { get; set; }
        [DataMember]
        public int? ProjectId { get; set; }
        public int? PhaseId { get; set; }
        public int? ParentId { get; set; }
        [DataMember]
        public int StepType { get; set; }
        [DataMember]
        public string Title { get; set; }
        [DataMember]
        public string AutotaskNumber { get; set; }
        [DataMember]
        public int AutotaskStatusId { get; set; }
        [DataMember]
        public int SprintId { get; set; }
        [DataMember]
        [NotMapped]
        public string CompanyName
        {
            get
            {
                return this.Company?.CompanyName ?? string.Empty;
            }
        }
        [DataMember]
        [NotMapped]
        public string ProjectName
        {
            get
            {
                return this.Project?.ProjectName ?? string.Empty;
            }
        }
        [NotMapped]
        public string PhaseName
        {
            get
            {
                return this.Phase?.Title ?? string.Empty;
            }
        }


    }
}