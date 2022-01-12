using MongoDB.Bson;
using MongoDB.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorApp.Server.Models
{
    [Collection("BranchMaster")]
    public class mdBranchMaster : Entity
    {
        public string BranchID { get; set; } = "";
        public string BranchName { get; set; } = "";
        public string InternalID { get; set; } = "";
        public string Discriptions { get; set; } = "";
        public int DspOrder { get; set; }
        public bool Enabled { get; set; }
        public DateTime ModifiedOn { get; set; }
        public int UpdMode { get; set; }
    }
}
