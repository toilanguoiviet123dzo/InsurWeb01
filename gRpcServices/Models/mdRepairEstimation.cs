using MongoDB.Bson;
using MongoDB.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cores.Service.Models
{
    [Collection("RepairEstimation")]
    public class mdRepairEstimation : Entity
    {
        public string CompenNo { get; set; } = "";
        public string EstNo { get; set; } = "";
        public DateTime EstDateTime { get; set; }
        public string EstContent { get; set; } = "";
        public int ApproveLevel { get; set; }
        public List<EstGroupItemModel> EstGroupItems { get; set; } = new List<EstGroupItemModel>();
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
        public string EstItem { get; set; } = "";
        public double RepairPrice { get; set; }
        public double DealRepairPrice { get; set; }
        public double AprRepairPrice { get; set; }
        public double EstVAT { get; set; }
        public double DealVAT { get; set; }
        public double AprVAT { get; set; }
        public string RecNo { get; set; } = "";
        public List<EstDetailItemModel> EstDetailItems { get; set; } = new List<EstDetailItemModel>();
    }
    public class EstDetailItemModel
    {
        public int LineNo { get; set; }
        public DateTime IssueDateTime { get; set; }
        public string EstItem { get; set; } = "";
        public double Quantity { get; set; }
        public double UnitPrice { get; set; }
        public double Amount { get; set; }
        public double DealAmount { get; set; }
        public double AprAmount { get; set; }
        public double VatRate { get; set; }
        public double EstVAT { get; set; }
        public double DealVAT { get; set; }
        public double AprVAT { get; set; }
        public string Notes { get; set; } = "";
        public string RecNo { get; set; } = "";
    }
}
