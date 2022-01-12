using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Cores.Common
{
    public static class MyStringExtentions
    {
        #region Md5
        /// <summary>
        /// Mã hóa MD5
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string Md5(this string s)
        {
            try
            {
                MD5 md5 = MD5CryptoServiceProvider.Create();
                byte[] dataMd5 = md5.ComputeHash(Encoding.UTF8.GetBytes(s));
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < dataMd5.Length; i++)
                    sb.AppendFormat("{0:x2}", dataMd5[i]);
                return sb.ToString();
            }
            catch
            {
                return "";
            }
        }
        #endregion

        #region String manipulation
        public static string Left(this string s, int nCount)
        {
            return s.Substring(0, nCount);
        }

        public static string Right(this string s, int nCount)
        {
            return s.Substring(s.Length - nCount, nCount);
        }

        public static string Mid(this string s, int nIndex, int nCount)
        {
            return s.Substring(nIndex, nCount);
        }
        #endregion

        #region conversion
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
        public static void RemoveVietnameseSign(this string s)
        {
            //Replace sign
            for (int i = 1; i < VietnameseSigns.Length; i++)
            {
                for (int j = 0; j < VietnameseSigns[i].Length; j++)
                    s = s.Replace(VietnameseSigns[i][j], VietnameseSigns[0][i - 1]);
            }
        }
        public static int ToInt(this string s)
        {
            int numValue = 0;
            int.TryParse(s, out numValue);
            return numValue;
        }
        public static double ToDouble(this string s)
        {
            double numValue = 0;
            double.TryParse(s, out numValue);
            return numValue;
        }
        public static decimal ToDecimal(this string s)
        {
            decimal numValue = 0;
            decimal.TryParse(s, out numValue);
            return numValue;
        }
        #endregion


    }
}
