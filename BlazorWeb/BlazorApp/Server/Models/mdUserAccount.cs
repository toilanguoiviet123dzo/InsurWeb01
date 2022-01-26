using MongoDB.Bson;
using MongoDB.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorApp.Server.Models
{
    [Collection("UserAccount")]
    public class mdUserAccount : Entity
    {
        public string UserID { get; set; } = "";
        public string UserName { get; set; } = "";
        public string Password { get; set; } = "";
        public string Fullname { get; set; } = "";
        public string Phone { get; set; } = "";
        public string Email { get; set; } = "";
        public string Address { get; set; } = "";
        public string RoleID { get; set; } = "";
        public string BrancheID { get; set; } = "";
        public int ApproveLevel { get; set; }
        public string ApproveLevelName { get; set; }
        public int DocumentLevel { get; set; }
        public string DocumentLevelName { get; set; }
        public int Status { get; set; }
        public DateTime ModifiedOn { get; set; }
        public int UpdMode { get; set; }
    }
}
