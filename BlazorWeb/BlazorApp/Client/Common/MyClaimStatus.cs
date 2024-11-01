﻿using BlazorApp.Client.BindingModels;
using Cores.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorApp.Client.Common
{
    public static class MyClaimStatus
    {
        #region All
        public static List<CodeNameModel> Get_StatusList()
        {
            var ret = new List<CodeNameModel>();
            //
            ret.Add(new CodeNameModel { CodeInt = 1, Name = "Thụ lý" });
            ret.Add(new CodeNameModel { CodeInt = 2, Name = "Lấy hàng" });
            ret.Add(new CodeNameModel { CodeInt = 3, Name = "Nhập hàng" });
            ret.Add(new CodeNameModel { CodeInt = 4, Name = "Kiểm tra" });
            ret.Add(new CodeNameModel { CodeInt = 5, Name = "Chấp nhận" });
            ret.Add(new CodeNameModel { CodeInt = 6, Name = "Báo giá" });
            ret.Add(new CodeNameModel { CodeInt = 7, Name = "Duyệt" });
            ret.Add(new CodeNameModel { CodeInt = 8, Name = "Sửa chữa" });
            ret.Add(new CodeNameModel { CodeInt = 9, Name = "Xuất hàng" });
            ret.Add(new CodeNameModel { CodeInt = 10, Name = "Trả hàng" });
            ret.Add(new CodeNameModel { CodeInt = 11, Name = "Chi trả" });
            ret.Add(new CodeNameModel { CodeInt = 12, Name = "Từ chối" });
            //
            return ret;
        }
        public static string Get_StatusName(ClaimRequestListModel dataRow)
        {
            string StatusName = "Chưa thụ lý";
            if (dataRow.ProcessStatus) StatusName = "Đã thụ lý";
            if (dataRow.PickupReqStatus) StatusName = "YC lấy hàng";
            if (dataRow.PickupStatus1) StatusName = "Đã lấy hàng";
            if (dataRow.PickupStatus2) StatusName = "Đã nhập hàng";
            if (dataRow.CheckStatus) StatusName = "Đã kiểm tra";
            if (dataRow.AcceptStatus) StatusName = "Đã chấp nhận";
            if (dataRow.EstimationReqStatus) StatusName = "YC báo giá";
            if (dataRow.EstimationStatus) StatusName = "Đã báo giá";
            if (dataRow.ApproveReqStatus) StatusName = "YC duyệt";
            if (dataRow.ApproveStatus) StatusName = "Đã duyệt";
            if (dataRow.RepairStatus) StatusName = "Đã sửa chữa";
            if (dataRow.ReturnStatus1) StatusName = "Đã xuất hàng";
            if (dataRow.ReturnStatus2) StatusName = "Đã trả hàng";
            if (dataRow.PayStatus) StatusName = "Đã chi trả";
            if (dataRow.CancelStatus) StatusName = "Đã từ chối";
            if (dataRow.CloseStatus) StatusName = "Đã đóng";
            //
            return StatusName;
        }
        #endregion

        #region Pickup
        public static List<CodeNameModel> Get_PickupStatusList(int fromStatus = 1, int toStatus = 10)
        {
            var ret = new List<CodeNameModel>();
            ret.Add(new CodeNameModel { CodeInt = 2, Name = "Lấy hàng" });
            ret.Add(new CodeNameModel { CodeInt = 3, Name = "Nhập hàng" });
            //
            return ret;
        }

        public static string Get_PickupStatusName(ClaimRequestListModel dataRow)
        {
            string StatusName = "Chưa lấy hàng";
            if (dataRow.PickupStatus1) StatusName = "Đã lấy hàng";
            if (dataRow.PickupStatus2) StatusName = "Đã nhập hàng";
            if (dataRow.CloseStatus) StatusName = "Đã đóng";
            //
            return StatusName;
        }
        #endregion

        #region Repair
        public static List<CodeNameModel> Get_RepairStatusList(int fromStatus = 1, int toStatus = 10)
        {
            var ret = new List<CodeNameModel>();
            ret.Add(new CodeNameModel { CodeInt = 6, Name = "Báo giá" });
            ret.Add(new CodeNameModel { CodeInt = 8, Name = "Sửa chữa" });
            //
            return ret;
        }

        public static string Get_RepairStatusName(ClaimRequestListModel dataRow)
        {
            string StatusName = "Chưa báo giá";
            if (dataRow.EstimationStatus) StatusName = "Đã báo giá";
            if (dataRow.RepairStatus) StatusName = "Đã sửa chữa";
            if (dataRow.CloseStatus) StatusName = "Đã đóng";
            //
            return StatusName;
        }
        #endregion

        #region Approve
        public static List<CodeNameModel> Get_ApproveStatusList(int fromStatus = 1, int toStatus = 10)
        {
            var ret = new List<CodeNameModel>();
            ret.Add(new CodeNameModel { CodeInt = 7, Name = "Duyệt" });
            ret.Add(new CodeNameModel { CodeInt = 8, Name = "Sửa chữa" });
            ret.Add(new CodeNameModel { CodeInt = 11, Name = "Chi trả" });
            //
            return ret;
        }

        public static string Get_ApproveStatusName(ClaimRequestListModel dataRow)
        {
            string StatusName = "Chưa duyệt";
            if (dataRow.ApproveStatus) StatusName = "Đã duyệt";
            if (dataRow.RepairStatus) StatusName = "Đã sửa chữa";
            if (dataRow.PayStatus) StatusName = "Đã chi trả";
            if (dataRow.CloseStatus) StatusName = "Đã đóng";
            //
            return StatusName;
        }
        #endregion

        #region Payment
        public static List<CodeNameModel> Get_PaymentStatusList(int fromStatus = 1, int toStatus = 10)
        {
            var ret = new List<CodeNameModel>();
            ret.Add(new CodeNameModel { CodeInt = 8, Name = "Sửa chữa" });
            ret.Add(new CodeNameModel { CodeInt = 11, Name = "Chi trả" });
            //
            return ret;
        }

        public static string Get_PaymentStatusName(ClaimRequestListModel dataRow)
        {
            string StatusName = "Chưa chi trả";
            if (dataRow.RepairStatus) StatusName = "Đã sửa chữa";
            if (dataRow.PayStatus) StatusName = "Đã chi trả";
            if (dataRow.CloseStatus) StatusName = "Đã đóng";
            //
            return StatusName;
        }
        #endregion

        #region Return
        public static List<CodeNameModel> Get_ReturnStatusList(int fromStatus = 1, int toStatus = 10)
        {
            var ret = new List<CodeNameModel>();
            ret.Add(new CodeNameModel { CodeInt = 9, Name = "Xuất hàng" });
            ret.Add(new CodeNameModel { CodeInt = 10, Name = "Trả hàng" });
            //
            return ret;
        }

        public static string Get_ReturnStatusName(ClaimRequestListModel dataRow)
        {
            string StatusName = "Chưa xuất hàng";
            if (dataRow.ReturnStatus1) StatusName = "Đã xuất hàng";
            if (dataRow.ReturnStatus2) StatusName = "Đã trả hàng";
            if (dataRow.CloseStatus) StatusName = "Đã đóng";
            //
            return StatusName;
        }
        #endregion





    }// end class

}
