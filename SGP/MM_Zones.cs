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
    
    public partial class MM_Zones
    {
        public MM_Zones()
        {
            this.MM_PostOffices = new HashSet<MM_PostOffices>();
        }
    
        public string ZoneID { get; set; }
        public string ZoneName { get; set; }
        public string Notes { get; set; }
        public Nullable<System.DateTime> LastEditDate { get; set; }
        public Nullable<System.DateTime> CreationDate { get; set; }
    
        public virtual ICollection<MM_PostOffices> MM_PostOffices { get; set; }
    }
}
