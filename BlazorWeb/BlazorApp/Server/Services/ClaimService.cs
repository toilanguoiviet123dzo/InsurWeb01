﻿using System;
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
using Google.Protobuf.WellKnownTypes;
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
                        newRecord.ID = "";
                        newRecord.ModifiedOn = DateTime.UtcNow;

                        //Commit VoucherNo
                        var commitedVovoucher = await MyVoucher.CommitVoucherNo("001", newRecord.ClaimNo);
                        if (!string.IsNullOrWhiteSpace(commitedVovoucher))
                        {
                            //Update Estimation + Attach file for new ClaimNo
                            if (newRecord.ClaimNo != commitedVovoucher)
                            {
                                //mdAttachFile
                                await DB.Update<mdAttachFile>()
                                        .Match(a => a.VoucherNo == newRecord.ClaimNo)
                                        .Modify(x => x.VoucherNo, newRecord.ClaimNo)
                                        .ExecuteAsync();
                            }
                            newRecord.ClaimNo = commitedVovoucher;
                        }
                        //Update Estimation for approval
                        if (request.IsApproval)
                        {
                            
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

                            //Update Estimation for approval
                            if (request.IsApproval)
                            {
                                
                            }
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
                        await DB.DeleteAsync<mdClaimRequest>(request.ClaimRequest.ID);

                        //mdRepairEstimation
                        await DB.DeleteAsync<mdRepairEstimation>(x => x.ClaimNo == request.ClaimRequest.ClaimNo);

                        //Delete resource
                        var findRecords = await DB.Find<mdAttachFile>()
                                                  .Match(a => a.VoucherNo == request.ClaimRequest.ClaimNo)
                                                  .ExecuteAsync();
                        if (findRecords != null)
                        {
                            foreach (var record in findRecords)
                            {
                                var resourceFile = new grpcResourceFileModel();
                                resourceFile.ResourceID = record.ResourceID;
                                resourceFile.UpdMode = 3;   //Delete
                                //
                                var resourceID = await ResourceService.SaveResourceFile(resourceFile);
                            }
                        }
                        //Delete mdAttachFile
                        await DB.DeleteAsync<mdAttachFile>(x => x.VoucherNo == request.ClaimRequest.ClaimNo);
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
        // SaveRepairEstimation
        //-------------------------------------------------------------------------------------------------------/
        public override async Task<Claim.Services.Empty_Response> SaveRepairEstimation(SaveRepairEstimation_Request request, ServerCallContext context)
        {
            var response = new Claim.Services.Empty_Response();
            response.ReturnCode = GrpcReturnCode.OK;
            try
            {
                //Delete first
                await DB.DeleteAsync<mdRepairEstimation>(x => x.ClaimNo == request.RepairEstimation.ClaimNo);
                if (request.RepairEstimation.UpdMode == 3)
                {
                    return await Task.FromResult(response);
                }

                //Insert
                var saveRecord = new mdRepairEstimation();
                //Header
                ClassHelper.CopyPropertiesData(request.RepairEstimation, saveRecord);
                //
                saveRecord.ID = "";
                saveRecord.ModifiedOn = DateTime.UtcNow;

                //Group items
                saveRecord.EstGroupItems = new List<EstGroupItemModel>();
                if (request.RepairEstimation.EstGroupItems != null && request.RepairEstimation.EstGroupItems.Count > 0)
                {
                    foreach (var item in request.RepairEstimation.EstGroupItems)
                    {
                        var groupItem = new EstGroupItemModel();
                        ClassHelper.CopyPropertiesData(item, groupItem);
                        //
                        //Detail items
                        groupItem.EstDetailItems = new List<EstDetailItemModel>();
                        if (item.EstDetailItems != null && item.EstDetailItems.Count > 0)
                        {
                            foreach (var subItem in item.EstDetailItems)
                            {
                                var detailItem = new EstDetailItemModel();
                                ClassHelper.CopyPropertiesData(subItem, detailItem);
                                //
                                groupItem.EstDetailItems.Add(detailItem);
                            }
                        }
                        //Add group item
                        saveRecord.EstGroupItems.Add(groupItem);
                    }
                }
                //
                await saveRecord.SaveAsync();
            }
            catch (Exception ex)
            {
                response.ReturnCode = GrpcReturnCode.Error_ByServer;
                response.MsgCode = ex.Message;
                MyAppLog.WriteLog(MyConstant.LogLevel_Critical, "ClaimService", "SaveRepairEstimation", "Exception", response.ReturnCode, ex.Message);
            }
            return await Task.FromResult(response);
        }

        //-------------------------------------------------------------------------------------------------------/
        // GetRepairEstimation
        //-------------------------------------------------------------------------------------------------------/
        public override async Task<GetRepairEstimation_Response> GetRepairEstimation(Claim.Services.String_Request request, ServerCallContext context)
        {
            var response = new GetRepairEstimation_Response();
            response.ReturnCode = GrpcReturnCode.OK;
            response.MsgCode = "";
            //
            try
            {
                var query = DB.Find<mdRepairEstimation>();
                query.Match(a => a.ClaimNo == request.StringValue);
                var findRecord = await query.ExecuteFirstAsync();
                //
                if (findRecord != null)
                {
                    response.RepairEstimation = new grpcRepairEstimationModel();
                    //Header
                    ClassHelper.CopyPropertiesData(findRecord, response.RepairEstimation);

                    //Group items
                    if (findRecord.EstGroupItems != null && findRecord.EstGroupItems.Count > 0)
                    {
                        foreach (var item in findRecord.EstGroupItems)
                        {
                            var groupItem = new grpcEstGroupItemModel();
                            ClassHelper.CopyPropertiesData(item, groupItem);
                            //
                            //Detail items
                            if (item.EstDetailItems != null && item.EstDetailItems.Count > 0)
                            {
                                foreach (var subItem in item.EstDetailItems)
                                {
                                    var detailItem = new grpcEstDetailItemModel();
                                    ClassHelper.CopyPropertiesData(subItem, detailItem);
                                    //
                                    groupItem.EstDetailItems.Add(detailItem);
                                }
                            }
                            //Add group item
                            response.RepairEstimation.EstGroupItems.Add(groupItem);
                        }
                    }
                }
                else
                {
                    response.ReturnCode = GrpcReturnCode.Error_201;
                    return await Task.FromResult(response);
                }
            }
            catch (Exception ex)
            {
                response.ReturnCode = GrpcReturnCode.Error_ByServer;
                response.MsgCode = ex.Message;
                MyAppLog.WriteLog(MyConstant.LogLevel_Critical, "ClaimService", "GetRepairEstimation", "Exception", response.ReturnCode, ex.Message);
            }
            //
            return await Task.FromResult(response);
        }

        //-------------------------------------------------------------------------------------------------------/
        // SaveAttachFiles
        //-------------------------------------------------------------------------------------------------------/
        public override async Task<Claim.Services.Empty_Response> SaveAttachFiles(SaveAttachFiles_Request request, ServerCallContext context)
        {
            var response = new Claim.Services.Empty_Response();
            response.ReturnCode = GrpcReturnCode.OK;
            try
            {
                foreach (var item in request.AttachFiles)
                {
                    switch (item.UpdMode)
                    {
                        //add new
                        case 1:
                            var newRecord = new mdAttachFile();
                            ClassHelper.CopyPropertiesData(item, newRecord);
                            newRecord.ID = "";
                            newRecord.ModifiedOn = DateTime.UtcNow;
                            //
                            await newRecord.SaveAsync();
                            break;

                        //update
                        case 2:
                            var oldRecord = await DB.Find<mdAttachFile>().OneAsync(item.ID);
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
                            await DB.DeleteAsync<mdAttachFile>(item.ID);
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
                MyAppLog.WriteLog(MyConstant.LogLevel_Critical, "ClaimService", "SaveAttachFiles", "Exception", response.ReturnCode, ex.Message);
            }
            return await Task.FromResult(response);
        }

        //-------------------------------------------------------------------------------------------------------/
        // GetAttachFiles
        //-------------------------------------------------------------------------------------------------------/
        public override async Task<GetAttachFiles_Response> GetAttachFiles(Claim.Services.String_Request request, ServerCallContext context)
        {
            var response = new GetAttachFiles_Response();
            response.ReturnCode = GrpcReturnCode.OK;
            response.MsgCode = "";
            //
            try
            {
                var query = DB.Find<mdAttachFile>();
                if (!string.IsNullOrWhiteSpace(request.StringValue))
                {
                    query.Match(a => a.VoucherNo == request.StringValue);
                }
                var findRecords = await query.ExecuteAsync();
                //
                if (findRecords != null && findRecords.Count > 0)
                {
                    findRecords.ForEach(item =>
                    {
                        var grpcItem = new grpcAttachFileModel();
                        ClassHelper.CopyPropertiesData(item, grpcItem);
                        response.AttachFiles.Add(grpcItem);
                    });
                }
            }
            catch (Exception ex)
            {
                response.ReturnCode = GrpcReturnCode.Error_ByServer;
                response.MsgCode = ex.Message;
                MyAppLog.WriteLog(MyConstant.LogLevel_Critical, "ClaimService", "GetAttachFiles", "Exception", response.ReturnCode, ex.Message);
            }
            //
            return await Task.FromResult(response);
        }
        //-------------------------------------------------------------------------------------------------------/
        // GetAttachFileCount
        //-------------------------------------------------------------------------------------------------------/
        public override async Task<Claim.Services.Int_Response> GetAttachFileCount(Claim.Services.GetAttachFileCount_Request request, ServerCallContext context)
        {
            var response = new Claim.Services.Int_Response();
            response.ReturnCode = GrpcReturnCode.OK;
            response.MsgCode = "";
            response.IntValue = 0;
            //
            try
            {
                var query = DB.Find<mdAttachFile>();
                query.Match(a => a.VoucherNo == request.VoucherNo && a.DocumentLevel <= request.DocumentLevel);
                var findRecords = await query.ExecuteAsync();
                //
                if (findRecords != null)
                {
                    response.IntValue = findRecords.Count;
                }
            }
            catch (Exception ex)
            {
                response.ReturnCode = GrpcReturnCode.Error_ByServer;
                response.MsgCode = ex.Message;
                MyAppLog.WriteLog(MyConstant.LogLevel_Critical, "ClaimService", "GetAttachFileCount", "Exception", response.ReturnCode, ex.Message);
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

                //CustomerID
                if (!string.IsNullOrWhiteSpace(request.CustomerID)) query.Match(a => a.CustomerID == request.CustomerID);
                //ReqPersonID
                if (!string.IsNullOrWhiteSpace(request.ReqPersonID)) query.Match(a => a.ReqPersonID == request.ReqPersonID);
                //EstPersonID
                if (!string.IsNullOrWhiteSpace(request.EstPersonID)) query.Match(a => a.EstPersonID == request.EstPersonID);
                //AprPersonID
                if (!string.IsNullOrWhiteSpace(request.AprPersonID)) query.Match(a => a.AprPersonID == request.AprPersonID);
                //PayPersonID
                if (!string.IsNullOrWhiteSpace(request.PayPersonID)) query.Match(a => a.PayPersonID == request.PayPersonID);
                //PhoneNo
                if (!string.IsNullOrWhiteSpace(request.PhoneNo)) query.Match(a => a.PhoneNo.Contains(request.PhoneNo));
                //CarOwner
                if (!string.IsNullOrWhiteSpace(request.CarOwner)) query.Match(a => a.CarOwner.Contains(request.CarOwner));
                //ClaimNo
                if (!string.IsNullOrWhiteSpace(request.ClaimNo)) query.Match(a => a.ClaimNo.Contains(request.ClaimNo));
                //LicensePlate
                if (!string.IsNullOrWhiteSpace(request.LicensePlate)) query.Match(a => a.LicensePlate.Contains(request.LicensePlate));

                //Not Accept
                if (request.Status == 1) query.Match(a => !a.AcceptStatus);
                //Not process
                if (request.Status == 2) query.Match(a => a.AcceptStatus && !a.CompenStatus);
                //Not req est
                if (request.Status == 3) query.Match(a => a.CompenStatus && !a.EstReqStatus);
                //Not done est
                if (request.Status == 4) query.Match(a => a.EstReqStatus && !a.EstDoneStatus);
                //Not Accept est
                if (request.Status == 5) query.Match(a => a.EstDoneStatus && !a.EstAprStatus);
                //Not approve
                if (request.Status == 6) query.Match(a => a.EstAprStatus && !a.AprStatus);
                //Not repair
                if (request.Status == 7) query.Match(a => a.AprStatus && !a.RepairStatus);
                //Not repair Accept
                if (request.Status == 8) query.Match(a => a.RepairStatus && !a.AprRepairStatus);
                //Not payment
                if (request.Status == 9) query.Match(a => a.AprRepairStatus && !a.PayStatus);

                //StartDate
                if (request.StartDate.ToDateTime().ToString("yyyyMMdd") != DateTime.Today.MinShortDateString())
                {
                    query.Match(a => a.CompenDateTime >= request.StartDate.ToDateTime());
                }
                //EndDate
                if (request.EndDate.ToDateTime().ToString("yyyyMMdd") != DateTime.Today.MaxShortDateString())
                {
                    query.Match(a => a.CompenDateTime <= request.EndDate.ToDateTime());
                    //
                }
                var findRecords = await query.ExecuteAsync();
                //
                if (findRecords != null && findRecords.Count > 0)
                {
                    findRecords.ForEach(item =>
                    {
                        var grpcItem = new grpcClaimRequestModel();
                        ClassHelper.CopyPropertiesData(item, grpcItem);
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
        // UpdateTotalClaimRequest
        //-------------------------------------------------------------------------------------------------------/
        public override async Task<Claim.Services.Empty_Response> UpdateTotalClaimRequest(UpdateTotalClaimRequest_Request request, ServerCallContext context)
        {
            var response = new Claim.Services.Empty_Response();
            response.ReturnCode = GrpcReturnCode.OK;
            response.MsgCode = "";
            //
            try
            {
                var findRecord = await DB.Find<mdClaimRequest>()
                                         .Match(x => x.ClaimNo == request.ClaimNo)
                                         .ExecuteFirstAsync();
                //
                if (findRecord != null)
                {
                    findRecord.EstRepairPrice = request.EstRepairPrice;
                    findRecord.DealRepairPrice = request.DealRepairPrice;
                    findRecord.AprRepairPrice = request.AprRepairPrice;
                    findRecord.EstVAT = request.EstVAT;
                    findRecord.DealVAT = request.DealVAT;
                    findRecord.AprVAT = request.AprVAT;
                    //Derived
                    findRecord.DiscountAmount = Math.Round((findRecord.AprRepairPrice + findRecord.AprVAT) * findRecord.DiscountRate, 0);
                    findRecord.ClaimPrice = findRecord.AprRepairPrice + findRecord.AprVAT - findRecord.DiscountAmount - findRecord.TipAmount;

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
                MyAppLog.WriteLog(MyConstant.LogLevel_Critical, "ClaimService", "UpdateTotalClaimRequest", "Exception", response.ReturnCode, ex.Message);
            }
            //
            return await Task.FromResult(response);
        }
        //-------------------------------------------------------------------------------------------------------/
        // UpdateStatusClaimRequest
        //-------------------------------------------------------------------------------------------------------/
        public override async Task<Claim.Services.Empty_Response> UpdateStatusClaimRequest(Claim.Services.UpdateStatusClaimRequest_Request request, ServerCallContext context)
        {
            var response = new Claim.Services.Empty_Response();
            response.ReturnCode = GrpcReturnCode.OK;
            response.MsgCode = "";
            //
            try
            {
                var findRecord = await DB.Find<mdClaimRequest>()
                                         .Match(x => x.ClaimNo == request.ClaimNo)
                                         .ExecuteFirstAsync();
                //
                if (findRecord != null)
                {
                    if (request.IsUpdateAcceptStatus) findRecord.AcceptStatus = request.AcceptStatus;
                    if (request.IsUpdateCancelStatus) findRecord.CancelStatus = request.CancelStatus;
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
                MyAppLog.WriteLog(MyConstant.LogLevel_Critical, "ClaimService", "UpdateTotalClaimRequest", "Exception", response.ReturnCode, ex.Message);
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
        //-------------------------------------------------------------------------------------------------------/
        // GetRefEstimationList
        //-------------------------------------------------------------------------------------------------------/
        public override async Task<GetRefEstimationList_Response> GetRefEstimationList(Claim.Services.Empty_Request request, ServerCallContext context)
        {
            var response = new GetRefEstimationList_Response();
            response.ReturnCode = GrpcReturnCode.OK;
            response.MsgCode = "";
            //
            try
            {
                var query = DB.Find<mdRepairEstimation>();
                query.Match(a => a.IsTemplate);
                //
                var findRecords = await query.ExecuteAsync();
                //
                if (findRecords != null && findRecords.Count > 0)
                {
                    findRecords.ForEach(item =>
                    {
                        var grpcItem = new grpcRefEstimationModel();
                        ClassHelper.CopyPropertiesData(item, grpcItem);
                        response.Records.Add(grpcItem);
                    });
                }
            }
            catch (Exception ex)
            {
                response.ReturnCode = GrpcReturnCode.Error_ByServer;
                response.MsgCode = ex.Message;
                MyAppLog.WriteLog(MyConstant.LogLevel_Critical, "ClaimService", "GetRefEstimationList", "Exception", response.ReturnCode, ex.Message);
            }
            //
            return await Task.FromResult(response);
        }


    }//End class
}//End name space