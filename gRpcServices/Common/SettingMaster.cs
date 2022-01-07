using Gosu.SystemConfig.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gosu.Common
{
    public static class SettingMaster
    {
        /// <summary>
        /// Get setting master
        /// </summary>
        /// <param name="settingCode"></param>
        /// <returns></returns>
        public async static Task<SettingMasterModel> GetSetting(string settingCode)
        {
            var ret = new SettingMasterModel();
            try
            {
                GetSetting_Response result = await GrpcClientFactory.CallServiceAtSpecificUrl<GetSetting_Response>(GrpcClientFactory.SystemConfigUrl, async channel =>
                {
                    var client = new grpcSystemConfigService.grpcSystemConfigServiceClient(channel);
                    return await client.GetSettingAsync(new String_Request()
                    {
                        Credential = new UserCredential()
                        {
                            Username = GrpcCredential.Username,
                            RoleID = GrpcCredential.RoleID,
                            AccessToken = GrpcCredential.AccessToken,
                            ApiKey = GrpcCredential.ApiKey
                        },
                        StringValue = settingCode
                    });
                });
                //Result
                if (result != null && result.ReturnCode == GrpcReturnCode.OK)
                {
                    ClassHelper.CopyPropertiesData(result, ret);
                    return ret;
                }
            }
            catch { }
            //
            return null;
        }

    }
}
