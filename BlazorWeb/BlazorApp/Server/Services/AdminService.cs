using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Google.Protobuf.WellKnownTypes;
using Cores.Grpc.Client;
using Cores.Admin.Services;
using WebPush;
using BlazorApp.Shared.Models;
using System.Text.Json;

namespace BlazorApp.Server.Services
{
    public class AdminService : grpcAdminService.grpcAdminServiceBase
    {
        //SubscribeToNotifications
        public override async Task<Empty_Response> SubscribeToNotifications(SubscribeToNotifications_Request request, ServerCallContext context)
        {
            return await GrpcClientFactory.CallServiceAsync<Empty_Response>(ServiceList.Admin, async channel =>
            {
                var client = new grpcAdminService.grpcAdminServiceClient(channel);
                return await client.SubscribeToNotificationsAsync(request);
            });
        }

        // WebPushNotification
        public override async Task<Empty_Response> WebPushNotification(WebPushNotification_Request request, ServerCallContext context)
        {
            var response = new Empty_Response();
            response.ReturnCode = GrpcReturnCode.OK;
            try
            {
                //Get subscription list for 1 user
                var getSubcribeRequest = new String_Request();
                getSubcribeRequest.StringValue = request.UserName;
                //
                var getSubcribeResponse = await GrpcClientFactory.CallServiceAsync<GetNotificationSubscribe_Response>(ServiceList.Admin, async channel =>
                {
                    var client = new grpcAdminService.grpcAdminServiceClient(channel);
                    return await client.GetNotificationSubscribeAsync(getSubcribeRequest);
                });
                if (getSubcribeResponse == null || getSubcribeResponse.ReturnCode != GrpcReturnCode.OK)
                {
                    response.ReturnCode = GrpcReturnCode.Error_201;
                    return response;
                }
                //Skip if no subcribe
                if (getSubcribeResponse.Records == null || getSubcribeResponse.Records.Count == 0)
                {
                    return response;
                }

                //Push message
                var webPushClient = new WebPushClient();
                var publicKey = "BAZ3BBQf7kCk6BmTGdBIKUhku56Nw9FQkWZNpTyKKqtx2vuhd6VTq915qNCmcpBCd44O5Y_xAxJbsIAiUnvCgV4";
                var privateKey = "yuSqS_Mwd1rXkppl1ltqRBTZc_VCzIEbE66b8M51riM";
                var vapidDetails = new VapidDetails("mailto: <toilanguoiviet123dzo@gmail.com>", publicKey, privateKey);
                //
                foreach (var subscription in getSubcribeResponse.Records)
                {
                    var pushSubscription = new PushSubscription(subscription.Url, subscription.P256Dh, subscription.Auth);
                    try
                    {
                        var payload = JsonSerializer.Serialize(new
                        {
                            request.Messages,
                            url = request.Url,
                        });
                        await webPushClient.SendNotificationAsync(pushSubscription, payload, vapidDetails);
                    }
                    catch (Exception ex)
                    {
                        Console.Error.WriteLine("Error sending push notification: " + ex.Message);
                    }
                }
            }
            catch (Exception)
            {
                response.ReturnCode = GrpcReturnCode.Error_ByServer;
            }
            //
            return response;
        }
        // Get ServiceList
        public override async Task<GrpcLogin_Response> GrpcLogin(GrpcLogin_Request request, ServerCallContext context)
        {
            return await GrpcClientFactory.CallServiceAsync<GrpcLogin_Response>(ServiceList.Admin, async channel =>
            {
                var client = new grpcAdminService.grpcAdminServiceClient(channel);
                return await client.GrpcLoginAsync(request);
            });
        }
        // Get ServiceList
        public override async Task<GetServiceList_Response> GetServiceList(Empty_Request request, ServerCallContext context)
        {
            return await GrpcClientFactory.CallServiceAsync<GetServiceList_Response>(ServiceList.Admin, async channel =>
            {
                var client = new grpcAdminService.grpcAdminServiceClient(channel);
                return await client.GetServiceListAsync(request);
            });
        }
        // Save ServiceList
        public override async Task<Empty_Response> SaveServiceList(SaveServiceList_Request request, ServerCallContext context)
        {
            return await GrpcClientFactory.CallServiceAsync<Empty_Response>(ServiceList.Admin, async channel =>
            {
                var client = new grpcAdminService.grpcAdminServiceClient(channel);
                return await client.SaveServiceListAsync(request);
            });
        }
        // Get OptionListHeader
        public override async Task<GetOptionListHeader_Response> GetOptionListHeader(Empty_Request request, ServerCallContext context)
        {
            return await GrpcClientFactory.CallServiceAsync<GetOptionListHeader_Response>(ServiceList.Admin, async channel =>
            {
                var client = new grpcAdminService.grpcAdminServiceClient(channel);
                return await client.GetOptionListHeaderAsync(request);
            });
        }
        // Save OptionListHeader
        public override async Task<String_Response> SaveOptionListHeader(SaveOptionListHeader_Request request, ServerCallContext context)
        {
            return await GrpcClientFactory.CallServiceAsync<String_Response>(ServiceList.Admin, async channel =>
            {
                var client = new grpcAdminService.grpcAdminServiceClient(channel);
                return await client.SaveOptionListHeaderAsync(request);
            });
        }
        // Get OptionList
        public override async Task<GetOptionList_Response> GetOptionList(String_Request request, ServerCallContext context)
        {
            return await GrpcClientFactory.CallServiceAsync<GetOptionList_Response>(ServiceList.Admin, async channel =>
            {
                var client = new grpcAdminService.grpcAdminServiceClient(channel);
                return await client.GetOptionListAsync(request);
            });
        }
        // Save OptionList
        public override async Task<String_Response> SaveOptionList(SaveOptionList_Request request, ServerCallContext context)
        {
            return await GrpcClientFactory.CallServiceAsync<String_Response>(ServiceList.Admin, async channel =>
            {
                var client = new grpcAdminService.grpcAdminServiceClient(channel);
                return await client.SaveOptionListAsync(request);
            });
        }
        // Get FunctionList
        public override async Task<GetFunctionList_Response> GetFunctionList(Empty_Request request, ServerCallContext context)
        {
            return await GrpcClientFactory.CallServiceAsync<GetFunctionList_Response>(ServiceList.Admin, async channel =>
            {
                var client = new grpcAdminService.grpcAdminServiceClient(channel);
                return await client.GetFunctionListAsync(request);
            });
        }
        // Save FunctionList
        public override async Task<Empty_Response> SaveFunctionList(SaveFunctionList_Request request, ServerCallContext context)
        {
            return await GrpcClientFactory.CallServiceAsync<Empty_Response>(ServiceList.Admin, async channel =>
            {
                var client = new grpcAdminService.grpcAdminServiceClient(channel);
                return await client.SaveFunctionListAsync(request);
            });
        }
        // Get RoleList
        public override async Task<GetRoleList_Response> GetRoleList(Empty_Request request, ServerCallContext context)
        {
            return await GrpcClientFactory.CallServiceAsync<GetRoleList_Response>(ServiceList.Admin, async channel =>
            {
                var client = new grpcAdminService.grpcAdminServiceClient(channel);
                return await client.GetRoleListAsync(request);
            });
        }
        // Save RoleList
        public override async Task<Empty_Response> SaveRoleList(SaveRoleList_Request request, ServerCallContext context)
        {
            return await GrpcClientFactory.CallServiceAsync<Empty_Response>(ServiceList.Admin, async channel =>
            {
                var client = new grpcAdminService.grpcAdminServiceClient(channel);
                return await client.SaveRoleListAsync(request);
            });
        }
        // Get RoleDetail
        public override async Task<GetRoleDetail_Response> GetRoleDetail(Cores.Admin.Services.String_Request request, ServerCallContext context)
        {
            return await GrpcClientFactory.CallServiceAsync<GetRoleDetail_Response>(ServiceList.Admin, async channel =>
            {
                var client = new grpcAdminService.grpcAdminServiceClient(channel);
                return await client.GetRoleDetailAsync(request);
            });
        }
        // Save RoleDetail
        public override async Task<Empty_Response> SaveRoleDetail(SaveRoleDetail_Request request, ServerCallContext context)
        {
            return await GrpcClientFactory.CallServiceAsync<Empty_Response>(ServiceList.Admin, async channel =>
            {
                var client = new grpcAdminService.grpcAdminServiceClient(channel);
                return await client.SaveRoleDetailAsync(request);
            });
        }
        // Delete RoleDetail
        public override async Task<Empty_Response> DeleteRoleDetail(DeleteRoleDetail_Request request, ServerCallContext context)
        {
            return await GrpcClientFactory.CallServiceAsync<Empty_Response>(ServiceList.Admin, async channel =>
            {
                var client = new grpcAdminService.grpcAdminServiceClient(channel);
                return await client.DeleteRoleDetailAsync(request);
            });
        }
        // Get UserRole
        public override async Task<GetUserRole_Response> GetUserRole(Empty_Request request, ServerCallContext context)
        {
            return await GrpcClientFactory.CallServiceAsync<GetUserRole_Response>(ServiceList.Admin, async channel =>
            {
                var client = new grpcAdminService.grpcAdminServiceClient(channel);
                return await client.GetUserRoleAsync(request);
            });
        }
        // Save UserRole
        public override async Task<Empty_Response> SaveUserRole(SaveUserRole_Request request, ServerCallContext context)
        {
            return await GrpcClientFactory.CallServiceAsync<Empty_Response>(ServiceList.Admin, async channel =>
            {
                var client = new grpcAdminService.grpcAdminServiceClient(channel);
                return await client.SaveUserRoleAsync(request);
            });
        }
        // Get SettingMaster
        public override async Task<GetSettingMaster_Response> GetSettingMaster(Empty_Request request, ServerCallContext context)
        {
            return await GrpcClientFactory.CallServiceAsync<GetSettingMaster_Response>(ServiceList.Admin, async channel =>
            {
                var client = new grpcAdminService.grpcAdminServiceClient(channel);
                return await client.GetSettingMasterAsync(request);
            });
        }
        // Save SettingMaster
        public override async Task<Empty_Response> SaveSettingMaster(SaveSettingMaster_Request request, ServerCallContext context)
        {
            return await GrpcClientFactory.CallServiceAsync<Empty_Response>(ServiceList.Admin, async channel =>
            {
                var client = new grpcAdminService.grpcAdminServiceClient(channel);
                return await client.SaveSettingMasterAsync(request);
            });
        }
        // Get MenuGroup
        public override async Task<GetMenuGroup_Response> GetMenuGroup(Empty_Request request, ServerCallContext context)
        {
            return await GrpcClientFactory.CallServiceAsync<GetMenuGroup_Response>(ServiceList.Admin, async channel =>
            {
                var client = new grpcAdminService.grpcAdminServiceClient(channel);
                return await client.GetMenuGroupAsync(request);
            });
        }
        // Save MenuGroup
        public override async Task<Empty_Response> SaveMenuGroup(SaveMenuGroup_Request request, ServerCallContext context)
        {
            return await GrpcClientFactory.CallServiceAsync<Empty_Response>(ServiceList.Admin, async channel =>
            {
                var client = new grpcAdminService.grpcAdminServiceClient(channel);
                return await client.SaveMenuGroupAsync(request);
            });
        }
        // Get MenuDetail
        public override async Task<GetMenuDetail_Response> GetMenuDetail(Empty_Request request, ServerCallContext context)
        {
            return await GrpcClientFactory.CallServiceAsync<GetMenuDetail_Response>(ServiceList.Admin, async channel =>
            {
                var client = new grpcAdminService.grpcAdminServiceClient(channel);
                return await client.GetMenuDetailAsync(request);
            });
        }
        // Save MenuDetail
        public override async Task<Empty_Response> SaveMenuDetail(SaveMenuDetail_Request request, ServerCallContext context)
        {
            return await GrpcClientFactory.CallServiceAsync<Empty_Response>(ServiceList.Admin, async channel =>
            {
                var client = new grpcAdminService.grpcAdminServiceClient(channel);
                return await client.SaveMenuDetailAsync(request);
            });
        }
        // Get UserAccount
        public override async Task<GetUserAccount_Response> GetUserAccount(String_Request request, ServerCallContext context)
        {
            return await GrpcClientFactory.CallServiceAsync<GetUserAccount_Response>(ServiceList.Admin, async channel =>
            {
                var client = new grpcAdminService.grpcAdminServiceClient(channel);
                return await client.GetUserAccountAsync(request);
            });
        }
        // Save UserAccount
        public override async Task<String_Response> SaveUserAccount(SaveUserAccount_Request request, ServerCallContext context)
        {
            return await GrpcClientFactory.CallServiceAsync<String_Response>(ServiceList.Admin, async channel =>
            {
                var client = new grpcAdminService.grpcAdminServiceClient(channel);
                return await client.SaveUserAccountAsync(request);
            });
        }
        // GetVoucherNo
        public override async Task<String_Response> GetVoucherNo(String_Request request, ServerCallContext context)
        {
            return await GrpcClientFactory.CallServiceAsync<String_Response>(ServiceList.Admin, async channel =>
            {
                var client = new grpcAdminService.grpcAdminServiceClient(channel);
                return await client.GetVoucherNoAsync(request);
            });
        }

    }
}
