using System;
using System.ComponentModel.DataAnnotations;

namespace BlazorApp.Client.BindingModels
{
    public class RoleListModel
    {
        public string ID { get; set; } = "";
        [Required(ErrorMessage = "Bắt buộc nhập.")]
        public string RoleID { get; set; }
        [Required(ErrorMessage = "Bắt buộc nhập.")]
        public string RoleName { get; set; } = "";
        public string Discriptions { get; set; } = "";
        public int DspOrder { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
        public int UpdMode { get; set; }
        //Row mode
        public bool RowMode_View { get; set; } = false;
        public bool RowMode_Edit { get; set; } = true;
        public bool RowMode_Delete { get; set; } = true;
    }
}
