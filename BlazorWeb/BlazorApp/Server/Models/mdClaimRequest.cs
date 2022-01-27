﻿using BlazorApp.Shared.Models;
using MongoDB.Bson;
using MongoDB.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorApp.Server.Models
{
    [Collection("ClaimRequest")]
    public class mdClaimRequest : Entity
    {
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
        public string NotificationChannel { get; set; } = "";
        public int Priority { get; set; }
        //Insur contraction
        public string InsurContractNo { get; set; } = "";
        public DateTime InsurStartDate { get; set; }
        public DateTime InsurEndDate { get; set; }
        public string InsurCompanyID { get; set; } = "";
        public string InsurCompanyName { get; set; } = "";
        public double InsurAmount { get; set; }
        public string AcceptNotes { get; set; } = "";
        public bool AcceptStatus { get; set; }
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
        public DateTime RepairReqDate { get; set; }
        public DateTime RepairDoneDate { get; set; }
        public string RepairCompanyID { get; set; } = "";
        public string RepairCompanyName { get; set; } = "";
        public string RepairAccountID { get; set; } = "";
        public string RepairAccountName { get; set; } = "";
        public string NewDeviceIMEI { get; set; } = "";
        public string NewDeviceModel { get; set; } = "";
        public string RepairNotes { get; set; } = "";
        public bool RepairStatus { get; set; }
        //Estimations
        public List<EstimationModel> Estimations { get; set; } = new List<EstimationModel>();
        public bool EstimationStatus { get; set; }
        //Approve
        public DateTime ApproveReqDate { get; set; }
        public DateTime ApproveDoneDate { get; set; }
        public string ApproveAccountID { get; set; } = "";
        public string ApproveAccountName { get; set; } = "";
        public string ApproveNotes { get; set; } = "";
        public bool ApproveStatus { get; set; }
        //Return
        public DateTime ReturnReqDate { get; set; }
        public DateTime ReturnDoneDate { get; set; }
        public string ReturnCompanyID { get; set; } = "";
        public string ReturnCompanyName { get; set; } = "";
        public string ReturnAccountID { get; set; } = "";
        public string ReturnAccountName { get; set; } = "";
        public string ReturnAddress { get; set; } = "";
        public string ReturnNotes { get; set; } = "";
        public bool ReturnStatus1 { get; set; }
        public bool ReturnStatus2 { get; set; }
        //sumary
        public double ClaimAmount { get; set; }
        public double ApproveAmount { get; set; }
        public double DeductibleAmount { get; set; }
        public double IndemnityAmount { get; set; }
        public double RemainingAmount { get; set; }
        //Images
        public List<ImageModel> Images { get; set; } = new List<ImageModel>();
        //Update History
        public List<UpdateHistoryModel> UpdateHistorys { get; set; } = new List<UpdateHistoryModel>();
        //Cancel
        public bool CancelStatus { get; set; }
        public string TextSearch { get; set; } = "";
        public DateTime ModifiedOn { get; set; }
        public int UpdMode { get; set; }
    }
    public class EstimationModel
    {
        public int LineNo { get; set; }
        public DateTime IssueDate { get; set; }
        public string ItemCode { get; set; } = "";
        public string ItemName { get; set; } = "";
        public bool IsReplace { get; set; }
        public double Quantity { get; set; }
        public double UnitPrice { get; set; }
        public double Amount { get; set; }
        public double ApproveAmount { get; set; }
        public string Notes { get; set; } = "";
    }

    public class ImageModel
    {
        public DateTime IssueDate { get; set; }
        public int SecureLevel { get; set; }
        public string ImageID { get; set; } = "";
        public string Title { get; set; } = "";
        public string UploadAccountID { get; set; } = "";
    }

    public class UpdateHistoryModel
    {
        public DateTime ChangedDate { get; set; }
        public string WorkStep { get; set; }
        public string ChangedStatus { get; set; } = "";
        public string ChangedNotes { get; set; } = "";
    }
}
