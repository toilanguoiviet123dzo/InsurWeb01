using MongoDB.Bson;
using MongoDB.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorApp.Server.Models
{
    [Collection("RepairerMaster")]
    public class mdRepairerMaster : Entity
    {
        public string RepairerID { get; set; } = "";
        public string RepairerName { get; set; } = "";
        public string PhoneNo { get; set; } = "";
        public string Email { get; set; } = "";
        public string Address { get; set; } = "";
        public string Notes { get; set; } = "";
        public string PicName { get; set; } = "";
        public bool Status { get; set; }
        public int DspOrder { get; set; }
        public DateTime ModifiedOn { get; set; }
        public int UpdMode { get; set; }
    }
}
