using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PharmacyDB.Models
{
    public partial class DrugForm
    {
        public int Id { get; set; }
        public int FormId { get; set; }

        public int DrugId { get; set; }
        public float Dose { get; set; }
        public float Volume { get; set; }

        public virtual Drug Drug { get; set; } 

        public virtual Form Form { get; set; }
    }
}
