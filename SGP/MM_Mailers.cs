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
    
    public partial class MM_Mailers
    {
        public MM_Mailers()
        {
            this.MM_MailerDeliveryDetail = new HashSet<MM_MailerDeliveryDetail>();
            this.MM_MailerDeliveryDetail1 = new HashSet<MM_MailerDeliveryDetail>();
        }
    
        public System.DateTime AcceptDate { get; set; }
        public System.DateTime AcceptTime { get; set; }
        public string MailerID { get; set; }
        public string SenderID { get; set; }
        public string SenderRepresenterID { get; set; }
        public string SenderName { get; set; }
        public string SenderAddress { get; set; }
        public string SenderPhone { get; set; }
        public string SenderCountryID { get; set; }
        public string SenderProvinceID { get; set; }
        public string SenderDistrictID { get; set; }
        public string RecieverID { get; set; }
        public string RecieverRepresenterID { get; set; }
        public string RecieverName { get; set; }
        public string RecieverAddress { get; set; }
        public string RecieverPhone { get; set; }
        public string RecieverCountryID { get; set; }
        public string RecieverProvinceID { get; set; }
        public string RecieverDistrictID { get; set; }
        public string ServiceTypeID { get; set; }
        public string MailerTypeID { get; set; }
        public int Quantity { get; set; }
        public double RealWeight { get; set; }
        public double Weight { get; set; }
        public decimal Money { get; set; }
        public decimal Price { get; set; }
        public decimal PriceDefault { get; set; }
        public decimal PriceService { get; set; }
        public double Discount { get; set; }
        public decimal BefVATAmount { get; set; }
        public double VATPercent { get; set; }
        public decimal VATAmount { get; set; }
        public decimal Amount { get; set; }
        public decimal AmountBefDiscount { get; set; }
        public string PostOfficeAcceptID { get; set; }
        public string PaymentMethodID { get; set; }
        public string PostOfficeRecieverMoneyID { get; set; }
        public string EmployeeID { get; set; }
        public string MailerDescription { get; set; }
        public string ThirdpartyDocID { get; set; }
        public Nullable<decimal> ThirdpartyCost { get; set; }
        public string ThirdpartyPaymentMethodID { get; set; }
        public string ParentMailerID { get; set; }
        public string UserGroupID { get; set; }
        public string LastUserGroupID { get; set; }
        public System.DateTime CreateDate { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public string CurrentStatusID { get; set; }
        public string CurrentPostOfficeID { get; set; }
        public Nullable<System.DateTime> LastEditDate { get; set; }
        public Nullable<System.DateTime> CreationDate { get; set; }
        public string DocIndex { get; set; }
        public string RangeWeightID { get; set; }
        public string RangeDistanceID { get; set; }
        public string PriceType { get; set; }
        public Nullable<bool> PriceIncludeVAT { get; set; }
        public Nullable<decimal> CommissionAmt { get; set; }
        public Nullable<double> CommissionPercent { get; set; }
        public Nullable<decimal> CostAmt { get; set; }
        public Nullable<System.DateTime> SalesClosingDate { get; set; }
        public string RecieverAddressNbr { get; set; }
        public string ReceiveProvinceID { get; set; }
        public Nullable<double> DiscountPercent { get; set; }
        public Nullable<System.DateTime> LastUpdDate { get; set; }
        public int RecordState { get; set; }
        public bool SyncFlag { get; set; }
        public Nullable<System.DateTime> LastSyncDate { get; set; }
        public Nullable<decimal> Amt4Comm { get; set; }
        public Nullable<System.DateTime> LastUpdStatusTime { get; set; }
    
        public virtual MM_Customers MM_Customers { get; set; }
        public virtual ICollection<MM_MailerDeliveryDetail> MM_MailerDeliveryDetail { get; set; }
        public virtual ICollection<MM_MailerDeliveryDetail> MM_MailerDeliveryDetail1 { get; set; }
        public virtual MM_PostOffices MM_PostOffices { get; set; }
        public virtual MM_PostOffices MM_PostOffices1 { get; set; }
    }
}
