namespace Pereklichka.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("DeferredMailing")]
    public partial class DeferredMailing
    {
        [Key]
        public int DeferredId { get; set; }

        public int GroupId { get; set; }

        public DateTime DeferredTime { get; set; }
        public string DeferredTimeStr
        {
            get
            {
                int hour = DeferredTime.Hour;
                int minute = DeferredTime.Minute;

                return DeferredTime.Date.ToString("dd.MM.yyyy") + "\n" + 
                       (hour.ToString().Length < 2 ? "0" + hour : hour.ToString()) + ":" +
                        (minute.ToString().Length < 2 ? "0" + minute : minute.ToString());
            }
        }

        [Required]
        [StringLength(50)]
        public string SendEmail { get; set; }

        public int StatusId { get; set; }

        public virtual Group Group { get; set; }

        public virtual Status Status { get; set; }
    }
}
