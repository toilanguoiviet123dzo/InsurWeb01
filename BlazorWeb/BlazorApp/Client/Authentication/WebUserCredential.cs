﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cores.GrpcClient.Authentication
{
    public static class WebUserCredential
    {
        public static bool IsAuthenticated = false;
        //
        public static string Username = "";
        public static string Fullname = "";
        public static string RoleID = "";
        public static string AccessToken = "";
        public static string ApiKey = "apiKey5963";
        public static int ApproveLevel = 0;
        public static int DocumentLevel = 0;
        public static string GrantedPages = "";
        public static bool IsDevelopmentMode = false;
    }
}