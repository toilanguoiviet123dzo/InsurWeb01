using System;
using System.ComponentModel.DataAnnotations;

namespace BlazorApp.Client.BindingModels
{
    public class UserAccountModel
    {
        public string ID { get; set; } = "";
        public string UserID { get; set; } = "";
        [Required]
        public string UserName { get; set; } = "";
        public string Password { get; set; } = "";
        [Required]
        public string Fullname { get; set; } = "";
        public string Phone { get; set; } = "";
        public string Email { get; set; } = "";
        public string Address { get; set; } = "";
        [Required]
        public string RoleID { get; set; } = "";
        public string RoleName { get; set; } = "";
        public int ApproveLevel { get; set; }
        public string ApproveLevelName { get; set; }
        public int DocumentLevel { get; set; }
        public string DocumentLevelName { get; set; }
        public int Status { get; set; }
        public string StatusName { get; set; }
        public DateTime ModifiedOn { get; set; }
        public int UpdMode { get; set; }
    }
}
