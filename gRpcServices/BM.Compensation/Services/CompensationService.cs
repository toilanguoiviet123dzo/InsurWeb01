using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Gosu.Service.Models;
using Gosu.Compensation.Services;
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
using Gosu.Admin.Services;
using Gosu.Resource.Services;


namespace Gosu.Service.Services
{
    public class CompensationService : grpcCompensationService.grpcCompensationServiceBase
    {
        private readonly ILogger<CompensationService> _logger;

        public CompensationService(ILogger<CompensationService> logger)
        {
            _logger = logger;
        }
        //-------------------------------------------------------------------------------------------------------/
        // SaveCompenRequest
        //-------------------------------------------------------------------------------------------------------/
        public override async Task<Compensation.Services.String_Response> SaveCompenRequest(SaveCompenRequest_Request request, ServerCallContext context)
        {
            var response = new Compensation.Services.String_Response();
            response.ReturnCode = GrpcReturnCode.OK;
            try
            {
                switch (request.CompenRequest.UpdMode)
                {
                    //add new
                    case 1:
                        var newRecord = new mdCompenRequest();
                        ClassHelper.CopyPropertiesData(request.CompenRequest, newRecord);
                        newRecord.ID = "";
                        newRecord.ModifiedOn = DateTime.UtcNow;
                        //Commit VoucherNo
                        var requestCommit = new CommitVoucherNo_Request();
                        requestCommit.Credential = new Gosu.Admin.Services.UserCredential()
                        {
                            Username = GrpcCredential.Username,
                            RoleID = GrpcCredential.RoleID,
                            AccessToken = GrpcCredential.AccessToken,
                            ApiKey = GrpcCredential.ApiKey
                        };
                        requestCommit.VoucherCode = "001";
                        requestCommit.VoucherNo = newRecord.CompenNo;
                        //
                        var responseCommit = await GrpcClientFactory.CallServiceAsync<Gosu.Admin.Services.String_Response>(ServiceList.Admin, async channel =>
                        {
                            var client = new grpcAdminService.grpcAdminServiceClient(channel);
                            return await client.CommitVoucherNoAsync(requestCommit);
                        });
                        if (responseCommit != null && responseCommit.ReturnCode == GrpcReturnCode.OK)
                        {

                            //Update Estimation + Attach file for new CompenNo
                            if (newRecord.CompenNo != responseCommit.StringValue)
                            {
                                //mdAttachFile
                                await DB.Update<mdAttachFile>()
                                        .Match(a => a.VoucherNo == newRecord.CompenNo)
                                        .Modify(x => x.VoucherNo, newRecord.CompenNo)
                                        .ExecuteAsync();

                                //mdAttachFile
                                await DB.Update<mdRepairEstimation>()
                                        .Match(a => a.CompenNo == newRecord.CompenNo)
                                        .Modify(x => x.CompenNo, newRecord.CompenNo)
                                        .ExecuteAsync();
                            }
                            newRecord.CompenNo = responseCommit.StringValue;
                        }
                        //Update Estimation for approval
                        if (request.IsApproval)
                        {
                            var updateResult = await Update_EstimationForApprove(newRecord.CompenNo);
                            if (updateResult.HasChanged)
                            {
                                Update_CompenTotal(newRecord, updateResult);
                            }
                        }
                        //
                        await newRecord.SaveAsync();
                        break;

                    //update
                    case 2:
                        var oldRecord = await DB.Find<mdCompenRequest>().OneAsync(request.CompenRequest.ID);
                        if (oldRecord != null)
                        {
                            ClassHelper.CopyPropertiesData(request.CompenRequest, oldRecord);
                            oldRecord.ModifiedOn = DateTime.UtcNow;

                            //Update Estimation for approval
                            if (request.IsApproval)
                            {
                                var updateResult = await Update_EstimationForApprove(oldRecord.CompenNo);
                                if (updateResult.HasChanged)
                                {
                                    Update_CompenTotal(oldRecord, updateResult);
                                }
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
                        //mdCompenRequest
                        await DB.DeleteAsync<mdCompenRequest>(request.CompenRequest.ID);

                        //mdRepairEstimation
                        await DB.DeleteAsync<mdRepairEstimation>(x => x.CompenNo == request.CompenRequest.CompenNo);

                        //Delete resource
                        var findRecords = await DB.Find<mdAttachFile>()
                                                  .Match(a => a.VoucherNo == request.CompenRequest.CompenNo)
                                                  .ExecuteAsync();
                        if (findRecords != null)
                        {
                            foreach (var record in findRecords)
                            {
                                var requestResource = new SaveResourceFile_Request();
                                var Credential = new Gosu.Resource.Services.UserCredential()
                                {
                                    Username = GrpcCredential.Username,
                                    RoleID = GrpcCredential.RoleID,
                                    AccessToken = GrpcCredential.AccessToken,
                                    ApiKey = GrpcCredential.ApiKey
                                };
                                requestResource.Credential = Credential;
                                requestResource.ResourceFile = new grpcResourceFileModel();
                                requestResource.ResourceFile.ResourceID = record.ResourceID;
                                requestResource.ResourceFile.UpdMode = 3;   //Delete
                                //
                                var responseResource = await GrpcClientFactory.CallServiceAsync<Gosu.Resource.Services.String_Response>(ServiceList.Resource, async channel =>
                                {
                                    var client = new grpcResourceService.grpcResourceServiceClient(channel);
                                    return await client.SaveResourceFileAsync(requestResource);
                                });
                            }
                        }
                        //Delete mdAttachFile
                        await DB.DeleteAsync<mdAttachFile>(x => x.VoucherNo == request.CompenRequest.CompenNo);
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
                MyAppLog.WriteLog(MyConstant.LogLevel_Critical, "CompensationService", "SaveCompenRequest", "1", response.ReturnCode, ex.Message);
            }
            return await Task.FromResult(response);
        }

        private void Update_CompenTotal(mdCompenRequest record, ApproveEstimation_ReturnModel updateResult)
        {
            try
            {
                record.EstRepairPrice = updateResult.RepairPrice;
                record.DealRepairPrice = updateResult.DealRepairPrice;
                record.AprRepairPrice = updateResult.AprRepairPrice;
                record.EstVAT = updateResult.EstVAT;
                record.DealVAT = updateResult.DealVAT;
                record.AprVAT = updateResult.AprVAT;

                //Cal: DiscountAmount, CompensationPrice
                record.DiscountAmount = Math.Round((record.AprRepairPrice + record.AprVAT) * record.DiscountRate, 0);
                record.CompensationPrice = record.AprRepairPrice + record.AprVAT - record.DiscountAmount - record.TipAmount;
            }
            catch { }
        }

        private async Task<ApproveEstimation_ReturnModel> Update_EstimationForApprove(string compenNo)
        {
            var result = new ApproveEstimation_ReturnModel();
            var hasChanged = false;
            result.CompenNo = compenNo;
            //
            try
            {
                var updateRecord = await DB.Find<mdRepairEstimation>()
                                              .Match(a => a.CompenNo == compenNo)
                                              .ExecuteFirstAsync();
                //
                if (updateRecord != null)
                {
                    //Group
                    if (updateRecord.EstGroupItems != null && updateRecord.EstGroupItems.Count > 0)
                    {
                        foreach (var groupRow in updateRecord.EstGroupItems)
                        {
                            var groupTotal = new ApproveEstimation_ReturnModel();
                            if (groupRow.EstDetailItems != null && groupRow.EstDetailItems.Count > 0)
                            {
                                foreach (var row in groupRow.EstDetailItems)
                                {
                                    //Update child
                                    //Deal
                                    if (row.DealAmount == 0)
                                    {
                                        row.DealAmount = row.Amount;
                                        row.DealVAT = row.EstVAT;
                                        hasChanged = true;
                                    }

                                    //Approve
                                    if (row.AprAmount == 0)
                                    {
                                        row.AprAmount = row.DealAmount > 0 ? row.DealAmount : row.Amount;
                                        row.AprVAT = row.DealVAT > 0 ? row.DealVAT : row.EstVAT;
                                        hasChanged = true;
                                    }
                                    //Sum in group
                                    groupTotal.RepairPrice += row.Amount;
                                    groupTotal.DealRepairPrice += row.DealAmount;
                                    groupTotal.AprRepairPrice += row.AprAmount;
                                    groupTotal.EstVAT += row.EstVAT;
                                    groupTotal.DealVAT += row.DealVAT;
                                    groupTotal.AprVAT += row.AprVAT;
                                }
                            }
                            //Update group
                            groupRow.RepairPrice = groupTotal.RepairPrice;
                            groupRow.DealRepairPrice = groupTotal.DealRepairPrice;
                            groupRow.AprRepairPrice = groupTotal.AprRepairPrice;
                            groupRow.EstVAT = groupTotal.EstVAT;
                            groupRow.DealVAT = groupTotal.DealVAT;
                            groupRow.AprVAT = groupTotal.AprVAT;

                            //Update for total result
                            result.RepairPrice += groupTotal.RepairPrice;
                            result.DealRepairPrice += groupTotal.DealRepairPrice;
                            result.AprRepairPrice += groupTotal.AprRepairPrice;
                            result.EstVAT += groupTotal.EstVAT;
                            result.DealVAT += groupTotal.DealVAT;
                            result.AprVAT += groupTotal.AprVAT;
                        }
                    }
                    //Save
                    if (hasChanged)
                    {
                        await updateRecord.SaveAsync();
                    }
                }
            }
            catch { }
            //Return
            result.HasChanged = hasChanged;
            return result;
        }

        //-------------------------------------------------------------------------------------------------------/
        // GetCompenRequest
        //-------------------------------------------------------------------------------------------------------/
        public override async Task<GetCompenRequest_Response> GetCompenRequest(Compensation.Services.String_Request request, ServerCallContext context)
        {
            var response = new GetCompenRequest_Response();
            response.ReturnCode = GrpcReturnCode.OK;
            response.MsgCode = "";
            //
            try
            {
                var findRecord = await DB.Find<mdCompenRequest>()
                                          .Match(a => a.CompenNo == request.StringValue)
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
                    response.CompenRequest = new grpcCompenRequestModel();
                    ClassHelper.CopyPropertiesData(findRecord, response.CompenRequest);
                }
            }
            catch (Exception ex)
            {
                response.ReturnCode = GrpcReturnCode.Error_ByServer;
                response.MsgCode = ex.Message;
                MyAppLog.WriteLog(MyConstant.LogLevel_Critical, "CompensationService", "GetCompenRequest", "1", response.ReturnCode, ex.Message);
            }
            //
            return await Task.FromResult(response);
        }

        //-------------------------------------------------------------------------------------------------------/
        // SaveRepairEstimation
        //-------------------------------------------------------------------------------------------------------/
        public override async Task<Compensation.Services.Empty_Response> SaveRepairEstimation(SaveRepairEstimation_Request request, ServerCallContext context)
        {
            var response = new Compensation.Services.Empty_Response();
            response.ReturnCode = GrpcReturnCode.OK;
            try
            {
                //Delete first
                await DB.DeleteAsync<mdRepairEstimation>(x => x.CompenNo == request.RepairEstimation.CompenNo);
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
                MyAppLog.WriteLog(MyConstant.LogLevel_Critical, "CompensationService", "SaveRepairEstimation", "1", response.ReturnCode, ex.Message);
            }
            return await Task.FromResult(response);
        }

        //-------------------------------------------------------------------------------------------------------/
        // GetRepairEstimation
        //-------------------------------------------------------------------------------------------------------/
        public override async Task<GetRepairEstimation_Response> GetRepairEstimation(Compensation.Services.String_Request request, ServerCallContext context)
        {
            var response = new GetRepairEstimation_Response();
            response.ReturnCode = GrpcReturnCode.OK;
            response.MsgCode = "";
            //
            try
            {
                var query = DB.Find<mdRepairEstimation>();
                query.Match(a => a.CompenNo == request.StringValue);
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
                MyAppLog.WriteLog(MyConstant.LogLevel_Critical, "CompensationService", "GetRepairEstimation", "1", response.ReturnCode, ex.Message);
            }
            //
            return await Task.FromResult(response);
        }

        //-------------------------------------------------------------------------------------------------------/
        // SaveAttachFiles
        //-------------------------------------------------------------------------------------------------------/
        public override async Task<Compensation.Services.Empty_Response> SaveAttachFiles(SaveAttachFiles_Request request, ServerCallContext context)
        {
            var response = new Compensation.Services.Empty_Response();
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
                MyAppLog.WriteLog(MyConstant.LogLevel_Critical, "CompensationService", "SaveAttachFiles", "1", response.ReturnCode, ex.Message);
            }
            return await Task.FromResult(response);
        }

        //-------------------------------------------------------------------------------------------------------/
        // GetAttachFiles
        //-------------------------------------------------------------------------------------------------------/
        public override async Task<GetAttachFiles_Response> GetAttachFiles(Compensation.Services.String_Request request, ServerCallContext context)
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
                MyAppLog.WriteLog(MyConstant.LogLevel_Critical, "CompensationService", "GetAttachFiles", "1", response.ReturnCode, ex.Message);
            }
            //
            return await Task.FromResult(response);
        }
        //-------------------------------------------------------------------------------------------------------/
        // GetAttachFileCount
        //-------------------------------------------------------------------------------------------------------/
        public override async Task<Int_Response> GetAttachFileCount(Compensation.Services.GetAttachFileCount_Request request, ServerCallContext context)
        {
            var response = new Int_Response();
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
                MyAppLog.WriteLog(MyConstant.LogLevel_Critical, "CompensationService", "GetAttachFileCount", "1", response.ReturnCode, ex.Message);
            }
            //
            return await Task.FromResult(response);
        }

        //-------------------------------------------------------------------------------------------------------/
        // GetCompenRequestList
        //-------------------------------------------------------------------------------------------------------/
        public override async Task<GetCompenRequestList_Response> GetCompenRequestList(Compensation.Services.GetCompenRequestList_Request request, ServerCallContext context)
        {
            var response = new GetCompenRequestList_Response();
            response.ReturnCode = GrpcReturnCode.OK;
            response.MsgCode = "";
            //
            try
            {
                var query = DB.Find<mdCompenRequest>();

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
                //CompenNo
                if (!string.IsNullOrWhiteSpace(request.CompenNo)) query.Match(a => a.CompenNo.Contains(request.CompenNo));
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
                        var grpcItem = new grpcCompenRequestModel();
                        ClassHelper.CopyPropertiesData(item, grpcItem);
                        response.CompenRequests.Add(grpcItem);
                    });
                }
            }
            catch (Exception ex)
            {
                response.ReturnCode = GrpcReturnCode.Error_ByServer;
                response.MsgCode = ex.Message;
                MyAppLog.WriteLog(MyConstant.LogLevel_Critical, "CompensationService", "GetCompenRequest", "1", response.ReturnCode, ex.Message);
            }
            //
            return await Task.FromResult(response);
        }
        //-------------------------------------------------------------------------------------------------------/
        // SaveRepairerMaster
        //-------------------------------------------------------------------------------------------------------/
        public override async Task<Compensation.Services.String_Response> SaveRepairerMaster(SaveRepairerMaster_Request request, ServerCallContext context)
        {
            var response = new Compensation.Services.String_Response();
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
                MyAppLog.WriteLog(MyConstant.LogLevel_Critical, "CompensationService", "SaveRepairerMaster", "1", response.ReturnCode, ex.Message);
            }
            return await Task.FromResult(response);
        }

        //-------------------------------------------------------------------------------------------------------/
        // GetRepairerMaster
        //-------------------------------------------------------------------------------------------------------/
        public override async Task<GetRepairerMaster_Response> GetRepairerMaster(Compensation.Services.Empty_Request request, ServerCallContext context)
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
                MyAppLog.WriteLog(MyConstant.LogLevel_Critical, "CompensationService", "GetRepairerMaster", "1", response.ReturnCode, ex.Message);
            }
            //
            return await Task.FromResult(response);
        }
        //-------------------------------------------------------------------------------------------------------/
        // UpdateTotalCompenRequest
        //-------------------------------------------------------------------------------------------------------/
        public override async Task<Gosu.Compensation.Services.Empty_Response> UpdateTotalCompenRequest(UpdateTotalCompenRequest_Request request, ServerCallContext context)
        {
            var response = new Gosu.Compensation.Services.Empty_Response();
            response.ReturnCode = GrpcReturnCode.OK;
            response.MsgCode = "";
            //
            try
            {
                var findRecord = await DB.Find<mdCompenRequest>()
                                         .Match(x => x.CompenNo == request.CompenNo)
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
                    findRecord.CompensationPrice = findRecord.AprRepairPrice + findRecord.AprVAT - findRecord.DiscountAmount - findRecord.TipAmount;

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
                MyAppLog.WriteLog(MyConstant.LogLevel_Critical, "CompensationService", "UpdateTotalCompenRequest", "1", response.ReturnCode, ex.Message);
            }
            //
            return await Task.FromResult(response);
        }
        //-------------------------------------------------------------------------------------------------------/
        // UpdateStatusCompenRequest
        //-------------------------------------------------------------------------------------------------------/
        public override async Task<Gosu.Compensation.Services.Empty_Response> UpdateStatusCompenRequest(Gosu.Compensation.Services.UpdateStatusCompenRequest_Request request, ServerCallContext context)
        {
            var response = new Gosu.Compensation.Services.Empty_Response();
            response.ReturnCode = GrpcReturnCode.OK;
            response.MsgCode = "";
            //
            try
            {
                var findRecord = await DB.Find<mdCompenRequest>()
                                         .Match(x => x.CompenNo == request.CompenNo)
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
                MyAppLog.WriteLog(MyConstant.LogLevel_Critical, "CompensationService", "UpdateTotalCompenRequest", "1", response.ReturnCode, ex.Message);
            }
            //
            return await Task.FromResult(response);
        }

        //-------------------------------------------------------------------------------------------------------/
        // SaveBranchMaster
        //-------------------------------------------------------------------------------------------------------/
        public override async Task<Compensation.Services.String_Response> SaveBranchMaster(SaveBranchMaster_Request request, ServerCallContext context)
        {
            var response = new Compensation.Services.String_Response();
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
                MyAppLog.WriteLog(MyConstant.LogLevel_Critical, "CompensationService", "SaveBranchMaster", "1", response.ReturnCode, ex.Message);
            }
            return await Task.FromResult(response);
        }

        //-------------------------------------------------------------------------------------------------------/
        // GetBranchMaster
        //-------------------------------------------------------------------------------------------------------/
        public override async Task<GetBranchMaster_Response> GetBranchMaster(Compensation.Services.Empty_Request request, ServerCallContext context)
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
                MyAppLog.WriteLog(MyConstant.LogLevel_Critical, "CompensationService", "GetBranchMaster", "1", response.ReturnCode, ex.Message);
            }
            //
            return await Task.FromResult(response);
        }
        //-------------------------------------------------------------------------------------------------------/
        // GetRefEstimationList
        //-------------------------------------------------------------------------------------------------------/
        public override async Task<GetRefEstimationList_Response> GetRefEstimationList(Compensation.Services.Empty_Request request, ServerCallContext context)
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
                MyAppLog.WriteLog(MyConstant.LogLevel_Critical, "CompensationService", "GetRefEstimationList", "1", response.ReturnCode, ex.Message);
            }
            //
            return await Task.FromResult(response);
        }


    }//End class
}//End name space
