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
    
    public partial class MM_MailerDeliveryDetail
    {
        public string DocumentID { get; set; }
        public string MailerID { get; set; }
        public string Notes { get; set; }
        public Nullable<bool> IsDeliverOver { get; set; }
        public string DeliveryTo { get; set; }
        public Nullable<System.DateTime> DeliveryDate { get; set; }
        public string DeliveryStatus { get; set; }
        public Nullable<bool> PaymentFinished { get; set; }
        public string DeliveryNotes { get; set; }
        public Nullable<System.DateTime> ConfirmDate { get; set; }
        public string ConfirmUserID { get; set; }
        public string ConfirmIndex { get; set; }
        public Nullable<System.DateTime> LastEditDate { get; set; }
        public Nullable<System.DateTime> CreationDate { get; set; }
        public long ID { get; set; }
        public string ReturnReasonID { get; set; }
    
        public virtual MM_MailerDelivery MM_MailerDelivery { get; set; }
    }
}
