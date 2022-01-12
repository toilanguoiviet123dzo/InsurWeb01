using System;
using System.ComponentModel.DataAnnotations;

namespace BlazorApp.Client.BindingModels
{
    public class RoleListModel
    {
        public string ID { get; set; } = "";
        public string SystemID { get; set; } = "";
        [Required]
        public string RoleID { get; set; }
        [Required]
        public string RoleName { get; set; } = "";
        public string Discriptions { get; set; } = "";
        public string DspOrder { get; set; } = "";
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
        public int UpdMode { get; set; }
    }
}
