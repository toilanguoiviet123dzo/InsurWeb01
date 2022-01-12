using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Cores.Common
{
    public static class MyConstant
    {
        //Log level
        public const int LogLevel_Trace = 0;
        public const int LogLevel_Debug = 1;
        public const int LogLevel_Information = 2;
        public const int LogLevel_Warning = 3;
        public const int LogLevel_Error = 4;
        public const int LogLevel_Critical = 5;
        public const int LogLevel_None = 6;
        //Currency
        public const string NaturalCurrencyUnit = "VND";
        //Payment channel
        public const string PaymentChannel_Google = "google";
        public const string PaymentChannel_Apple = "apple";
        public const string PaymentChannel_CardPay = "cardpay";
        public const string PaymentChannel_DirectPay = "directpay";
        public const string PaymentChannel_MochaPay = "mochapay";
        public const string PaymentChannel_MomoPay = "momopay";
        public const string PaymentChannel_OnePay = "onepay";
        public const string PaymentChannel_SmsPay = "smspay";
        public const string PaymentChannel_VnPay = "vnpay";
        public const string PaymentChannel_VAPay = "vapay";
        //Client framework
        public const string ClientFramework_Android = "1";
        public const string ClientFramework_IOS = "2";
        public const string ClientFramework_Web = "3";
        public const string ClientFramework_PC = "4";
        //Search
        public const int Search_MaxRecordCount = 1000;



    }// end class
}// end name space
