using MongoDB.Bson;
using MongoDB.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorApp.Server.Models
{
    [Collection("RoleList")]
    public class mdRoleList : Entity
    {
        public string RoleID { get; set; } 
        public string RoleName { get; set; } = "";
        public string Discriptions { get; set; } = "";                
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
        public int UpdMode { get; set; }
        public string DspOrder { get; set; } = "";
    }
}
