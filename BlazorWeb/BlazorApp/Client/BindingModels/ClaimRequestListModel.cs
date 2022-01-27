using System;
using System.ComponentModel.DataAnnotations;

namespace BlazorApp.Client.BindingModels
{
    public class ClaimRequestListModel
    {
        public string ClaimNo { get; set; } = "";
        public DateTime ClaimDate { get; set; }
        public string StatusName { get; set; } = "";
        public string CusFullname { get; set; } = "";
        public string CusPhone { get; set; } = "";
        public DateTime IncidentDate { get; set; }
        public string DeviceModel { get; set; } = "";
        public string DeviceIMEI { get; set; } = "";
        public string PickupAddress { get; set; } = "";
        public string ClaimAccountName { get; set; } = "";
        public string BrancheName { get; set; } = "";
        public bool AcceptStatus { get; set; }
        public bool PickupStatus1 { get; set; }
        public bool PickupStatus2 { get; set; }
        public bool CheckStatus { get; set; }
        public bool EstimationStatus { get; set; }
        public bool ApproveStatus { get; set; }
        public bool RepairStatus { get; set; }
        public bool ReturnStatus1 { get; set; }
        public bool ReturnStatus2 { get; set; }
        public bool CancelStatus { get; set; }
    }
}
