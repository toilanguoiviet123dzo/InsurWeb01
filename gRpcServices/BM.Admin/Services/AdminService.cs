using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Gosu.Service.Models;
using Gosu.Admin.Services;
using MongoDB.Entities;
using MongoDB.Driver.Linq;
using MongoDB.Driver;
using MongoDB.Bson;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using System.Text;
using Gosu.Service;
using Gosu.Common;
using Grpc.Net.Client;


namespace Gosu.Service.Services
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
                    .Match(x => x.UserName ==  request.UserName && x.Password == MD5Crypto.Encrypt(request.Password))
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
                MyAppLog.WriteLog(MyConstant.LogLevel_Critical, "AdminService", "GrpcLogin", "1", response.ReturnCode, ex.Message);
            }
            //
            return await Task.FromResult(response);
        }
        //-------------------------------------------------------------------------------------------------------/
        // SaveServiceList
        //-------------------------------------------------------------------------------------------------------/
        public override async Task<Admin.Services.Empty_Response> SaveServiceList(SaveServiceList_Request request, ServerCallContext context)
        {
            var response = new Admin.Services.Empty_Response();
            response.ReturnCode = GrpcReturnCode.OK;  //OK
            //
            try
            {
                foreach (grpcServiceList item in request.ServiceList)
                {
                    switch (item.UpdMode)
                    {
                        //add new
                        case 1:
                            var newRecord = new mdServiceList();
                            ClassHelper.CopyPropertiesData(item, newRecord);
                            newRecord.ID = "";
                            newRecord.CreatedOn = DateTime.UtcNow;
                            newRecord.ModifiedOn = DateTime.UtcNow;
                            //
                            await newRecord.SaveAsync();
                            break;

                        //update
                        case 2:
                            var oldRecord = await DB.Find<mdServiceList>().OneAsync(item.ID);
                            if (oldRecord != null)
                            {
                                ClassHelper.CopyPropertiesData(item, oldRecord);
                                //
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
                            await DB.DeleteAsync<mdServiceList>(item.ID);
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
                MyAppLog.WriteLog(MyConstant.LogLevel_Critical, "AdminService", "SaveServiceList", "1", response.ReturnCode, ex.Message);
            }
            //
            return await Task.FromResult(response);
        }
        //-------------------------------------------------------------------------------------------------------/
        // GetServiceList
        //-------------------------------------------------------------------------------------------------------/

        public override async Task<GetServiceList_Response> GetServiceList(Admin.Services.Empty_Request request, ServerCallContext context)
        {
            var response = new GetServiceList_Response();
            response.ReturnCode = GrpcReturnCode.OK;
            response.MsgCode = "";
            //
            try
            {
                var findRecords = await DB.Find<mdServiceList>().ExecuteAsync();
                //
                if (findRecords != null)
                {
                    findRecords.ForEach(item =>
                    {
                        var grpcItem = new grpcServiceList();
                        ClassHelper.CopyPropertiesData(item, grpcItem);
                        response.ServiceList.Add(grpcItem);
                    });
                }
            }
            catch (Exception ex)
            {
                response.ReturnCode = GrpcReturnCode.Error_ByServer;
                response.MsgCode = ex.Message;
                MyAppLog.WriteLog(MyConstant.LogLevel_Critical, "AdminService", "GetServiceList", "1", response.ReturnCode, ex.Message);
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
                MyAppLog.WriteLog(MyConstant.LogLevel_Critical, "AdminService", "SaveOptionList", "1", response.ReturnCode, ex.Message);
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
                MyAppLog.WriteLog(MyConstant.LogLevel_Critical, "AdminService", "SaveOptionListHeader", "1", response.ReturnCode, ex.Message);
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
                MyAppLog.WriteLog(MyConstant.LogLevel_Critical, "AdminService", "GetOptionListHeader", "1", response.ReturnCode, ex.Message);
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
                MyAppLog.WriteLog(MyConstant.LogLevel_Critical, "AdminService", "GetOptionList", "1", response.ReturnCode, ex.Message);
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
                MyAppLog.WriteLog(MyConstant.LogLevel_Critical, "AdminService", "SaveFunctionList", "1", response.ReturnCode, ex.Message);
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
                MyAppLog.WriteLog(MyConstant.LogLevel_Critical, "AdminService", "GetFunctionList", "1", response.ReturnCode, ex.Message);
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
                MyAppLog.WriteLog(MyConstant.LogLevel_Critical, "AdminService", "SaveRoleList", "1", response.ReturnCode, ex.Message);
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
                MyAppLog.WriteLog(MyConstant.LogLevel_Critical, "AdminService", "GetRoleList", "1", response.ReturnCode, ex.Message);
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
                    //Save record to GosuAdmin.UserRole
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
                MyAppLog.WriteLog(MyConstant.LogLevel_Critical, "AdminService", "SaveUserRole", "1", response.ReturnCode, ex.Message);
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
                MyAppLog.WriteLog(MyConstant.LogLevel_Critical, "AdminService", "GetUserRole", "1", response.ReturnCode, ex.Message);
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
                MyAppLog.WriteLog(MyConstant.LogLevel_Critical, "AdminService", "SaveMenuGroup", "1", response.ReturnCode, ex.Message);
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
                MyAppLog.WriteLog(MyConstant.LogLevel_Critical, "AdminService", "GetMenuGroup", "1", response.ReturnCode, ex.Message);
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
                MyAppLog.WriteLog(MyConstant.LogLevel_Critical, "AdminService", "SaveMenuDetail", "1", response.ReturnCode, ex.Message);
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
                MyAppLog.WriteLog(MyConstant.LogLevel_Critical, "AdminService", "GetMenuDetail", "1", response.ReturnCode, ex.Message);
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
                MyAppLog.WriteLog(MyConstant.LogLevel_Critical, "AdminService", "SaveRoleDetail", "1", response.ReturnCode, ex.Message);
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
                    query.Match(x => x.RoleID ==  request.StringValue);
                }
                var findRecords  = await query.ExecuteAsync();
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
                MyAppLog.WriteLog(MyConstant.LogLevel_Critical, "AdminService", "GetRoleDetail", "1", response.ReturnCode, ex.Message);
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
                MyAppLog.WriteLog(MyConstant.LogLevel_Critical, "AdminService", "DeleteRoleDetail", "1", response.ReturnCode, ex.Message);
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
                MyAppLog.WriteLog(MyConstant.LogLevel_Critical, "AdminService", "SaveSettingMaster", "1", response.ReturnCode, ex.Message);
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
                MyAppLog.WriteLog(MyConstant.LogLevel_Critical, "AdminService", "GetSettingMaster", "1", response.ReturnCode, ex.Message);
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
                        saveRecord.Password = MD5Crypto.Encrypt(saveRecord.Password);
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
                MyAppLog.WriteLog(MyConstant.LogLevel_Critical, "AdminService", "SaveUserAccount", "1", response.ReturnCode, ex.Message);
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
                MyAppLog.WriteLog(MyConstant.LogLevel_Critical, "AdminService", "GetUserAccount", "1", response.ReturnCode, ex.Message);
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
            //Default Voucher No
            string currentYear = DateTime.Now.ToString("yy");
            string newVoucherNo = currentYear + "000001";
            //
            try
            {
                //Get from DB + 1
                var findRecords = await DB.Find<mdVoucherMaster>()
                                          .Match(a => a.VoucherCode == request.StringValue)
                                          .ExecuteFirstAsync();
                //
                if (findRecords != null)
                {
                    //Check same year
                    if (!String.IsNullOrWhiteSpace(findRecords.CurrentVoucherNo) && currentYear == findRecords.CurrentVoucherNo.Left(2))
                    {
                        var nextSeq = findRecords.CurrentVoucherNo.Right(6).ToInt() + 1;
                        newVoucherNo = currentYear + nextSeq.ToString("000000");
                    }
                }
                else
                {
                    //Add new master record
                    var newRecord = new mdVoucherMaster();
                    newRecord.VoucherCode = request.StringValue;
                    newRecord.CurrentVoucherNo = currentYear + "000000"; ;
                    newRecord.MinVoucherNo = "00000000";
                    newRecord.MaxVoucherNo = "99999999";
                    newRecord.ModifiedOn = DateTime.UtcNow;
                    newRecord.UpdMode = 1;
                    //
                    await newRecord.SaveAsync();
                }
            }
            catch (Exception ex)
            {
                response.ReturnCode = GrpcReturnCode.Error_ByServer;
                response.MsgCode = ex.Message;
                MyAppLog.WriteLog(MyConstant.LogLevel_Critical, "AdminService", "GetCompenRequest", "1", response.ReturnCode, ex.Message);
            }
            //
            response.StringValue = newVoucherNo;
            return await Task.FromResult(response);
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
                //Check for dublicated
                var findRecords = await DB.Find<mdVoucherMaster>()
                                          .Match(a => a.CurrentVoucherNo == request.VoucherNo && a.VoucherCode == request.VoucherCode)
                                          .ExecuteFirstAsync();
                //
                if (findRecords != null)
                {
                    var getNewVoucherRequest = new Admin.Services.String_Request();
                    getNewVoucherRequest.StringValue = request.VoucherCode;
                    var responseGetNewVoucherNo = await GetVoucherNo(getNewVoucherRequest, null);
                    if (responseGetNewVoucherNo != null && responseGetNewVoucherNo.ReturnCode == GrpcReturnCode.OK)
                    {
                        committedVoucherNo = responseGetNewVoucherNo.StringValue;
                    }
                }
                //Commit
                var updateRecord = await DB.Find<mdVoucherMaster>()
                                          .Match(a => a.VoucherCode == request.VoucherCode)
                                          .ExecuteFirstAsync();
                if (updateRecord != null)
                {
                    updateRecord.CurrentVoucherNo = committedVoucherNo;
                    updateRecord.ModifiedOn = DateTime.UtcNow;
                    updateRecord.UpdMode = 2;
                    //
                    await updateRecord.SaveAsync();
                }
                else
                {
                    //Add new master record
                    var newRecord = new mdVoucherMaster();
                    newRecord.VoucherCode = request.VoucherCode;
                    newRecord.CurrentVoucherNo = committedVoucherNo;
                    newRecord.MinVoucherNo = "00000000";
                    newRecord.MaxVoucherNo = "99999999";
                    newRecord.ModifiedOn = DateTime.UtcNow;
                    newRecord.UpdMode = 1;
                    //
                    await newRecord.SaveAsync();
                }
            }
            catch (Exception ex)
            {
                response.ReturnCode = GrpcReturnCode.Error_ByServer;
                response.MsgCode = ex.Message;
                MyAppLog.WriteLog(MyConstant.LogLevel_Critical, "AdminService", "CommitVoucherNo", "1", response.ReturnCode, ex.Message);
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
                MyAppLog.WriteLog(MyConstant.LogLevel_Critical, "AdminService", "SubscribeToNotifications", "1", response.ReturnCode, ex.Message);
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
                MyAppLog.WriteLog(MyConstant.LogLevel_Critical, "AdminService", "SubscribeToNotifications", "1", response.ReturnCode, ex.Message);
            }
            //
            return await Task.FromResult(response);
        }



    }//End class
}//End name space
