using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using System.Net.Http.Headers;
using Gosu.Admin.Services;
using Gosu.GrpcWeb.Client;

namespace Gosu.GrpcClient.Authentication
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly AuthenticationStateProvider _autStateProvider;
        private readonly ILocalStorageService _localStorage;
        private readonly grpcAdminService.grpcAdminServiceClient _adminServiceClient;

        public AuthenticationService(AuthenticationStateProvider autStateProvider,
                                     ILocalStorageService localStorage,
                                     grpcAdminService.grpcAdminServiceClient adminServiceClient)
        {
            _autStateProvider = autStateProvider;
            _localStorage = localStorage;
            _adminServiceClient = adminServiceClient;
        }

        public async Task<bool> Login(AuthenticationRequestModel userForAuthentication)
        {
            //Clear
            var loginResult = false;
            WebUserCredential.Username = "";
            WebUserCredential.Fullname = "";
            WebUserCredential.RoleID = "";
            WebUserCredential.ApproveLevel = 0;
            WebUserCredential.DocumentLevel = 0;
            //
            try
            {
                //gRpc login
                var request = new GrpcLogin_Request()
                {
                    Credential = new UserCredential()
                    {
                        Username = WebUserCredential.Username,
                        RoleID = WebUserCredential.RoleID,
                        AccessToken = WebUserCredential.AccessToken,
                        ApiKey = WebUserCredential.ApiKey
                    },
                    UserName = userForAuthentication.UserName,
                    Password = userForAuthentication.Password
                };

                //Call grpc
                var response = await _adminServiceClient.GrpcLoginAsync(request);
                if (response != null && response.ReturnCode == GrpcReturnCode.OK)
                {
                    WebUserCredential.Username = response.UserName;
                    WebUserCredential.Fullname = response.Fullname;
                    WebUserCredential.RoleID = response.RoleID;
                    WebUserCredential.ApproveLevel = response.ApproveLevel;
                    WebUserCredential.DocumentLevel = response.DocumentLevel;
                    //
                    //Get granted pages
                    var requestRoleDetail = new String_Request()
                    {
                        Credential = new UserCredential()
                        {
                            Username = WebUserCredential.Username,
                            RoleID = WebUserCredential.RoleID,
                            AccessToken = WebUserCredential.AccessToken,
                            ApiKey = WebUserCredential.ApiKey
                        }
                    };
                    requestRoleDetail.StringValue = WebUserCredential.RoleID;
                    //
                    var responseRoleDetail = await _adminServiceClient.GetRoleDetailAsync(requestRoleDetail);
                    if (responseRoleDetail != null && responseRoleDetail.ReturnCode == 200)
                    {
                        WebUserCredential.GrantedPages = "";
                        foreach (var item in responseRoleDetail.RoleDetail)
                        {
                            WebUserCredential.GrantedPages += item.PageID + ";";
                        }
                    }
                    //
                    loginResult = true;
                    //Notify login
                    ((AuthStateProvider)_autStateProvider).NotifyUserAuthentication();
                }
            }
            catch { }
            //
            return loginResult;
        }

        public void Development_Login()
        {
            //Clear
            WebUserCredential.Username = "AdminDev";
            WebUserCredential.Fullname = "Admin Development";
            WebUserCredential.RoleID = "99";
            WebUserCredential.ApproveLevel = 99;
            WebUserCredential.DocumentLevel = 99;
            WebUserCredential.IsDevelopmentMode = true;
            //
            //Notify login
            ((AuthStateProvider)_autStateProvider).NotifyUserAuthentication();
        }

        public void Logout()
        {
            //Notify logout
            ((AuthStateProvider)_autStateProvider).NotifyUserLogout();
        }


    }
}
