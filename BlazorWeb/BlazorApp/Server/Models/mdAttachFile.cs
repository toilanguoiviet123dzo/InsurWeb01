using MongoDB.Bson;
using MongoDB.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorApp.Server.Models
{
    [Collection("AttachFile")]
    public class mdAttachFile : Entity
    {
        public string VoucherNo { get; set; } = "";
        public DateTime IssueDateTime { get; set; }
        public string CategoryID { get; set; } = "";
        public string CategoryName { get; set; } = "";
        public string FileName { get; set; } = "";
        public string Notes { get; set; } = "";
        public string ResourceID { get; set; } = "";
        public int DocumentLevel { get; set; }
        public string DocumentLevelName { get; set; }
        public string DataOwnerID { get; set; }
        public DateTime ModifiedOn { get; set; }
        public int UpdMode { get; set; }
    }
}
