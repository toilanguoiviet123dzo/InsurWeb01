using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GosuAdmin.Client.BindingModels
{
    public class RepairEstimationModel
    {
        public string ID { get; set; } = "";
        public string CompenNo { get; set; } = "";
        public string EstNo { get; set; } = "";
        public DateTime EstDateTime { get; set; }
        public string EstContent { get; set; } = "";
        public int ApproveLevel { get; set; }
        public bool IsTemplate { get; set; }
        //
        public string ReqPersonName { get; set; } = "";
        public string BranchName { get; set; } = "";
        public string RepairerName { get; set; } = "";
        public string TemplateName { get; set; } = "";
        //
        public DateTime ModifiedOn { get; set; }
        public int UpdMode { get; set; }
    }

    public class EstGroupItemModel
    {
        public int LineNo { get; set; }
        public DateTime IssueDateTime { get; set; }
        [Required]
        public string EstItem { get; set; } = "";
        public double RepairPrice { get; set; }
        public double DealRepairPrice { get; set; }
        public double AprRepairPrice { get; set; }
        public double EstVAT { get; set; }
        public double DealVAT { get; set; }
        public double AprVAT { get; set; }
        public string RecNo { get; set; } = "";
        //
        public List<EstDetailItemModel> EstDetailItems = new List<EstDetailItemModel>();
        //
        public int UpdMode { get; set; }
    }

    public class EstDetailItemModel
    {
        [Range(0, 999, ErrorMessage = "Số dòng không hợp lệ")]
        public int LineNo { get; set; }
        public DateTime IssueDateTime { get; set; }
        [Required(ErrorMessage ="Bắt buộc nhập")]
        public string EstItem { get; set; } = "";
        [Range(1, 9999, ErrorMessage ="Số lượng không hợp lệ")]
        public double Quantity { get; set; }
        [Range(0, 9999999999, ErrorMessage = "Số lượng không hợp lệ")]
        public double UnitPrice { get; set; }
        public double Amount { get; set; }
        [Range(0, 99999999999, ErrorMessage = "Số lượng không hợp lệ")]
        public double DealAmount { get; set; }
        public double AprAmount { get; set; }
        public double VatRate { get; set; }
        public double EstVAT { get; set; }
        public double DealVAT { get; set; }
        public double AprVAT { get; set; }
        public string Notes { get; set; } = "";
        public string RecNo { get; set; } = "";
        //
        public int UpdMode { get; set; }
    }
}
