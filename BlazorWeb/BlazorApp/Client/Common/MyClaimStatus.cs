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
            ret.Add(new CodeNameModel { CodeInt = 11, Name = "Từ chối" });
            //
            return ret;
        }
        public static string Get_StatusName(int StatusCode)
        {
            string StatusName = "Chưa lấy hàng";
            if (StatusCode == 1) StatusName = "Đã thụ lý";
            if (StatusCode == 2) StatusName = "Đã lấy hàng";
            if (StatusCode == 3) StatusName = "Đã nhập hàng";
            if (StatusCode == 4) StatusName = "Đã kiểm tra";
            if (StatusCode == 5) StatusName = "Đã chấp nhận";
            if (StatusCode == 6) StatusName = "Đã báo giá";
            if (StatusCode == 7) StatusName = "Đã duyệt";
            if (StatusCode == 8) StatusName = "Đã sửa chữa";
            if (StatusCode == 9) StatusName = "Đã xuất hàng";
            if (StatusCode == 10) StatusName = "Đã trả hàng";
            if (StatusCode == 11) StatusName = "Đã từ chối";
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

        public static string Get_PickupStatusName(int StatusCode)
        {
            string StatusName = "Chưa lấy hàng";
            if (StatusCode == 2) StatusName = "Đã lấy hàng";
            if (StatusCode == 3) StatusName = "Đã nhập hàng";
            //
            return StatusName;
        }
        #endregion



    }// end class

}
