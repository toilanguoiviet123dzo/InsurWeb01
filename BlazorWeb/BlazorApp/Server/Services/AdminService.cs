using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Google.Protobuf.WellKnownTypes;
using Cores.Grpc.Client;
using Admin.Services;
using WebPush;
using BlazorApp.Shared.Models;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using MongoDB.Entities;
using BlazorApp.Server.Models;
using BlazorApp.Server.Common;
using Cores.Helpers;
using Cores.Utilities;

namespace BlazorApp.Server.Services
{
    public class AdminService : grpcAdminService.grpcAdminServiceBase
    {
        private readonly ILogger<AdminService> _logger;

        public AdminService(ILogger<AdminService> logger)
        {
            _logger = logger;
        }
        //-------------------------------------------------------------------------------------------------------/
        // GrpcLogin
        //-------------------------------------------------------------------------------------------------------/
        public override async Task<GrpcLogin_Response> GrpcLogin(Admin.Services.GrpcLogin_Request request, ServerCallContext context)
        {
            var response = new GrpcLogin_Response();
            response.ReturnCode = GrpcReturnCode.OK;
            response.MsgCode = "";
            //
            try
            {
                var findRecord = await DB.Find<mdUserAccount>()
                                         .Match(x => x.UserName == request.UserName && x.Password == request.Password)
                                         .ExecuteFirstAsync();
                //
                if (findRecord != null)
                {
                    response.UserName = findRecord.UserName;
                    response.Fullname = findRecord.Fullname;
                    response.RoleID = findRecord.RoleID;
                    response.ApproveLevel = findRecord.ApproveLevel;
                    response.DocumentLevel = findRecord.DocumentLevel;
                }
                else
                {
                    response.ReturnCode = GrpcReturnCode.Error_201;
                }
            }
            catch (Exception ex)
            {
                response.ReturnCode = GrpcReturnCode.Error_ByServer;
                response.MsgCode = ex.Message;
                MyAppLog.WriteLog(MyConstant.LogLevel_Critical, "AdminService", "GrpcLogin", "Exception", response.ReturnCode, ex.Message);
            }
            //
            return await Task.FromResult(response);
        }

        //-------------------------------------------------------------------------------------------------------/
        // SaveOptionList
        //-------------------------------------------------------------------------------------------------------/
        public override async Task<Admin.Services.String_Response> SaveOptionList(SaveOptionList_Request request, ServerCallContext context)
        {
            var response = new Admin.Services.String_Response();
            response.ReturnCode = GrpcReturnCode.OK;  //OK
            //
            try
            {
                if (request.Record.UpdMode != 3)
                {
                    //Insert || Update
                    var saveRecord = new mdOptionList();
                    ClassHelper.CopyPropertiesData(request.Record, saveRecord);
                    //Insert
                    if (request.Record.UpdMode == 1) saveRecord.ID = saveRecord.GenerateNewID();
                    //
                    await saveRecord.SaveAsync();
                    //return saved ID
                    response.StringValue = saveRecord.ID;
                }
                else
                {
                    //Delete
                    await DB.DeleteAsync<mdOptionList>(request.Record.ID);
                }
            }
            catch (Exception ex)
            {
                response.ReturnCode = GrpcReturnCode.Error_ByServer;
                response.MsgCode = ex.Message;
                MyAppLog.WriteLog(MyConstant.LogLevel_Critical, "AdminService", "SaveOptionList", "Exception", response.ReturnCode, ex.Message);
            }
            //
            return await Task.FromResult(response);
        }
        //-------------------------------------------------------------------------------------------------------/
        // SaveOptionListHeader
        //-------------------------------------------------------------------------------------------------------/
        public override async Task<Admin.Services.String_Response> SaveOptionListHeader(SaveOptionListHeader_Request request, ServerCallContext context)
        {
            var response = new Admin.Services.String_Response();
            response.ReturnCode = GrpcReturnCode.OK;
            try
            {
                if (request.Record.UpdMode != 3)
                {
                    //Insert || Update
                    var saveRecord = new mdOptionListHeader();
                    ClassHelper.CopyPropertiesData(request.Record, saveRecord);
                    //Insert
                    if (request.Record.UpdMode == 1) saveRecord.ID = saveRecord.GenerateNewID();
                    //
                    await saveRecord.SaveAsync();
                    //return saved ID
                    response.StringValue = saveRecord.ID;
                }
                else
                {
                    //Delete
                    await DB.DeleteAsync<mdOptionListHeader>(request.Record.ID);
                }
            }
            catch (Exception ex)
            {
                response.ReturnCode = GrpcReturnCode.Error_ByServer;
                response.MsgCode = ex.Message;
                MyAppLog.WriteLog(MyConstant.LogLevel_Critical, "AdminService", "SaveOptionListHeader", "Exception", response.ReturnCode, ex.Message);
            }
            return await Task.FromResult(response);
        }
        //-------------------------------------------------------------------------------------------------------/
        // GetOptionListHeader
        //-------------------------------------------------------------------------------------------------------/
        public override async Task<GetOptionListHeader_Response> GetOptionListHeader(Admin.Services.Empty_Request request, ServerCallContext context)
        {
            var response = new GetOptionListHeader_Response();
            response.ReturnCode = GrpcReturnCode.OK;
            response.MsgCode = "";
            //
            try
            {
                var findRecords = await DB.Find<mdOptionListHeader>().ExecuteAsync();
                //
                if (findRecords != null)
                {
                    findRecords.ForEach(item =>
                    {
                        var grpcItem = new grpcOptionListHeader();
                        ClassHelper.CopyPropertiesData(item, grpcItem);
                        response.OptionListHeader.Add(grpcItem);
                    });
                }
            }
            catch (Exception ex)
            {
                response.ReturnCode = GrpcReturnCode.Error_ByServer;
                response.MsgCode = ex.Message;
                MyAppLog.WriteLog(MyConstant.LogLevel_Critical, "AdminService", "GetOptionListHeader", "Exception", response.ReturnCode, ex.Message);
            }
            //
            return await Task.FromResult(response);
        }
        //-------------------------------------------------------------------------------------------------------/
        // GetOptionList
        //-------------------------------------------------------------------------------------------------------/

