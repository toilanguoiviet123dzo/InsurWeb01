using Cores.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorApp.Client.Common
{
    public static class MyClaimStatus
    {
        public static List<CodeNameModel> Get_StatusList()
        {
            return new List<CodeNameModel>() {
                new CodeNameModel{CodeInt = 1, Name="Chấp nhận"},
                new CodeNameModel{CodeInt = 2, Name="Lấy hàng"},
                new CodeNameModel{CodeInt = 3, Name="Nhập hàng"},
                new CodeNameModel{CodeInt = 4, Name="Kiểm tra"},
                new CodeNameModel{CodeInt = 5, Name="Báo giá"},
                new CodeNameModel{CodeInt = 6, Name="Duyệt"},
                new CodeNameModel{CodeInt = 7, Name="Sửa chữa"},
                new CodeNameModel{CodeInt = 8, Name="Xuất hàng"},
                new CodeNameModel{CodeInt = 9, Name="Trả hàng"},
                new CodeNameModel{CodeInt = 10, Name="Từ chối"}
            };
        }
        public static string Get_StatusName(int StatusCode)
        {
            string StatusName = "";
            if (StatusCode == 0) StatusName = "Chưa chấp nhận";
            if (StatusCode == 1) StatusName = "Đã chấp nhận";
            if (StatusCode == 2) StatusName = "Đã lấy hàng";
            if (StatusCode == 3) StatusName = "Đã nhập hàng";
            if (StatusCode == 4) StatusName = "Đã kiểm tra";
            if (StatusCode == 5) StatusName = "Đã báo giá";
            if (StatusCode == 6) StatusName = "Đã duyệt";
            if (StatusCode == 7) StatusName = "Đã sửa chữa";
            if (StatusCode == 8) StatusName = "Đã xuất hàng";
            if (StatusCode == 9) StatusName = "Đã trả hàng";
            if (StatusCode == 10) StatusName = "Đã từ chối";
            //
            return StatusName;
        }


    }// end class

}
