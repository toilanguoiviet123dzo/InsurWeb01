using System;
using System.ComponentModel.DataAnnotations;

namespace GosuAdmin.Client.BindingModels
{
    public class OptionListModel
    {
        public string ID { get; set; } = "";
        public string ListCode { get; set; } = "";
        public string ItemCode { get; set; } = "";
        public int IntCode { get; set; }
        public double DoubleCode { get; set; }
        [Required]
        public string ItemName { get; set; } = "";
        public string DspOrder { get; set; } = "";
        public string Description { get; set; } = "";
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
        public int UpdMode { get; set; }
    }
}
