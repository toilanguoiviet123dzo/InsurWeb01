using System;
using System.ComponentModel.DataAnnotations;

namespace GosuAdmin.Client.BindingModels
{
    public class SettingMasterModel
    {
        public string ID { get; set; } = "";
        [Required]
        public string Code { get; set; } = "";
        [Required]
        public string Description { get; set; } = "";
        public string StringValue1 { get; set; } = "";
        public string StringValue2 { get; set; } = "";
        public int IntValue1 { get; set; }
        public int IntValue2 { get; set; }
        public double DoubleValue1 { get; set; }
        public double DoubleValue2 { get; set; }
        public DateTime ModifiedOn { get; set; }
        public int UpdMode { get; set; }
    }
}