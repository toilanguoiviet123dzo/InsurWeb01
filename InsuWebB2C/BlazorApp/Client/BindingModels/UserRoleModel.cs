using System;
using System.ComponentModel.DataAnnotations;

namespace BlazorApp.Client.BindingModels
{
    public class UserRoleModel
    {
        public string ID { get; set; } = "";
        [Required]
        public string UserName { get; set; } = "";
        public string SystemID { get; set; } = "";
        //[RegularExpression(@"[^0]", ErrorMessage = "The RoleName field is required.")]
        public string RoleID { get; set; }
        public string Discriptions { get; set; } = "";
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
        public int UpdMode { get; set; }
        // Just Local
        [Required]
        public string RoleName { get; set; } = "";
    }
}
