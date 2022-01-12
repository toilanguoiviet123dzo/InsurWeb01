using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cores.Service.Models
{
    public class ApproveEstimation_ReturnModel
    {
        public bool HasChanged { get; set; } = false;
        public string CompenNo { get; set; } = "";
        public double RepairPrice { get; set; }
        public double DealRepairPrice { get; set; }
        public double AprRepairPrice { get; set; }
        public double EstVAT { get; set; }
        public double DealVAT { get; set; }
        public double AprVAT { get; set; }
    }
}
