using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GosuAdmin.Client.BindingModels
{
    public class MenuDetailModel
    {
        public string ID { get; set; } = "";
        public string SystemID { get; set; } = "";
        public string GroupID { get; set; } = "";
        [Required(ErrorMessage = "The PageName field is required.")]
        public string PageID { get; set; } = "";
        public string PageName { get; set; } = "";
        public string IconUrl { get; set; } = "";
        public int DisplayOrder { get; set; }
        public bool Enabled { get; set; }
        public int UpdMode { get; set; }
    }
}