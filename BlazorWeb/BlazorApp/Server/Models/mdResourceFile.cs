using MongoDB.Bson;
using MongoDB.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorApp.Server.Models
{
    [Collection("ResourceFile")]
    public class mdResourceFile : Entity
    {
        public string ResourceID { get; set; } = "";
        public string CategoryID { get; set; } = "";
        public string FileName { get; set; } = "";
        public string ServerFileName { get; set; } = "";
        public byte[] FileContent { get; set; } = new byte[] { };
        public int ArchiveMode { get; set; }
        public DateTime ModifiedOn { get; set; }
        public int UpdMode { get; set; }
    }
}
