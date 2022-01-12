using MongoDB.Bson;
using MongoDB.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorApp.Server.Models
{
    [Collection("MenuDetail")]
    public class mdMenuDetail : Entity
    {
        public string GroupID { get; set; } = "";
        public string PageID { get; set; } = "";
        public string PageName { get; set; } = "";
        public string IconUrl { get; set; } = "";
        public int DisplayOrder { get; set; }
        public bool Enabled { get; set; }
        public int UpdMode { get; set; }
    }
}
