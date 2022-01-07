using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Gosu.Extentions
{
    public static class StringExt
    {
        #region Conversion
        public static SecureString ConvertToSecureString(this string plainString)
        {
            var secureStr = new SecureString();
            if (plainString.Length > 0)
            {
                foreach (var c in plainString.ToCharArray()) secureStr.AppendChar(c);
            }
            return secureStr;
        }
        private static readonly string[] VietnameseSigns = new string[]
        {
            "aAeEoOuUiIdDyY",
            "áàạảãâấầậẩẫăắằặẳẵ",
            "ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ",
            "éèẹẻẽêếềệểễ",
            "ÉÈẸẺẼÊẾỀỆỂỄ",
            "óòọỏõôốồộổỗơớờợởỡ",
            "ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ",
            "úùụủũưứừựửữ",
            "ÚÙỤỦŨƯỨỪỰỬỮ",
            "íìịỉĩ",
            "ÍÌỊỈĨ",
            "đ",
            "Đ",
            "ýỳỵỷỹ",
            "ÝỲỴỶỸ"
        };
        public static string RemoveVietnameseSign(this string str)
        {
            //Replace sign
            for (int i = 1; i < VietnameseSigns.Length; i++)
            {
                for (int j = 0; j < VietnameseSigns[i].Length; j++)
                    str = str.Replace(VietnameseSigns[i][j], VietnameseSigns[0][i - 1]);
            }
            return str;
        }
        #endregion

        #region String manipulation
        public static string Left(this string scrString, int nCount)
        {
            return scrString.Substring(0, nCount);
        }

        public static string Right(this string scrString, int nCount)
        {
            return scrString.Substring(scrString.Length - nCount, nCount);
        }

        public static string Mid(this string scrString, int nIndex, int nCount)
        {
            return scrString.Substring(nIndex, nCount);
        }
        #endregion

        #region format
        public static string Format_Voucher8(this string str)
        {
            //Skip check
            if (str == null || str.Trim() == "") return str;
            if (str.Length >= 7) return str;

            //Year
            string year = DateTime.Now.ToString("yy");

            //Parse to int
            int num = 0;
            int.TryParse(str, out num);

            string seq = num.ToString("000000");

            //
            return year + seq;
        }
        public static string Format_Voucher11(this string str)
        {
            //Skip check
            if (str == null || str.Trim() == "") return str;
            if (str.Length >= 10) return str;

            //Year
            string year = DateTime.Now.ToString("yy");

            //Parse to int
            int num = 0;
            int.TryParse(str, out num);

            string seq = num.ToString("000000000");

            //
            return year + seq;
        }
        #endregion

    }
}
