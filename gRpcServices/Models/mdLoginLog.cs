using MongoDB.Bson;
using MongoDB.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gosu.Service.Models
{
    [Name("UserLog")]
    public class mdLoginLog : Entity
    {
        public string UserName { get; set; } = "";
        public string IP { get; set; } = "";
        public string LogAction { get; set; } = "";
        public string Content1 { get; set; } = "";
        public string Content2 { get; set; } = "";
        public string Utm { get; set; } = "";
        public DateTime CreatedOn { get; set; }
    }
}
