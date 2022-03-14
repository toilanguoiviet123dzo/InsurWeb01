using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Cores.Service.Models;
using MongoDB.Entities;
using MongoDB.Driver.Linq;
using MongoDB.Driver;
using MongoDB.Bson;
using Google.Protobuf;
using System.Text;
using Cores.Service;
using Grpc.Net.Client;
using Cores.Grpc.Client;
using Cores.Helpers;
using BlazorApp.Server.Common;
using BlazorApp.Server.Models;
using Cores.Utilities;
using Insure.Services;

namespace BlazorApp.Server.Services
{
    public class InsureService : grpcInsureService.grpcInsureServiceBase
    {
        private readonly ILogger<InsureService> _logger;

        public InsureService(ILogger<InsureService> logger)
        {
            _logger = logger;
        }
        //-------------------------------------------------------------------------------------------------------/
        // SaveClaimRequest
        //-------------------------------------------------------------------------------------------------------/
        public override async Task<Insure.Services.GetProductDetail_Response> GetProductDetail(String_Request request, ServerCallContext context)
        {
            var response = new Insure.Services.GetProductDetail_Response();
            response.ReturnCode = GrpcReturnCode.OK;
            try
            {
                
            }
            catch (Exception ex)
            {
                response.ReturnCode = GrpcReturnCode.Error_ByServer;
                response.MsgCode = ex.Message;
                MyAppLog.WriteLog(MyConstant.LogLevel_Critical, "InsureService", "SaveClaimRequest", "Exception", response.ReturnCode, ex.Message);
            }
            return await Task.FromResult(response);
        }
        



    }//End class
}//End name space
