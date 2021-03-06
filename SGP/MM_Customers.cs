//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SGP
{
    using System;
    using System.Collections.Generic;
    
    public partial class MM_Customers
    {
        public MM_Customers()
        {
            this.MM_Mailers = new HashSet<MM_Mailers>();
        }
    
        public string CustomerID { get; set; }
        public string CustomerName { get; set; }
        public Nullable<int> CustomerType { get; set; }
        public string CustomerGroupID { get; set; }
        public string Address { get; set; }
        public string DistrictID { get; set; }
        public string ProvinceID { get; set; }
        public string CountryID { get; set; }
        public string FaxNo { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string CompanyPhone { get; set; }
        public string Mobile { get; set; }
        public string PersonalInfo { get; set; }
        public string BankAccount { get; set; }
        public string BankName { get; set; }
        public string TaxCode { get; set; }
        public bool IsActive { get; set; }
        public string PostOfficeID { get; set; }
        public System.DateTime CreateDate { get; set; }
        public Nullable<System.DateTime> LastEditDate { get; set; }
        public Nullable<System.DateTime> CreationDate { get; set; }
        public Nullable<byte> DebtDayInMonth { get; set; }
        public string MemberOf { get; set; }
        public string DebitObjectID { get; set; }
        public string CustomerPreID { get; set; }
    
        public virtual ICollection<MM_Mailers> MM_Mailers { get; set; }
    }
}
