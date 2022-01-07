using MongoDB.Bson;
using MongoDB.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gosu.Service.Models
{
    [Collection("FunctionList")]
    public class mdFunctionList : Entity
    {
        public string PageID { get; set; } = "";
        public string PageName { get; set; } = "";
        public string Discriptions { get; set; } = "";
        public string F1 { get; set; } = "";
        public string F2 { get; set; } = "";
        public string F3 { get; set; } = "";
        public string F4 { get; set; } = "";
        public string F5 { get; set; } = "";        
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
        public int UpdMode { get; set; }
    }
}
