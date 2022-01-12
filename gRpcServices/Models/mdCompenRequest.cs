using MongoDB.Bson;
using MongoDB.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cores.Service.Models
{
    [Collection("CompenRequest")]
    public class mdCompenRequest : Entity
    {
        public string CustomerID { get; set; } = "";
        public string CustomerName { get; set; } = "";
        public string CompenNo { get; set; } = "";
        public string InternalDocNo { get; set; } = "";
        public DateTime CompenDateTime { get; set; }
        public string CarOwner { get; set; } = "";
        public string PhoneNo { get; set; } = "";
        public string LicensePlate { get; set; } = "";
        public string BrandName { get; set; } = "";
        public string CarType { get; set; } = "";
        public string ManufactureYear { get; set; } = "";
        public int SeatCount { get; set; }
        public string BusinessTarget { get; set; } = "";
        public string CarRegisterNo { get; set; } = "";
        public DateTime RegFromDate { get; set; }
        public DateTime RegToDate { get; set; }
        public string FirstYearReg { get; set; } = "";
        public int Weight { get; set; }
        public string Driver { get; set; } = "";
        public string DriveLicenseNo { get; set; } = "";
        public string DriveLicenseLevel { get; set; } = "";
        public DateTime LicenseFromDate { get; set; }
        public DateTime LicenseToDate { get; set; }
        //Accident
        public DateTime AccidentDateTime { get; set; }
        public string AccidentPlace { get; set; } = "";
        public string AccidentProcessor { get; set; } = "";
        public string AccidentProgress { get; set; } = "";
        public string AccidentReason { get; set; } = "";
        public string DamageVolume { get; set; } = "";
        //Insurance
        public string VCXContractNo { get; set; } = "";
        public DateTime VCXFromDate { get; set; }
        public DateTime VCXToDate { get; set; }
        public string VCXPayStatus { get; set; } = "";
        public DateTime VCXPayDate { get; set; }
        public string VCXContent { get; set; } = "";
        public string VCXRules { get; set; }
        public string VCXDeduct { get; set; } = "";
        //
        public string TNDSContractNo { get; set; } = "";
        public DateTime TNDSFromDate { get; set; }
        public DateTime TNDSToDate { get; set; }
        public string TNDSPayStatus { get; set; } = "";
        public DateTime TNDSPayDate { get; set; }
        public string TNDSContent { get; set; } = "";
        //Accept
        public bool AcceptStatus { get; set; }
        public bool CancelStatus { get; set; }
        public DateTime AcceptDatetime { get; set; }
        public string AcceptPersonID { get; set; } = "";
        public string AcceptPersonName { get; set; } = "";
        public string AcceptNotes { get; set; } = "";
        //Estimation
        public bool EstReqStatus { get; set; }
        public DateTime EstReqDateTime { get; set; }
        public bool EstDoneStatus { get; set; }
        public DateTime EstDoneDateTime { get; set; }
        public bool EstAprStatus { get; set; }
        public DateTime EstAprDateTime { get; set; }
        public string EstPersonID { get; set; } = "";
        public string EstPersonName { get; set; } = "";
        //Request
        public bool CompenStatus { get; set; }
        public DateTime ReqDateTime { get; set; }
        public string ReqPersonID { get; set; } = "";
        public string ReqPersonName { get; set; } = "";
        public string ReqContent { get; set; } = "";
        public double EstDamageAmount { get; set; }
        public string CompenCalDescription { get; set; } = "";
        //Approve
        public bool AprStatus { get; set; }
        public DateTime AprDateTime { get; set; }
        public string AprPersonID { get; set; } = "";
        public string AprPersonName { get; set; } = "";
        public string AprContent { get; set; } = "";
        public int ApproveLevel { get; set; }
        //Repaire
        public bool RepairStatus { get; set; }
        public bool AprRepairStatus { get; set; }
        public DateTime RepairDoneDatetime { get; set; }
        public DateTime AprRepairDatetime { get; set; }
        public string RepairerID { get; set; } = "";
        public string RepairerName { get; set; } = "";
        public string RepairNotes { get; set; } = "";
        //Pyament
        public bool PayStatus { get; set; }
        public DateTime PayDateTime { get; set; }
        public string PayPersonID { get; set; } = "";
        public string PayPersonName { get; set; } = "";
        public string PayContent { get; set; } = "";
        public string PaymentTo { get; set; } = "";
        //Price
        public double EstRepairPrice { get; set; }
        public double DealRepairPrice { get; set; }
        public double AprRepairPrice { get; set; }
        public double DiscountRate { get; set; }
        public double DiscountAmount { get; set; }
        public double TipAmount { get; set; }
        public double CompensationPrice { get; set; }
        public double EstVAT { get; set; }
        public double DealVAT { get; set; }
        public double AprVAT { get; set; }
        //Documents
        public int DocumentLevel { get; set; }
        public bool IsPayForCustomer { get; set; }
        //Branch
        public string BranchID { get; set; } = "";
        public string BranchName { get; set; } = "";
        //
        public DateTime ModifiedOn { get; set; }
        public int UpdMode { get; set; }
    }
}