        public override async Task<GetOptionList_Response> GetOptionList(Admin.Services.String_Request request, ServerCallContext context)
        {
            var response = new GetOptionList_Response();
            response.ReturnCode = GrpcReturnCode.OK;
            response.MsgCode = "";
            //
            try
            {
                var findRecords = new List<mdOptionList>();
                if (request.StringValue == "")
                {
                    findRecords = await DB.Find<mdOptionList>()
                        .ExecuteAsync();
                }
                else
                {
                    findRecords = await DB.Find<mdOptionList>()
                            .Match(a => a.ListCode == request.StringValue)
                            .ExecuteAsync();
                }
                //
                if (findRecords != null)
                {
                    findRecords.ForEach(item =>
                    {
                        var grpcItem = new grpcOptionList();
                        ClassHelper.CopyPropertiesData(item, grpcItem);
                        response.OptionList.Add(grpcItem);
                    });
                }

            }
            catch (Exception ex)
            {
                response.ReturnCode = GrpcReturnCode.Error_ByServer;
                response.MsgCode = ex.Message;
                MyAppLog.WriteLog(MyConstant.LogLevel_Critical, "AdminService", "GetOptionList", "Exception", response.ReturnCode, ex.Message);
            }
            //
            return await Task.FromResult(response);
        }

