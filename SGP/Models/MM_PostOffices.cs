//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SGP.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class MM_PostOffices
    {
        public MM_PostOffices()
        {
            this.MM_PostOffices1 = new HashSet<MM_PostOffices>();
        }
    
        public string PostOfficeID { get; set; }
        public string PostOfficeName { get; set; }
        public string Address { get; set; }
        public string ZoneID { get; set; }
        public string ProvinceID { get; set; }
        public string Phone { get; set; }
        public string FaxNo { get; set; }
        public string Email { get; set; }
        public bool IsCollaborator { get; set; }
        public string Notes { get; set; }
        public Nullable<System.DateTime> LastEditDate { get; set; }
        public Nullable<System.DateTime> CreationDate { get; set; }
        public string TaxCode { get; set; }
        public string BankAccount { get; set; }
        public string MemberOf { get; set; }
        public string CustomerPre { get; set; }
    
        public virtual ICollection<MM_PostOffices> MM_PostOffices1 { get; set; }
        public virtual MM_PostOffices MM_PostOffices2 { get; set; }
    }
}
