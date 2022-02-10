using System;
using System.ComponentModel.DataAnnotations;

namespace BlazorApp.Client.BindingModels
{
    public class BranchMasterModel
    {
        public string ID { get; set; } = "";
        [Required(ErrorMessage = "Bắt buộc nhập.")]
        public string BranchID { get; set; } = "";
        [Required(ErrorMessage = "Bắt buộc nhập.")]
        public string BranchName { get; set; } = "";
        public string PhoneNo { get; set; } = "";
        public string Email { get; set; } = "";
        public string Address { get; set; } = "";
        public string Notes { get; set; } = "";
        public string PicName { get; set; } = "";
        public int Status { get; set; }
        public string StatusName { get; set; } = "";
        public int DspOrder { get; set; }
        public DateTime ModifiedOn { get; set; }
        public int UpdMode { get; set; }
    }
}
