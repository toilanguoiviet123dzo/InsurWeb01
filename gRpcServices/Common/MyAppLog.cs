using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gosu.SystemConfig.Services;
using Gosu.AppLog.Services;

namespace Gosu.Common
{
    public static class MyAppLog
    {
        public static void WriteLog(int LogLevel,
                                    string Class,
                                    string Method,
                                    string Step,
                                    int ErrorCode,
                                    string ErrorMessage) 
        {
            var request = new Gosu.AppLog.Services.WriteLog_Request();
            request.Credential = new AppLog.Services.UserCredential()
            {
                Username = GrpcCredential.Username,
                RoleID = GrpcCredential.RoleID,
                AccessToken = GrpcCredential.AccessToken,
                ApiKey = GrpcCredential.ApiKey
            };
            request.LogLevel = LogLevel;
            request.Class = Class;
            request.Method = Method;
            request.Step = Step;
            request.ErrorCode = ErrorCode;
            request.ErrorMessage = ErrorMessage;
            //
            GrpcClientFactory.CallServiceFireForget(ServiceList.AppLog, async channel =>
            {
                var client = new grpcAppLogService.grpcAppLogServiceClient(channel);
                await client.WriteLogAsync(request);
            });
        }
    }
}