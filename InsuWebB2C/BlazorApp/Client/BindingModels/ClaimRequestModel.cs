using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorApp.Client.BindingModels
{
    public class ClaimRequestModel
    {
        public string ID { get; set; } = "";
        public string ClaimNo { get; set; } = "";
        public DateTime ClaimDate { get; set; }
        public string ClaimAccountID { get; set; } = "";
        public string ClaimAccountName { get; set; } = "";
        public string BrancheID { get; set; } = "";
        public string BrancheName { get; set; } = "";
        //Customer & claim
        public string CusFullname { get; set; } = "";
        public string CusPhone { get; set; } = "";
        public string CusEmail { get; set; } = "";
        public string CusCardID { get; set; } = "";
        public string DeviceIMEI { get; set; } = "";
        public string DeviceModel { get; set; } = "";
        public string TPAProductID { get; set; } = "";
        public string TPAProductName { get; set; } = "";
        public DateTime IncidentDate { get; set; }
        public DateTime NotificationDate { get; set; }
        public string DamageCause { get; set; } = "";
        public string DamageType { get; set; }
        public string DamageTypeName { get; set; }
        public string NotificationChannelID { get; set; } = "";
        public string NotificationChannelName { get; set; } = "";
        public int Priority { get; set; }
        //Insur contraction
        public string InsurContractNo { get; set; } = "";
        public DateTime InsurStartDate { get; set; }
        public DateTime InsurEndDate { get; set; }
        public string InsurCompanyID { get; set; } = "";
        public string InsurCompanyName { get; set; } = "";
        public double ContractAmount { get; set; }
        public double InsurAmount { get; set; }
        public string AcceptNotes { get; set; } = "";
        public bool ProcessStatus { get; set; }
        public bool AcceptStatus { get; set; }
        public bool CancelStatus { get; set; }
        //Pickup
        public DateTime PickupReqDate { get; set; }
        public DateTime PickupDoneDate1 { get; set; }
        public DateTime PickupDoneDate2 { get; set; }
        public string PickupCompanyID { get; set; } = "";
        public string PickupCompanyName { get; set; } = "";
        public string PickupAccountID { get; set; } = "";
        public string PickupAccountName { get; set; } = "";
        public string PickupNotes { get; set; } = "";
        public string PickupAddress { get; set; } = "";
        public bool PickupReqStatus { get; set; }
        public bool PickupStatus1 { get; set; }
        public bool PickupStatus2 { get; set; }
        //Check
        public DateTime CheckReqDate { get; set; }
        public DateTime CheckDoneDate { get; set; }
        public string PicAccountID { get; set; } = "";
        public string PicPhone { get; set; } = "";
        public string PicAccountName { get; set; } = "";
        public string CheckNotes { get; set; } = "";
        public bool CheckStatus { get; set; }
        //Repair
        public DateTime EstReqDate { get; set; }
        public DateTime EstDoneDate { get; set; }
        public DateTime RepairDoneDate { get; set; }
        public string RepairCompanyID { get; set; } = "";
        public string RepairCompanyName { get; set; } = "";
        public string RepairAccountID { get; set; } = "";
        public string RepairAccountName { get; set; } = "";
        public string NewDeviceIMEI { get; set; } = "";
        public string NewDeviceModel { get; set; } = "";
        public string RepairReqNotes { get; set; } = "";
        public string RepairNotes { get; set; } = "";
        public bool RepairStatus { get; set; }
        //Estimations
        public List<EstimationModel> Estimations { get; set; } = new List<EstimationModel>();
        public bool EstimationReqStatus { get; set; }
        public bool EstimationStatus { get; set; }
        //Approve
        public DateTime ApproveReqDate { get; set; }
        public DateTime ApproveDoneDate { get; set; }
        public string ApproveAccountID { get; set; } = "";
        public string ApproveAccountName { get; set; } = "";
        public string ApproveNotes { get; set; } = "";
        public bool ApproveReqStatus { get; set; }
        public bool ApproveStatus { get; set; }
        //Return
        public DateTime ReturnReqDate { get; set; }
        public DateTime ReturnDoneDate1 { get; set; }
        public DateTime ReturnDoneDate2 { get; set; }
        public string ReturnCompanyID { get; set; } = "";
        public string ReturnCompanyName { get; set; } = "";
        public string ReturnAccountID { get; set; } = "";
        public string ReturnAccountName { get; set; } = "";
        public string ReturnAddress { get; set; } = "";
        public string ReturnNotes { get; set; } = "";
        public bool ReturnReqStatus { get; set; }
        public bool ReturnStatus1 { get; set; }
        public bool ReturnStatus2 { get; set; }
        //Payment
        public DateTime PayReqDate { get; set; }
        public DateTime PayDoneDate { get; set; }
        public string PayAccountID { get; set; } = "";
        public string PayAccountName { get; set; } = "";
        public string PayReqNotes { get; set; } = "";
        public string PayNotes { get; set; } = "";
        public bool PayReqStatus { get; set; }
        public bool PayStatus { get; set; }
        //Close
        public bool CloseStatus { get; set; }
        //sumary
        public double ClaimAmount { get; set; }
        public double ApproveAmount { get; set; }
        public double DeductibleAmount { get; set; }
        public double IndemnityAmount { get; set; }
        public double RemainingAmount { get; set; }
        //Update History
        public List<UpdateHistoryModel> UpdateHistorys { get; set; } = new List<UpdateHistoryModel>();
        //
        public DateTime ModifiedOn { get; set; }
        public int UpdMode { get; set; }
    }

    public class EstimationModel
    {
        [Range(0, 999, ErrorMessage = "Số dòng không hợp lệ")]
        public int LineNo { get; set; }
        public DateTime IssueDate { get; set; }
        public string ItemCode { get; set; } = "";
        [Required(ErrorMessage = "Bắt buộc nhập")]
        public string ItemName { get; set; } = "";
        public bool IsReplace { get; set; }
        [Range(1, 9999, ErrorMessage = "Số lượng không hợp lệ")]
        public double Quantity { get; set; }
        [Range(0, 9999999999, ErrorMessage = "Đơn giá không hợp lệ")]
        public double UnitPrice { get; set; }
        public double Amount { get; set; }
        [Range(0, 99999999999, ErrorMessage = "Số tiền duyệt không hợp lệ")]
        public double ApproveAmount { get; set; }
        public string Notes { get; set; } = "";
        public string RecNo { get; set; } = "";
        public int UpdMode { get; set; }
        //Row mode
        public bool RowMode_View { get; set; } = false;
        public bool RowMode_Edit { get; set; } = true;
        public bool RowMode_Delete { get; set; } = true;
    }

    public class UpdateHistoryModel
    {
        public DateTime ChangedDate { get; set; }
        public string WorkStep { get; set; }
        public string ChangedStatus { get; set; } = "";
        public string ChangedNotes { get; set; } = "";
    }
}
