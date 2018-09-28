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
    
    public partial class Comment
    {
        public Comment()
        {
            this.FileAttachComments = new HashSet<FileAttachComment>();
        }
    
        public string Id { get; set; }
        public string UserPost { get; set; }
        public string PostOfficeId { get; set; }
        public string Content { get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }
        public string ActivityId { get; set; }
    
        public virtual ActivityInfo ActivityInfo { get; set; }
        public virtual ICollection<FileAttachComment> FileAttachComments { get; set; }
        public virtual WK_PostOffice WK_PostOffice { get; set; }
    }
}