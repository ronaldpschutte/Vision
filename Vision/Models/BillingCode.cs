﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Vision.Models
{
    public partial class BillingCode
    {
        public int BillingCodeId { get; set; }
        public int? AfterHoursWorkType { get; set; }
        public int? BillingCodeType { get; set; }
        public int? DepartmentId { get; set; }
        public string Description { get; set; }
        public string ExternalNumber { get; set; }
        public int? GeneralLedgerAccount { get; set; }
        public bool IsActive { get; set; }
        public bool? IsExcludedFromNewContracts { get; set; }
        public double? MarkupRate { get; set; }
        public string Name { get; set; }
        public int? TaxCategoryId { get; set; }
        public double UnitCost { get; set; }
        public double UnitPrice { get; set; }
        public int? UseType { get; set; }
    }
}