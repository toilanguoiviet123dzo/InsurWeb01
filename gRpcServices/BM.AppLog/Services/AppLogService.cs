using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Cores.Service.Models;
using Cores.AppLog.Services;
using MongoDB.Entities;
using MongoDB.Driver.Linq;
using MongoDB.Driver;
using MongoDB.Bson;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using System.Text;
using Cores.Common;

namespace Cores.Services
{
    public class AppLogService : grpcAppLogService.grpcAppLogServiceBase
    {
        private readonly ILogger<AppLogService> _logger;
        public AppLogService(ILogger<AppLogService> logger)
        {
            _logger = logger;
        }
        //-------------------------------------------------------------------------------------------------------/
        // WriteErrorLog
        //-------------------------------------------------------------------------------------------------------/
        public override async Task<Empty_Response> WriteLog(WriteLog_Request request, ServerCallContext context)
            {

            var response = new Empty_Response();
            response.MsgCode = "";
            response.ReturnCode = 200;//OK
            try
            {
                var newRecord = new mdAppLog();
                ClassHelper.CopyPropertiesData(request, newRecord);
                newRecord.ID = "";
                newRecord.CreatedOn = DateTime.UtcNow;
                await newRecord.SaveAsync();
            }
            catch (Exception ex)
            {
                response.ReturnCode = 500;
                response.MsgCode = ex.Message;
            }
            return await Task.FromResult(response);
        }
        
    }//End class
}//End namespace