        //-------------------------------------------------------------------------------------------------------/
        // SaveFunctionList
        //-------------------------------------------------------------------------------------------------------/
        public override async Task<Admin.Services.Empty_Response> SaveFunctionList(SaveFunctionList_Request request, ServerCallContext context)
        {
            var response = new Admin.Services.Empty_Response();
            response.ReturnCode = GrpcReturnCode.OK;  //OK                                        
            try
            {
                foreach (var item in request.FunctionList)
                {
                    switch (item.UpdMode)
                    {
                        //add new
                        case 1:
                            var newRecord = new mdFunctionList();
                            ClassHelper.CopyPropertiesData(item, newRecord);
                            newRecord.ID = "";
                            newRecord.CreatedOn = DateTime.UtcNow;
                            newRecord.ModifiedOn = DateTime.UtcNow;
                            //
                            await newRecord.SaveAsync();
                            break;

                        //update
                        case 2:
                            var oldRecord = await DB.Find<mdFunctionList>().OneAsync(item.ID);
                            if (oldRecord != null)
                            {
                                ClassHelper.CopyPropertiesData(item, oldRecord);
                                oldRecord.ModifiedOn = DateTime.UtcNow;
                                await oldRecord.SaveAsync();
                            }
                            else
                            {
                                response.ReturnCode = GrpcReturnCode.Error_201;
                            }
                            break;

                        //delete
                        case 3:
                            await DB.DeleteAsync<mdFunctionList>(item.ID);
                            break;
                        default:
                            response.ReturnCode = GrpcReturnCode.Error_BadRequest; //UpdMode = blank
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                response.ReturnCode = GrpcReturnCode.Error_ByServer;
                response.MsgCode = ex.Message;
                MyAppLog.WriteLog(MyConstant.LogLevel_Critical, "AdminService", "SaveFunctionList", "Exception", response.ReturnCode, ex.Message);
            }
            return await Task.FromResult(response);
        }
        //-------------------------------------------------------------------------------------------------------/
        // GetFunctionList
        //-------------------------------------------------------------------------------------------------------/
        public override async Task<GetFunctionList_Response> GetFunctionList(Admin.Services.Empty_Request request, ServerCallContext context)
        {
            var response = new GetFunctionList_Response();
            response.ReturnCode = GrpcReturnCode.OK;
            response.MsgCode = "";
            //
            try
            {
                var findRecords = new List<mdFunctionList>();

                findRecords = await DB.Find<mdFunctionList>().ExecuteAsync();
                //
                if (findRecords != null)
                {
                    findRecords.ForEach(item =>
                    {
                        var grpcItem = new grpcFunctionList();
                        ClassHelper.CopyPropertiesData(item, grpcItem);
                        response.FunctionList.Add(grpcItem);
                    });
                }

            }
            catch (Exception ex)
            {
                response.ReturnCode = GrpcReturnCode.Error_ByServer;
                response.MsgCode = ex.Message;
                MyAppLog.WriteLog(MyConstant.LogLevel_Critical, "AdminService", "GetFunctionList", "Exception", response.ReturnCode, ex.Message);
            }
            //
            return await Task.FromResult(response);
        }

        //-------------------------------------------------------------------------------------------------------/
        // SaveRoleList
        //-------------------------------------------------------------------------------------------------------/
        public override async Task<Admin.Services.Empty_Response> SaveRoleList(SaveRoleList_Request request, ServerCallContext context)
        {
            var response = new Admin.Services.Empty_Response();
            response.ReturnCode = GrpcReturnCode.OK;
            try
            {
                foreach (var item in request.RoleList)
                {
                    switch (item.UpdMode)
                    {
                        //add new
                        case 1:
                            var newRecord = new mdRoleList();
                            ClassHelper.CopyPropertiesData(item, newRecord);
                            newRecord.ID = "";
                            newRecord.CreatedOn = DateTime.UtcNow;
                            newRecord.ModifiedOn = DateTime.UtcNow;
                            //
                            await newRecord.SaveAsync();
                            break;

                        //update
                        case 2:
                            var oldRecord = await DB.Find<mdRoleList>().OneAsync(item.ID);
                            if (oldRecord != null)
                            {
                                ClassHelper.CopyPropertiesData(item, oldRecord);
                                oldRecord.ModifiedOn = DateTime.UtcNow;
                                await oldRecord.SaveAsync();
                            }
                            else
                            {
                                response.ReturnCode = GrpcReturnCode.Error_201;
                            }
                            break;

                        //delete
                        case 3:
                            await DB.DeleteAsync<mdRoleList>(item.ID);
                            break;
                        default:
                            response.ReturnCode = GrpcReturnCode.Error_BadRequest; //UpdMode = blank
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                response.ReturnCode = GrpcReturnCode.Error_ByServer;
                response.MsgCode = ex.Message;
                MyAppLog.WriteLog(MyConstant.LogLevel_Critical, "AdminService", "SaveRoleList", "Exception", response.ReturnCode, ex.Message);
            }
            return await Task.FromResult(response);
        }

        //-------------------------------------------------------------------------------------------------------/
        // GetRoleList
        //-------------------------------------------------------------------------------------------------------/
        public override async Task<GetRoleList_Response> GetRoleList(Admin.Services.Empty_Request request, ServerCallContext context)
        {
            var response = new GetRoleList_Response();
            response.ReturnCode = GrpcReturnCode.OK;
            response.MsgCode = "";
            //
            try
            {
                var findRecords = await DB.Find<mdRoleList>().ExecuteAsync();
                //
                if (findRecords != null)
                {
                    findRecords.ForEach(item =>
                    {
                        var grpcItem = new grpcRoleList();
                        ClassHelper.CopyPropertiesData(item, grpcItem);
                        response.RoleList.Add(grpcItem);
                    });
                }
            }
            catch (Exception ex)
            {
                response.ReturnCode = GrpcReturnCode.Error_ByServer;
                response.MsgCode = ex.Message;
                MyAppLog.WriteLog(MyConstant.LogLevel_Critical, "AdminService", "GetRoleList", "Exception", response.ReturnCode, ex.Message);
            }
            //
            return await Task.FromResult(response);
        }
        //-------------------------------------------------------------------------------------------------------/
        // SaveUserRole
        //-------------------------------------------------------------------------------------------------------/
        public override async Task<Admin.Services.Empty_Response> SaveUserRole(SaveUserRole_Request request, ServerCallContext context)
        {
            var response = new Admin.Services.Empty_Response();
            response.ReturnCode = GrpcReturnCode.OK;
            try
            {
                foreach (var item in request.UserRoles)
                {
                    //Save record to CoresAdmin.UserRole
                    switch (item.UpdMode)
                    {
                        //add new
                        case 1:
                            var newRecord = new mdUserRole();
                            ClassHelper.CopyPropertiesData(item, newRecord);
                            newRecord.ID = "";
                            newRecord.CreatedOn = DateTime.UtcNow;
                            newRecord.ModifiedOn = DateTime.UtcNow;
                            //
                            await newRecord.SaveAsync();
                            break;

                        //update
                        case 2:
                            var oldRecord = await DB.Find<mdUserRole>().OneAsync(item.ID);
                            if (oldRecord != null)
                            {
                                ClassHelper.CopyPropertiesData(item, oldRecord);
                                oldRecord.ModifiedOn = DateTime.UtcNow;
                                await oldRecord.SaveAsync();
                            }
                            else
                            {
                                response.ReturnCode = GrpcReturnCode.Error_201;
                            }
                            break;

                        //delete
                        case 3:
                            await DB.DeleteAsync<mdUserRole>(item.ID);
                            break;
                        default:
                            response.ReturnCode = GrpcReturnCode.Error_BadRequest; //UpdMode = blank
                            break;
                    }
                }
                //
            }
            catch (Exception ex)
            {
                response.ReturnCode = GrpcReturnCode.Error_ByServer;
                response.MsgCode = ex.Message;
                MyAppLog.WriteLog(MyConstant.LogLevel_Critical, "AdminService", "SaveUserRole", "Exception", response.ReturnCode, ex.Message);
            }
            return await Task.FromResult(response);
        }

        //-------------------------------------------------------------------------------------------------------/
        // GetUserRole
        //-------------------------------------------------------------------------------------------------------/
        public override async Task<GetUserRole_Response> GetUserRole(Admin.Services.Empty_Request request, ServerCallContext context)
        {
            var response = new GetUserRole_Response();
            response.ReturnCode = GrpcReturnCode.OK;
            response.MsgCode = "";
            //
            try
            {
                var findRecords = await DB.Find<mdUserRole>().ExecuteAsync();
                //
                if (findRecords != null)
                {
                    findRecords.ForEach(item =>
                    {
                        var grpcItem = new grpcUserRole();
                        ClassHelper.CopyPropertiesData(item, grpcItem);
                        response.UserRoles.Add(grpcItem);
                    });
                }
            }
            catch (Exception ex)
            {
                response.ReturnCode = GrpcReturnCode.Error_ByServer;
                response.MsgCode = ex.Message;
                MyAppLog.WriteLog(MyConstant.LogLevel_Critical, "AdminService", "GetUserRole", "Exception", response.ReturnCode, ex.Message);
            }
            //
            return await Task.FromResult(response);
        }
        //-------------------------------------------------------------------------------------------------------/
        // SaveMenuGroup
        //-------------------------------------------------------------------------------------------------------/
        public override async Task<Admin.Services.Empty_Response> SaveMenuGroup(SaveMenuGroup_Request request, ServerCallContext context)
        {
            var response = new Admin.Services.Empty_Response();
            response.ReturnCode = GrpcReturnCode.OK;
            try
            {
                foreach (var item in request.MenuGroups)
                {
                    switch (item.UpdMode)
                    {
                        //add new
                        case 1:
                            var newRecord = new mdMenuGroup();
                            ClassHelper.CopyPropertiesData(item, newRecord);
                            newRecord.ID = "";
                            //
                            await newRecord.SaveAsync();
                            break;

                        //update
                        case 2:
                            var oldRecord = await DB.Find<mdMenuGroup>().OneAsync(item.ID);
                            if (oldRecord != null)
                            {
                                ClassHelper.CopyPropertiesData(item, oldRecord);
                                await oldRecord.SaveAsync();
                            }
                            else
                            {
                                response.ReturnCode = GrpcReturnCode.Error_201;
                            }
                            break;

                        //delete
                        case 3:
                            await DB.DeleteAsync<mdMenuGroup>(item.ID);
                            break;
                        default:
                            response.ReturnCode = GrpcReturnCode.Error_BadRequest; //UpdMode = blank
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                response.ReturnCode = GrpcReturnCode.Error_ByServer;
                response.MsgCode = ex.Message;
                MyAppLog.WriteLog(MyConstant.LogLevel_Critical, "AdminService", "SaveMenuGroup", "Exception", response.ReturnCode, ex.Message);
            }
            return await Task.FromResult(response);
        }
        //-------------------------------------------------------------------------------------------------------/
        // GetMenuGroup
        //-------------------------------------------------------------------------------------------------------/
        public override async Task<GetMenuGroup_Response> GetMenuGroup(Admin.Services.Empty_Request request, ServerCallContext context)
        {
            var response = new GetMenuGroup_Response();
            response.ReturnCode = GrpcReturnCode.OK;
            response.MsgCode = "";
            //
            try
            {
                var query = DB.Find<mdMenuGroup>();
                var findRecords = await query.ExecuteAsync();
                //
                if (findRecords != null)
                {
                    findRecords.ForEach(item =>
                    {
                        var grpcItem = new grpcMenuGroup();
                        ClassHelper.CopyPropertiesData(item, grpcItem);
                        response.MenuGroups.Add(grpcItem);
                    });
                }
            }
            catch (Exception ex)
            {
                response.ReturnCode = GrpcReturnCode.Error_ByServer;
                response.MsgCode = ex.Message;
                MyAppLog.WriteLog(MyConstant.LogLevel_Critical, "AdminService", "GetMenuGroup", "Exception", response.ReturnCode, ex.Message);
            }
            //
            return await Task.FromResult(response);
        }
        //-------------------------------------------------------------------------------------------------------/
        // SaveMenuDetail
        //-------------------------------------------------------------------------------------------------------/
        public override async Task<Admin.Services.Empty_Response> SaveMenuDetail(SaveMenuDetail_Request request, ServerCallContext context)
        {
            var response = new Admin.Services.Empty_Response();
            response.ReturnCode = GrpcReturnCode.OK;
            try
            {
                foreach (var item in request.MenuDetails)
                {
                    switch (item.UpdMode)
                    {
                        //add new
                        case 1:
                            var newRecord = new mdMenuDetail();
                            ClassHelper.CopyPropertiesData(item, newRecord);
                            newRecord.ID = "";
                            //
                            await newRecord.SaveAsync();
                            break;

                        //update
                        case 2:
                            var oldRecord = await DB.Find<mdMenuDetail>().OneAsync(item.ID);
                            if (oldRecord != null)
                            {
                                ClassHelper.CopyPropertiesData(item, oldRecord);
                                await oldRecord.SaveAsync();
                            }
                            else
                            {
                                response.ReturnCode = GrpcReturnCode.Error_201;
                            }
                            break;

                        //delete
                        case 3:
                            await DB.DeleteAsync<mdMenuDetail>(item.ID);
                            break;
                        default:
                            response.ReturnCode = GrpcReturnCode.Error_BadRequest; //UpdMode = blank
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                response.ReturnCode = GrpcReturnCode.Error_ByServer;
                response.MsgCode = ex.Message;
                MyAppLog.WriteLog(MyConstant.LogLevel_Critical, "AdminService", "SaveMenuDetail", "Exception", response.ReturnCode, ex.Message);
            }
            return await Task.FromResult(response);
        }
        //-------------------------------------------------------------------------------------------------------/
        // GetMenuDetail
        //-------------------------------------------------------------------------------------------------------/
        public override async Task<GetMenuDetail_Response> GetMenuDetail(Admin.Services.Empty_Request request, ServerCallContext context)
        {
            var response = new GetMenuDetail_Response();
            response.ReturnCode = GrpcReturnCode.OK;
            response.MsgCode = "";
            //
            try
            {
                var findRecords = await DB.Find<mdMenuDetail>().ExecuteAsync();
                //
                if (findRecords != null)
                {
                    findRecords.ForEach(item =>
                    {
                        var grpcItem = new grpcMenuDetail();
                        ClassHelper.CopyPropertiesData(item, grpcItem);
                        response.MenuDetails.Add(grpcItem);
                    });
                }
            }
            catch (Exception ex)
            {
                response.ReturnCode = GrpcReturnCode.Error_ByServer;
                response.MsgCode = ex.Message;
                MyAppLog.WriteLog(MyConstant.LogLevel_Critical, "AdminService", "GetMenuDetail", "Exception", response.ReturnCode, ex.Message);
            }
            //
            return await Task.FromResult(response);
        }
        //-------------------------------------------------------------------------------------------------------/
        // SaveRoleDetail
        //-------------------------------------------------------------------------------------------------------/
        public override async Task<Admin.Services.Empty_Response> SaveRoleDetail(SaveRoleDetail_Request request, ServerCallContext context)
        {
            var response = new Admin.Services.Empty_Response();
            response.ReturnCode = GrpcReturnCode.OK;
            try
            {
                foreach (var item in request.RoleDetail)
                {
                    switch (item.UpdMode)
                    {
                        //add new
                        case 1:
                            var newRecord = new mdRoleDetail();
                            ClassHelper.CopyPropertiesData(item, newRecord);
                            newRecord.ID = "";
                            newRecord.CreatedOn = DateTime.UtcNow;
                            newRecord.ModifiedOn = DateTime.UtcNow;
                            //
                            await newRecord.SaveAsync();
                            break;

                        //update
                        case 2:
                            var oldRecord = await DB.Find<mdRoleDetail>().OneAsync(item.ID);
                            if (oldRecord != null)
                            {
                                ClassHelper.CopyPropertiesData(item, oldRecord);
                                oldRecord.ModifiedOn = DateTime.UtcNow;
                                await oldRecord.SaveAsync();
                            }
                            else
                            {
                                response.ReturnCode = GrpcReturnCode.Error_201;
                            }
                            break;

                        //delete
                        case 3:
                            await DB.DeleteAsync<mdRoleDetail>(item.ID);
                            break;
                        default:
                            response.ReturnCode = GrpcReturnCode.Error_BadRequest; //UpdMode = blank
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                response.ReturnCode = GrpcReturnCode.Error_ByServer;
                response.MsgCode = ex.Message;
                MyAppLog.WriteLog(MyConstant.LogLevel_Critical, "AdminService", "SaveRoleDetail", "Exception", response.ReturnCode, ex.Message);
            }
            return await Task.FromResult(response);
        }
        //-------------------------------------------------------------------------------------------------------/
        // GetRoleDetail
        //-------------------------------------------------------------------------------------------------------/
        public override async Task<GetRoleDetail_Response> GetRoleDetail(Admin.Services.String_Request request, ServerCallContext context)
        {
            var response = new GetRoleDetail_Response();
            response.ReturnCode = GrpcReturnCode.OK;
            response.MsgCode = "";
            //
            try
            {
                var query = DB.Find<mdRoleDetail>();
                if (!string.IsNullOrWhiteSpace(request.StringValue))
                {
                    query.Match(x => x.RoleID == request.StringValue);
                }
                var findRecords = await query.ExecuteAsync();
                //
                if (findRecords != null)
                {
                    findRecords.ForEach(item =>
                    {
                        var grpcItem = new grpcRoleDetail();
                        ClassHelper.CopyPropertiesData(item, grpcItem);
                        response.RoleDetail.Add(grpcItem);
                    });
                }
            }
            catch (Exception ex)
            {
                response.ReturnCode = GrpcReturnCode.Error_ByServer;
                response.MsgCode = ex.Message;
                MyAppLog.WriteLog(MyConstant.LogLevel_Critical, "AdminService", "GetRoleDetail", "Exception", response.ReturnCode, ex.Message);
            }
            //
            return await Task.FromResult(response);
        }
        //-------------------------------------------------------------------------------------------------------/
        // DeleteRoleDetail
        //-------------------------------------------------------------------------------------------------------/

        public override async Task<Admin.Services.Empty_Response> DeleteRoleDetail(DeleteRoleDetail_Request request, ServerCallContext context)
        {
            var response = new Admin.Services.Empty_Response();
            response.ReturnCode = GrpcReturnCode.OK;
            try
            {
                await DB.DeleteAsync<mdRoleDetail>(a => a.RoleID == request.RoleID);

            }
            catch (Exception ex)
            {
                response.ReturnCode = GrpcReturnCode.Error_ByServer;
                response.MsgCode = ex.Message;
                MyAppLog.WriteLog(MyConstant.LogLevel_Critical, "AdminService", "DeleteRoleDetail", "Exception", response.ReturnCode, ex.Message);
            }
            return await Task.FromResult(response);
        }
        //-------------------------------------------------------------------------------------------------------/
        // SaveSettingMaster
        //-------------------------------------------------------------------------------------------------------/
        public override async Task<Admin.Services.Empty_Response> SaveSettingMaster(SaveSettingMaster_Request request, ServerCallContext context)
        {
            var response = new Admin.Services.Empty_Response();
            response.ReturnCode = GrpcReturnCode.OK;
            try
            {
                foreach (var item in request.SettingMasters)
                {
                    switch (item.UpdMode)
                    {
                        //add new
                        case 1:
                            var newRecord = new mdSettingMaster();
                            ClassHelper.CopyPropertiesData(item, newRecord);
                            newRecord.ID = "";
                            newRecord.ModifiedOn = DateTime.UtcNow;
                            //
                            await newRecord.SaveAsync();
                            break;

                        //update
                        case 2:
                            var oldRecord = await DB.Find<mdSettingMaster>().OneAsync(item.ID);
                            if (oldRecord != null)
                            {
                                ClassHelper.CopyPropertiesData(item, oldRecord);
                                oldRecord.ModifiedOn = DateTime.UtcNow;
                                //
                                await oldRecord.SaveAsync();
                            }
                            else
                            {
                                response.ReturnCode = GrpcReturnCode.Error_201;
                            }
                            break;

                        //delete
                        case 3:
                            await DB.DeleteAsync<mdSettingMaster>(item.ID);
                            break;
                        default:
                            response.ReturnCode = GrpcReturnCode.Error_BadRequest; //UpdMode = blank
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                response.ReturnCode = GrpcReturnCode.Error_ByServer;
                response.MsgCode = ex.Message;
                MyAppLog.WriteLog(MyConstant.LogLevel_Critical, "AdminService", "SaveSettingMaster", "Exception", response.ReturnCode, ex.Message);
            }
            return await Task.FromResult(response);
        }

        //-------------------------------------------------------------------------------------------------------/
        // GetSettingMaster
        //-------------------------------------------------------------------------------------------------------/
        public override async Task<GetSettingMaster_Response> GetSettingMaster(Admin.Services.Empty_Request request, ServerCallContext context)
        {
            var response = new GetSettingMaster_Response();
            response.ReturnCode = GrpcReturnCode.OK;
            response.MsgCode = "";
            //
            try
            {
                var findRecords = await DB.Find<mdSettingMaster>().ExecuteAsync();
                //
                if (findRecords != null)
                {
                    findRecords.ForEach(item =>
                    {
                        var grpcItem = new grpcSettingMaster();
                        ClassHelper.CopyPropertiesData(item, grpcItem);
                        response.SettingMasters.Add(grpcItem);
                    });
                }
            }
            catch (Exception ex)
            {
                response.ReturnCode = GrpcReturnCode.Error_ByServer;
                response.MsgCode = ex.Message;
                MyAppLog.WriteLog(MyConstant.LogLevel_Critical, "AdminService", "GetSettingMaster", "Exception", response.ReturnCode, ex.Message);
            }
            //
            return await Task.FromResult(response);
        }

        //-------------------------------------------------------------------------------------------------------/
        // SaveUserAccount
        //-------------------------------------------------------------------------------------------------------/
        public override async Task<Admin.Services.String_Response> SaveUserAccount(SaveUserAccount_Request request, ServerCallContext context)
        {
            var response = new Admin.Services.String_Response();
            response.ReturnCode = GrpcReturnCode.OK;
            try
            {
                if (request.Record.UpdMode != 3)
                {
                    //Get Save password
                    var oldRecord = await DB.Find<mdUserAccount>()
                                            .Match(x => x.ID == request.Record.ID)
                                            .ExecuteFirstAsync();
                    string oldEnctyptedPassWord = "";
                    if (oldRecord != null)
                    {
                        oldEnctyptedPassWord = oldRecord.Password;
                    }

                    //Insert || Update
                    var saveRecord = new mdUserAccount();
                    ClassHelper.CopyPropertiesData(request.Record, saveRecord);
                    //Insert
                    if (request.Record.UpdMode == 1) saveRecord.ID = saveRecord.GenerateNewID();
                    //Encrypt password
                    if (!string.IsNullOrWhiteSpace(saveRecord.Password))
                    {
                        saveRecord.Password = saveRecord.Password;
                    }
                    else
                    {
                        saveRecord.Password = oldEnctyptedPassWord;
                    }
                    //
                    await saveRecord.SaveAsync();
                    //return saved ID
                    response.StringValue = saveRecord.ID;
                }
                else
                {
                    //Delete
                    await DB.DeleteAsync<mdUserAccount>(request.Record.ID);
                }
            }
            catch (Exception ex)
            {
                response.ReturnCode = GrpcReturnCode.Error_ByServer;
                response.MsgCode = ex.Message;
                MyAppLog.WriteLog(MyConstant.LogLevel_Critical, "AdminService", "SaveUserAccount", "Exception", response.ReturnCode, ex.Message);
            }
            return await Task.FromResult(response);
        }

        //-------------------------------------------------------------------------------------------------------/
        // GetUserAccount
        //-------------------------------------------------------------------------------------------------------/
        public override async Task<GetUserAccount_Response> GetUserAccount(Admin.Services.String_Request request, ServerCallContext context)
        {
            var response = new GetUserAccount_Response();
            response.ReturnCode = GrpcReturnCode.OK;
            response.MsgCode = "";
            //
            try
            {
                var query = DB.Find<mdUserAccount>();
                if (!string.IsNullOrWhiteSpace(request.StringValue))
                {
                    query.Match(a => a.UserName == request.StringValue);
                }
                var findRecords = await query.ExecuteAsync();
                //
                if (findRecords != null && findRecords.Count > 0)
                {
                    findRecords.ForEach(item =>
                    {
                        var grpcItem = new grpcUserAccount();
                        ClassHelper.CopyPropertiesData(item, grpcItem);
                        //Clear password
                        grpcItem.Password = "";
                        //
                        response.UserAccounts.Add(grpcItem);
                    });
                }
            }
            catch (Exception ex)
            {
                response.ReturnCode = GrpcReturnCode.Error_ByServer;
                response.MsgCode = ex.Message;
                MyAppLog.WriteLog(MyConstant.LogLevel_Critical, "AdminService", "GetUserAccount", "Exception", response.ReturnCode, ex.Message);
            }
            //
            return await Task.FromResult(response);
        }
        //-------------------------------------------------------------------------------------------------------/
        // GetVoucherNo
        //-------------------------------------------------------------------------------------------------------/
        public override async Task<Admin.Services.String_Response> GetVoucherNo(Admin.Services.String_Request request, ServerCallContext context)
        {
            var response = new Admin.Services.String_Response();
            response.ReturnCode = GrpcReturnCode.OK;
            response.MsgCode = "";
            //
            string newVoucherNo = "";
            //
            try
            {
                newVoucherNo =  await MyVoucher.GetVoucherNo(request.StringValue);
            }
            catch (Exception ex)
            {
                response.ReturnCode = GrpcReturnCode.Error_ByServer;
                response.MsgCode = ex.Message;
                MyAppLog.WriteLog(MyConstant.LogLevel_Critical, "AdminService", "GetVoucherNo", "Exception", response.ReturnCode, ex.Message);
            }
            //
            response.StringValue = newVoucherNo;
            return response;
        }
        //-------------------------------------------------------------------------------------------------------/
        // Commit VoucherNo
        //-------------------------------------------------------------------------------------------------------/
        public override async Task<Admin.Services.String_Response> CommitVoucherNo(Admin.Services.CommitVoucherNo_Request request, ServerCallContext context)
        {
            var response = new Admin.Services.String_Response();
            response.ReturnCode = GrpcReturnCode.OK;
            response.MsgCode = "";
            string committedVoucherNo = request.VoucherNo;
            //
            try
            {
                committedVoucherNo = await MyVoucher.CommitVoucherNo(request.VoucherCode, request.VoucherNo);
                if (string.IsNullOrWhiteSpace(committedVoucherNo))
                {
                    response.ReturnCode = GrpcReturnCode.Error_201;
                }
            }
            catch (Exception ex)
            {
                response.ReturnCode = GrpcReturnCode.Error_ByServer;
                response.MsgCode = ex.Message;
                MyAppLog.WriteLog(MyConstant.LogLevel_Critical, "AdminService", "CommitVoucherNo", "Exception", response.ReturnCode, ex.Message);
            }
            //
            response.StringValue = committedVoucherNo;
            return await Task.FromResult(response);
        }
        //-------------------------------------------------------------------------------------------------------/
        // SubscribeToNotifications
        //-------------------------------------------------------------------------------------------------------/
        public override async Task<Empty_Response> SubscribeToNotifications(SubscribeToNotifications_Request request, ServerCallContext context)
        {
            var response = new Empty_Response();
            response.ReturnCode = GrpcReturnCode.OK;
            response.MsgCode = "";
            //
            try
            {
                //Duplicated check
                var updateRecord = await DB.Find<mdNotificationSubcribe>()
                                          .Match(a => a.Url == request.Url)
                                          .Match(a => a.P256dh == request.P256Dh)
                                          .Match(a => a.Auth == request.Auth)
                                          .ExecuteFirstAsync();
                if (updateRecord != null)
                {
                    return response;
                }

                //Insert new
                var newRecord = new mdNotificationSubcribe();
                newRecord.NotificationSubscriptionId = request.NotificationSubscriptionId;
                newRecord.UserId = request.Credential.Username;
                newRecord.Url = request.Url;
                newRecord.P256dh = request.P256Dh;
                newRecord.Auth = request.Auth;
                newRecord.ModifiedOn = DateTime.UtcNow;
                newRecord.UpdMode = 1;
                //
                await newRecord.SaveAsync();
            }
            catch (Exception ex)
            {
                response.ReturnCode = GrpcReturnCode.Error_ByServer;
                response.MsgCode = ex.Message;
                MyAppLog.WriteLog(MyConstant.LogLevel_Critical, "AdminService", "SubscribeToNotifications", "Exception", response.ReturnCode, ex.Message);
            }
            //
            return await Task.FromResult(response);
        }
        //-------------------------------------------------------------------------------------------------------/
        // GetNotificationSubscribe
        //-------------------------------------------------------------------------------------------------------/
        public override async Task<GetNotificationSubscribe_Response> GetNotificationSubscribe(String_Request request, ServerCallContext context)
        {
            var response = new GetNotificationSubscribe_Response();
            response.ReturnCode = GrpcReturnCode.OK;
            response.MsgCode = "";
            //
            try
            {
                //Duplicated check
                var findRecords = await DB.Find<mdNotificationSubcribe>()
                                          .Match(a => a.UserId == request.StringValue)
                                          .ExecuteAsync();
                if (findRecords != null)
                {
                    foreach (var item in findRecords)
                    {
                        var grpcItem = new grpcNotificationSubcribeModel();
                        ClassHelper.CopyPropertiesData(item, grpcItem);
                        grpcItem.P256Dh = item.P256dh;
                        //
                        response.Records.Add(grpcItem);
                    }
                }
            }
            catch (Exception ex)
            {
                response.ReturnCode = GrpcReturnCode.Error_ByServer;
                response.MsgCode = ex.Message;
                MyAppLog.WriteLog(MyConstant.LogLevel_Critical, "AdminService", "SubscribeToNotifications", "Exception", response.ReturnCode, ex.Message);
            }
            //
            return await Task.FromResult(response);
        }



    }//End class
}//End namespace
