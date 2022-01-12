using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorApp.Client.BindingModels
{
    public class MenuGroupModel
    {
        public string ID { get; set; } = "";
        public string SystemID { get; set; } = "";
        public string GroupID { get; set; } = Cores.Utilities.MyCodeGenerator.GenTransactionID();
        [Required]
        public string GroupName { get; set; } = "";
        public int DisplayOrder { get; set; }
        public string IconUrl { get; set; } = "";
        public bool Enabled { get; set; }
        public int UpdMode { get; set; }
    }
}