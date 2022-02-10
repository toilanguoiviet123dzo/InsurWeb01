using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Cores.Service.Models;
using Claim.Services;
using MongoDB.Entities;
using MongoDB.Driver.Linq;
using MongoDB.Driver;
using MongoDB.Bson;
using Google.Protobuf;
//using Google.Protobuf.WellKnownTypes;
using System.Text;
using Cores.Service;
using Grpc.Net.Client;
using Cores.Grpc.Client;
using Cores.Helpers;
using BlazorApp.Server.Common;
using BlazorApp.Server.Models;
using Resource.Services;
using Cores.Utilities;

namespace BlazorApp.Server.Services
{
    public class ClaimService : grpcClaimService.grpcClaimServiceBase
    {
        private readonly ILogger<ClaimService> _logger;

        public ClaimService(ILogger<ClaimService> logger)
        {
            _logger = logger;
        }
        //-------------------------------------------------------------------------------------------------------/
        // SaveClaimRequest
        //-------------------------------------------------------------------------------------------------------/
        public override async Task<Claim.Services.String_Response> SaveClaimRequest(SaveClaimRequest_Request request, ServerCallContext context)
        {
            var response = new Claim.Services.String_Response();
            response.ReturnCode = GrpcReturnCode.OK;
            try
            {
                switch (request.ClaimRequest.UpdMode)
                {
                    //add new
                    case 1:
                        var newRecord = new mdClaimRequest();
                        ClassHelper.CopyPropertiesData(request.ClaimRequest, newRecord);
                        newRecord.ID = newRecord.GenerateNewID();
                        newRecord.ModifiedOn = DateTime.UtcNow;
                        newRecord.TextSearch = newRecord.CusFullname.RemoveVietnameseSign();

                        //Commit VoucherNo
                        var commitedVovoucher = await MyVoucher.CommitVoucherNo("001", newRecord.ClaimNo);
                        if (!string.IsNullOrWhiteSpace(commitedVovoucher))
                        {
                            newRecord.ClaimNo = commitedVovoucher;
                        }
                        //
                        await newRecord.SaveAsync();
                        break;

                    //update
                    case 2:
                        var oldRecord = await DB.Find<mdClaimRequest>().OneAsync(request.ClaimRequest.ID);
                        if (oldRecord != null)
                        {
                            ClassHelper.CopyPropertiesData(request.ClaimRequest, oldRecord);
                            oldRecord.ModifiedOn = DateTime.UtcNow;
                            oldRecord.TextSearch = oldRecord.CusFullname.RemoveVietnameseSign();
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
                        //mdClaimRequest
                        var deleteClaim = await DB.Find<mdClaimRequest>()
                                                   .Match(x => x.ClaimNo == request.ClaimRequest.ClaimNo)
                                                   .ExecuteFirstAsync();
                        if (deleteClaim != null)
                        {
                            TaskHelper.RunBg(async () =>
                            {
                                try
                                {
                                    //Delete resource
                                    ResourceService.DeleteResourceFilesByOwner(deleteClaim.ClaimNo);

                                    //Delete claim
                                    await DB.DeleteAsync<mdClaimRequest>(deleteClaim.ID);
                                }
                                catch { }
                            });
                        }
                        //
                        break;
                    default:
                        response.ReturnCode = GrpcReturnCode.Error_BadRequest; //UpdMode = blank
                        break;
                }
            }
            catch (Exception ex)
            {
                response.ReturnCode = GrpcReturnCode.Error_ByServer;
                response.MsgCode = ex.Message;
                MyAppLog.WriteLog(MyConstant.LogLevel_Critical, "ClaimService", "SaveClaimRequest", "Exception", response.ReturnCode, ex.Message);
            }
            return await Task.FromResult(response);
        }


        //-------------------------------------------------------------------------------------------------------/
        // GetClaimRequest
        //-------------------------------------------------------------------------------------------------------/
        public override async Task<GetClaimRequest_Response> GetClaimRequest(Claim.Services.String_Request request, ServerCallContext context)
        {
            var response = new GetClaimRequest_Response();
            response.ReturnCode = GrpcReturnCode.OK;
            response.MsgCode = "";
            //
            try
            {
                var findRecord = await DB.Find<mdClaimRequest>()
                                          .Match(a => a.ClaimNo == request.StringValue)
                                          .ExecuteFirstAsync();
                //
                if (findRecord == null)
                {
                    response.ReturnCode = GrpcReturnCode.Error_201;
                    return await Task.FromResult(response);
                }
                else
                {
                    //OK
                    response.ClaimRequest = new grpcClaimRequestModel();
                    ClassHelper.CopyPropertiesData(findRecord, response.ClaimRequest);
                }
            }
            catch (Exception ex)
            {
                response.ReturnCode = GrpcReturnCode.Error_ByServer;
                response.MsgCode = ex.Message;
                MyAppLog.WriteLog(MyConstant.LogLevel_Critical, "ClaimService", "GetClaimRequest", "Exception", response.ReturnCode, ex.Message);
            }
            //
            return await Task.FromResult(response);
        }

        //-------------------------------------------------------------------------------------------------------/
        // GetCustomerInfo
        //-------------------------------------------------------------------------------------------------------/
        public override async Task<GetCustomerInfo_Response> GetCustomerInfo(Claim.Services.String_Request request, ServerCallContext context)
        {
            var response = new GetCustomerInfo_Response();
            response.ReturnCode = GrpcReturnCode.OK;
            response.MsgCode = "";
            //
            try
            {
                var findRecord = await DB.Find<mdClaimRequest>()
                                          .Match(a => a.CusPhone == request.StringValue)
                                          .ExecuteFirstAsync();
                //
                if (findRecord == null)
                {
                    response.ReturnCode = GrpcReturnCode.Error_201;
                    return await Task.FromResult(response);
                }
                else
                {
                    //OK
                    response.CusEmail = findRecord.CusEmail;
                    response.CusFullname = findRecord.CusFullname;
                    response.CusCardID = findRecord.CusCardID;
                    response.PickupAddress = findRecord.PickupAddress;
                }
            }
            catch (Exception ex)
            {
                response.ReturnCode = GrpcReturnCode.Error_ByServer;
                response.MsgCode = ex.Message;
                MyAppLog.WriteLog(MyConstant.LogLevel_Critical, "ClaimService", "GetCustomerInfo", "Exception", response.ReturnCode, ex.Message);
            }
            //
            return await Task.FromResult(response);
        }

        //-------------------------------------------------------------------------------------------------------/
        // GetInsureContract
        //-------------------------------------------------------------------------------------------------------/
        public override async Task<GetInsureContract_Response> GetInsureContract(Claim.Services.GetInsureContract_Request request, ServerCallContext context)
        {
            var response = new GetInsureContract_Response();
            response.ReturnCode = GrpcReturnCode.OK;
            response.MsgCode = "";
            //
            try
            {
                //Todo: Call ebao api for get contract info
                response.InsurStartDate = DateTime.UtcNow.ToTimestamp();
                response.InsurEndDate = DateTime.UtcNow.AddYears(1).ToTimestamp();
                response.InsurAmount = 999999999;
            }
            catch (Exception ex)
            {
                response.ReturnCode = GrpcReturnCode.Error_ByServer;
                response.MsgCode = ex.Message;
                MyAppLog.WriteLog(MyConstant.LogLevel_Critical, "ClaimService", "GetInsureContract", "Exception", response.ReturnCode, ex.Message);
            }
            //
            return await Task.FromResult(response);
        }

        //-------------------------------------------------------------------------------------------------------/
        // GetClaimRequestList
        //-------------------------------------------------------------------------------------------------------/
        public override async Task<GetClaimRequestList_Response> GetClaimRequestList(Claim.Services.GetClaimRequestList_Request request, ServerCallContext context)
        {
            var response = new GetClaimRequestList_Response();
            response.ReturnCode = GrpcReturnCode.OK;
            response.MsgCode = "";
            //
            try
            {
                var query = DB.Find<mdClaimRequest>();

                // CusPhone
                if (!string.IsNullOrWhiteSpace(request.CusPhone)) query.Match(a => a.CusPhone.Contains(request.CusPhone));
                //DeviceIMEI
                if (!string.IsNullOrWhiteSpace(request.DeviceIMEI)) query.Match(a => a.DeviceIMEI.Contains(request.DeviceIMEI));
                //CusFullname
                if (!string.IsNullOrWhiteSpace(request.CusFullname)) query.Match(a => a.CusFullname.RemoveVietnameseSign().Contains(request.CusFullname));
                //BrancheID
                if (!string.IsNullOrWhiteSpace(request.BrancheID)) query.Match(a => a.BrancheID == request.BrancheID);
                //InsurCompanyID
                if (!string.IsNullOrWhiteSpace(request.InsurCompanyID)) query.Match(a => a.InsurCompanyID == request.InsurCompanyID);
                //PickupCompanyID
                if (!string.IsNullOrWhiteSpace(request.PickupCompanyID)) query.Match(a => a.PickupCompanyID == request.PickupCompanyID);
                //RepairCompanyID
                if (!string.IsNullOrWhiteSpace(request.RepairCompanyID)) query.Match(a => a.RepairCompanyID == request.RepairCompanyID);
                //Status
                bool status = request.StatusCheck ? true : false;
                if (request.Status == 1) query.Match(a => a.ProcessStatus == status);
                if (request.Status == 2) query.Match(a => a.PickupStatus1 == status);
                if (request.Status == 3) query.Match(a => a.PickupStatus2 == status);
                if (request.Status == 4) query.Match(a => a.CheckStatus == status);
                if (request.Status == 5) query.Match(a => a.AcceptStatus == status);
                if (request.Status == 6) query.Match(a => a.EstimationStatus == status);
                if (request.Status == 7) query.Match(a => a.ApproveStatus == status);
                if (request.Status == 8) query.Match(a => a.RepairStatus == status);
                if (request.Status == 9) query.Match(a => a.ReturnStatus1 == status);
                if (request.Status == 10) query.Match(a => a.ReturnStatus2 == status);
                if (request.Status == 11) query.Match(a => a.CancelStatus == status);
                //Time range
                //StartDate
                if (request.StartDate.ToDateTime().ToString("yyyyMMdd") != DateTime.Today.MinShortDateString())
                {
                    query.Match(a => a.ClaimDate >= request.StartDate.ToDateTime());
                }
                //EndDate
                if (request.EndDate.ToDateTime().ToString("yyyyMMdd") != DateTime.Today.MaxShortDateString())
                {
                    query.Match(a => a.ClaimDate <= request.EndDate.ToDateTime());
                }
                var findRecords = await query.ExecuteAsync();
                //
                if (findRecords != null && findRecords.Count > 0)
                {
                    findRecords.ForEach(item =>
                    {
                        var grpcItem = new grpcClaimRequestModel();
                        ClassHelper.CopyPropertiesData(item, grpcItem);
                        //
                        response.ClaimRequests.Add(grpcItem);
                    });
                }
            }
            catch (Exception ex)
            {
                response.ReturnCode = GrpcReturnCode.Error_ByServer;
                response.MsgCode = ex.Message;
                MyAppLog.WriteLog(MyConstant.LogLevel_Critical, "ClaimService", "GetClaimRequest", "Exception", response.ReturnCode, ex.Message);
            }
            //
            return await Task.FromResult(response);
        }
        //-------------------------------------------------------------------------------------------------------/
        // GetPickupList
        //-------------------------------------------------------------------------------------------------------/
        public override async Task<GetClaimRequestList_Response> GetPickupList(Claim.Services.GetPickupList_Request request, ServerCallContext context)
        {
            var response = new GetClaimRequestList_Response();
            response.ReturnCode = GrpcReturnCode.OK;
            response.MsgCode = "";
            //
            try
            {
                var query = DB.Find<mdClaimRequest>();

                //PickupCompanyID
                if (!string.IsNullOrWhiteSpace(request.PickupCompanyID)) query.Match(a => a.PickupCompanyID == request.PickupCompanyID);

                //PickupReqStatus = true
                query.Match(a => a.PickupReqStatus == true);

                //Status
                bool status = request.StatusCheck ? true : false;
                if (request.Status == 2) query.Match(a => a.PickupStatus1 == status);
                if (request.Status == 3) query.Match(a => a.PickupStatus2 == status);
                //Time range
                //StartDate
                if (request.StartDate.ToDateTime().ToString("yyyyMMdd") != DateTime.Today.MinShortDateString())
                {
                    query.Match(a => a.ClaimDate >= request.StartDate.ToDateTime());
                }
                //EndDate
                if (request.EndDate.ToDateTime().ToString("yyyyMMdd") != DateTime.Today.MaxShortDateString())
                {
                    query.Match(a => a.ClaimDate <= request.EndDate.ToDateTime());
                }
                var findRecords = await query.ExecuteAsync();
                //
                if (findRecords != null && findRecords.Count > 0)
                {
                    findRecords.ForEach(item =>
                    {
                        var grpcItem = new grpcClaimRequestModel();
                        ClassHelper.CopyPropertiesData(item, grpcItem);
                        //
                        response.ClaimRequests.Add(grpcItem);
                    });
                }
            }
            catch (Exception ex)
            {
                response.ReturnCode = GrpcReturnCode.Error_ByServer;
                response.MsgCode = ex.Message;
                MyAppLog.WriteLog(MyConstant.LogLevel_Critical, "ClaimService", "GetPickupList", "Exception", response.ReturnCode, ex.Message);
            }
            //
            return await Task.FromResult(response);
        }

        //-------------------------------------------------------------------------------------------------------/
        // SaveRepairerMaster
        //-------------------------------------------------------------------------------------------------------/
        public override async Task<Claim.Services.String_Response> SaveRepairerMaster(SaveRepairerMaster_Request request, ServerCallContext context)
        {
            var response = new Claim.Services.String_Response();
            response.ReturnCode = GrpcReturnCode.OK;
            try
            {
                if (request.Record.UpdMode != 3)
                {
                    //Insert || Update
                    var saveRecord = new mdRepairerMaster();
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
                    await DB.DeleteAsync<mdRepairerMaster>(request.Record.ID);
                }
            }
            catch (Exception ex)
            {
                response.ReturnCode = GrpcReturnCode.Error_ByServer;
                response.MsgCode = ex.Message;
                MyAppLog.WriteLog(MyConstant.LogLevel_Critical, "ClaimService", "SaveRepairerMaster", "Exception", response.ReturnCode, ex.Message);
            }
            return await Task.FromResult(response);
        }

        //-------------------------------------------------------------------------------------------------------/
        // GetRepairerMaster
        //-------------------------------------------------------------------------------------------------------/
        public override async Task<GetRepairerMaster_Response> GetRepairerMaster(Claim.Services.Empty_Request request, ServerCallContext context)
        {
            var response = new GetRepairerMaster_Response();
            response.ReturnCode = GrpcReturnCode.OK;
            response.MsgCode = "";
            //
            try
            {
                var findRecords = await DB.Find<mdRepairerMaster>().ExecuteAsync();
                //
                if (findRecords != null)
                {
                    findRecords.ForEach(item =>
                    {
                        var grpcItem = new grpcRepairerMasterModel();
                        ClassHelper.CopyPropertiesData(item, grpcItem);
                        response.Records.Add(grpcItem);
                    });
                }
            }
            catch (Exception ex)
            {
                response.ReturnCode = GrpcReturnCode.Error_ByServer;
                response.MsgCode = ex.Message;
                MyAppLog.WriteLog(MyConstant.LogLevel_Critical, "ClaimService", "GetRepairerMaster", "Exception", response.ReturnCode, ex.Message);
            }
            //
            return await Task.FromResult(response);
        }
        //-------------------------------------------------------------------------------------------------------/
        // UpdateStatusClaimRequest
        //-------------------------------------------------------------------------------------------------------/
        public override async Task<Claim.Services.Empty_Response> CancelClaimRequest(Claim.Services.String_Request request, ServerCallContext context)
        {
            var response = new Claim.Services.Empty_Response();
            response.ReturnCode = GrpcReturnCode.OK;
            response.MsgCode = "";
            //
            try
            {
                var findRecord = await DB.Find<mdClaimRequest>()
                                         .Match(x => x.ClaimNo == request.StringValue)
                                         .ExecuteFirstAsync();
                //
                if (findRecord != null)
                {
                    findRecord.CancelStatus = true;
                    //Save
                    await findRecord.SaveAsync();
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
                MyAppLog.WriteLog(MyConstant.LogLevel_Critical, "ClaimService", "CancelClaimRequest", "Exception", response.ReturnCode, ex.Message);
            }
            //
            return await Task.FromResult(response);
        }

        //-------------------------------------------------------------------------------------------------------/
        // SaveBranchMaster
        //-------------------------------------------------------------------------------------------------------/
        public override async Task<Claim.Services.String_Response> SaveBranchMaster(SaveBranchMaster_Request request, ServerCallContext context)
        {
            var response = new Claim.Services.String_Response();
            response.ReturnCode = GrpcReturnCode.OK;
            try
            {
                if (request.Record.UpdMode != 3)
                {
                    //Insert || Update
                    var saveRecord = new mdBranchMaster();
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
                    await DB.DeleteAsync<mdBranchMaster>(request.Record.ID);
                }
            }
            catch (Exception ex)
            {
                response.ReturnCode = GrpcReturnCode.Error_ByServer;
                response.MsgCode = ex.Message;
                MyAppLog.WriteLog(MyConstant.LogLevel_Critical, "ClaimService", "SaveBranchMaster", "Exception", response.ReturnCode, ex.Message);
            }
            return await Task.FromResult(response);
        }

        //-------------------------------------------------------------------------------------------------------/
        // GetBranchMaster
        //-------------------------------------------------------------------------------------------------------/
        public override async Task<GetBranchMaster_Response> GetBranchMaster(Claim.Services.Empty_Request request, ServerCallContext context)
        {
            var response = new GetBranchMaster_Response();
            response.ReturnCode = GrpcReturnCode.OK;
            response.MsgCode = "";
            //
            try
            {
                var findRecords = await DB.Find<mdBranchMaster>().ExecuteAsync();
                //
                if (findRecords != null)
                {
                    findRecords.ForEach(item =>
                    {
                        var grpcItem = new grpcBranchMasterModel();
                        ClassHelper.CopyPropertiesData(item, grpcItem);
                        response.Records.Add(grpcItem);
                    });
                }
            }
            catch (Exception ex)
            {
                response.ReturnCode = GrpcReturnCode.Error_ByServer;
                response.MsgCode = ex.Message;
                MyAppLog.WriteLog(MyConstant.LogLevel_Critical, "ClaimService", "GetBranchMaster", "Exception", response.ReturnCode, ex.Message);
            }
            //
            return await Task.FromResult(response);
        }
        


    }//End class
}//End name space
