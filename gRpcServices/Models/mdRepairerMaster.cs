using MongoDB.Bson;
using MongoDB.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cores.Service.Models
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
        public string EstPersonID { get; set; } = "";
        public string EstPersonName { get; set; } = "";
        public int Status { get; set; }
        public DateTime ModifiedOn { get; set; }
        public int UpdMode { get; set; }
    }
}
