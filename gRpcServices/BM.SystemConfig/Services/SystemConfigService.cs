using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Gosu.Service.Models;
using MongoDB.Entities;
using MongoDB.Driver.Linq;
using MongoDB.Driver;
using MongoDB.Bson;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using System.Text;
using Gosu.Service;
using Gosu.Common;
using Gosu.SystemConfig.Services;

namespace Gosu.Service
{
    public class SystemConfigService: grpcSystemConfigService.grpcSystemConfigServiceBase
    {
        private readonly ILogger<SystemConfigService> _logger;

        public SystemConfigService(ILogger<SystemConfigService> logger)
        {
            _logger = logger;
        }            
        //-------------------------------------------------------------------------------------------------------/
        // GetServiceList
        //-------------------------------------------------------------------------------------------------------/

        public override async Task<GetServiceList_Response> GetServiceList(Empty_Request request, ServerCallContext context)
        {
            var response = new GetServiceList_Response();
            response.ReturnCode = GrpcReturnCode.OK;
            response.MsgCode = "";
            //
            try
            {
                var findRecords = await DB.Find<mdServiceList>().ExecuteAsync();
                //
                if (findRecords == null)
                {
                    response.ReturnCode = GrpcReturnCode.Error_201;
                    return await Task.FromResult(response);
                }
                //
                findRecords.ForEach(item =>
                {
                    var grpcItem = new grpcServiceListModel();
                    ClassHelper.CopyPropertiesData(item, grpcItem);
                    response.ServiceList.Add(grpcItem);
                });
            }
            catch (Exception ex)
            {
                response.ReturnCode = GrpcReturnCode.Error_ByServer;
                response.MsgCode = ex.Message;
                MyAppLog.WriteLog(MyConstant.LogLevel_Critical, "SystemConfigService", "GetServiceList", "1", response.ReturnCode, ex.Message);
            }
            //
            return await Task.FromResult(response);
        }
        //-------------------------------------------------------------------------------------------------------/
        // GetSetting
        //-------------------------------------------------------------------------------------------------------/
        public override async Task<GetSetting_Response> GetSetting(String_Request request, ServerCallContext context)
        {
            var response = new GetSetting_Response();
            response.ReturnCode = GrpcReturnCode.OK;
            response.MsgCode = "";
            //
            try
            {
                var findRecords = new mdSettingMaster();

                if(request.StringValue != "")
                {
                    findRecords = await DB.Find<mdSettingMaster>()
                            .Match(a => a.Code == request.StringValue)
                            .ExecuteFirstAsync();
                }
                else
                {
                    findRecords = null;
                }                 
                //
                if (findRecords == null)
                {
                    response.ReturnCode = GrpcReturnCode.Error_201;
                    return await Task.FromResult(response);
                }
                else
                {
                    ClassHelper.CopyPropertiesData(findRecords, response);
                }
                //                                                                                
            }
            catch (Exception ex)
            {
                response.ReturnCode = GrpcReturnCode.Error_ByServer;
                response.MsgCode = ex.Message;
                MyAppLog.WriteLog(MyConstant.LogLevel_Critical, "SystemConfigService", "GetSetting", "1", response.ReturnCode, ex.Message);
            }
            //
            return await Task.FromResult(response);
        }
    }
}
