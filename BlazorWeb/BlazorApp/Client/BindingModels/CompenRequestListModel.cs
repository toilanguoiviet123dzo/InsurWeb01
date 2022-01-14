using System;
using System.ComponentModel.DataAnnotations;

namespace BlazorApp.Client.BindingModels
{
    public class ClaimRequestListModel
    {
        public string ClaimNo { get; set; } = "";
        public DateTime AcceptDatetime { get; set; }
        public string StatusName { get; set; } = "";
        public string BranchName { get; set; } = "";
        public string CarOwner { get; set; } = "";
        public string VCXContractNo { get; set; } = "";
        public string PhoneNo { get; set; } = "";
        public string LicensePlate { get; set; } = "";
        public DateTime CompenDateTime { get; set; }
        public DateTime AccidentDateTime { get; set; }
        public string AccidentPlace { get; set; } = "";
        public double EstDamageAmount { get; set; }
        public string ReqPersonName { get; set; } = "";

        //accept
        public bool AcceptStatus { get; set; }
        public bool CancelStatus { get; set; }
        //Process
        public bool CompenStatus { get; set; }
        public DateTime ReqDateTime { get; set; }
        //Estimation
        public bool EstReqStatus { get; set; }
        public DateTime EstReqDateTime { get; set; }
        public bool EstDoneStatus { get; set; }
        public DateTime EstDoneDateTime { get; set; }
        public bool EstAprStatus { get; set; }
        public DateTime EstAprDateTime { get; set; }
        //Approve
        public bool AprStatus { get; set; }
        public int ApproveLevel { get; set; }
        //Repaire
        public bool RepairStatus { get; set; }
        public bool AprRepairStatus { get; set; }
        public DateTime RepairDoneDatetime { get; set; }
        public DateTime AprRepairDatetime { get; set; }
        //Pay
        public bool PayStatus { get; set; }
    }
}
