using System;
using System.ComponentModel.DataAnnotations;

namespace BlazorApp.Client.BindingModels
{
    public class BranchMasterModel
    {
        public string ID { get; set; } = "";
        [Required(ErrorMessage ="Bắt buộc nhập.")]
        public string BranchID { get; set; } = "";
        [Required(ErrorMessage = "Bắt buộc nhập.")]
        public string BranchName { get; set; } = "";
        public string InternalID { get; set; } = "";
        public string Discriptions { get; set; } = "";
        public int DspOrder { get; set; }
        public bool Enabled { get; set; }
        public DateTime ModifiedOn { get; set; }
        public int UpdMode { get; set; }
    }
}
