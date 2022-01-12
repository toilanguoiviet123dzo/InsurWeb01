using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cores.Common
{
    public static class MyDecoder
    {
        public const string PROC_CONTENT_1 = "Chờ xác nhận";
        public const string PROC_CONTENT_2 = "Đã tiếp nhận đơn hàng";
        public const string PROC_CONTENT_3 = "Hoàn tác - Đã tiếp nhận đơn hàng";
        public const string PROC_CONTENT_4 = "Đặt hàng thành công";
        public const string PROC_CONTENT_5 = "Thanh toán thất bại";
        public const string PROC_CONTENT_6 = "Thanh toán thành công";
        public const string PROC_CONTENT_7 = "Hoàn tác - Thanh toán thành công";
        public const string PROC_CONTENT_8 = "Yêu cầu hủy";
        public const string PROC_CONTENT_9 = "Hoàn tác - Yêu cầu hủy";
        public const string PROC_CONTENT_10 = "Đã hủy";
        public const string PROC_CONTENT_11 = "Hoàn tác - Đã hủy";
        public const string PROC_CONTENT_12 = "Từ chối hủy ghi danh";
        public const string PROC_CONTENT_13 = "Đã hoàn tiền";
        public const string PROC_CONTENT_14 = "Hoàn tác - Đã hoàn tiền";
        public static string ProcContent_ToText(string contentCode)
        {
            switch (contentCode)
            {
                case "1":
                    return PROC_CONTENT_1;
                case "2":
                    return PROC_CONTENT_2;
                case "3":
                    return PROC_CONTENT_3;
                case "4":
                    return PROC_CONTENT_4;
                case "5":
                    return PROC_CONTENT_5;
                case "6":
                    return PROC_CONTENT_6;
                case "7":
                    return PROC_CONTENT_7;
                case "8":
                    return PROC_CONTENT_8;
                case "9":
                    return PROC_CONTENT_9;
                case "10":
                    return PROC_CONTENT_10;
                case "11":
                    return PROC_CONTENT_11;
                case "12":
                    return PROC_CONTENT_12;
                case "13":
                    return PROC_CONTENT_13;
                case "14":
                    return PROC_CONTENT_14;
                default:
                    break;
            }
            //Not match
            return contentCode;
        }

        public static string ProcContent_ToCode(string contentText)
        {
            switch (contentText)
            {
                case PROC_CONTENT_1:
                    return "1";
                case PROC_CONTENT_2:
                    return "2";
                case PROC_CONTENT_3:
                    return "3";
                case PROC_CONTENT_4:
                    return "4";
                case PROC_CONTENT_5:
                    return "5";
                case PROC_CONTENT_6:
                    return "6";
                case PROC_CONTENT_7:
                    return "7";
                case PROC_CONTENT_8:
                    return "8";
                case PROC_CONTENT_9:
                    return "9";
                case PROC_CONTENT_10:
                    return "10";
                case PROC_CONTENT_11:
                    return "11";
                case PROC_CONTENT_12:
                    return "12";
                case PROC_CONTENT_13:
                    return "13";
                case PROC_CONTENT_14:
                    return "14";
                default:
                    break;
            }
            //Not match
            return contentText;
        }
    }
}
