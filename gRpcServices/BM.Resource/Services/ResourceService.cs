using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Cores.Service.Models;
using Cores.Resource.Services;
using MongoDB.Entities;
using MongoDB.Driver.Linq;
using MongoDB.Driver;
using MongoDB.Bson;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using System.Text;
using Cores.Service;
using Cores.Common;
using Grpc.Net.Client;


namespace Cores.Service.Services
{
    public class ResourceService : grpcResourceService.grpcResourceServiceBase
    {
        private readonly ILogger<ResourceService> _logger;
        private int ArchiveMode = 2;    //1: DB 2:Disk

        public ResourceService(ILogger<ResourceService> logger)
        {
            _logger = logger;
        }
        //-------------------------------------------------------------------------------------------------------/
        // SaveResourceFile
        //-------------------------------------------------------------------------------------------------------/
        public override async Task<Resource.Services.String_Response> SaveResourceFile(SaveResourceFile_Request request, ServerCallContext context)
        {
            var response = new Resource.Services.String_Response();
            response.ReturnCode = GrpcReturnCode.OK;
            try
            {
                //Archive mode
                request.ResourceFile.ArchiveMode = ArchiveMode;

                //archiveFolder
                SettingMasterModel settingMaster;
                string archiveFolder = "";
                if (request.ResourceFile.ArchiveMode == 2)
                {
                    settingMaster = await SettingMaster.GetSetting("001");
                    archiveFolder = settingMaster == null ? "" : settingMaster.StringValue1;
                }

                //Insert/Update
                if (request.ResourceFile.UpdMode == 1 || request.ResourceFile.UpdMode == 2)
                {
                    var saveRecord = new mdResourceFile();
                    ClassHelper.CopyPropertiesData(request.ResourceFile, saveRecord);
                    //Insert
                    if (request.ResourceFile.UpdMode == 1)
                    {
                        saveRecord.ResourceID = MyCodeGenerator.GenResourceID();
                        saveRecord.ID = "";
                    }
                    saveRecord.ModifiedOn = DateTime.UtcNow;

                    //Save to local file
                    if (request.ResourceFile.ArchiveMode == 2 && !string.IsNullOrWhiteSpace(archiveFolder))
                    {
                        saveRecord.ServerFileName = saveRecord.ResourceID + "_" + saveRecord.FileName;
                        string fileName = archiveFolder + saveRecord.ServerFileName;
                        //Save file
                        MyFile.Write_ToBinary(fileName, saveRecord.FileContent);

                        MyAppLog.WriteLog(MyConstant.LogLevel_Critical, "ResourceService", "Write_ToBinary", "1", response.ReturnCode, fileName);

                        //Clear file content for DB
                        saveRecord.FileContent = null;
                    }
                    //
                    await saveRecord.SaveAsync();

                    //Return resource ID
                    response.StringValue = saveRecord.ResourceID;
                }

                //Delete
                if (request.ResourceFile.UpdMode == 3)
                {
                    //Get ServerFileName
                    var findRecord = await DB.Find<mdResourceFile>()
                                          .Match(a => a.ResourceID == request.ResourceFile.ResourceID)
                                          .ExecuteFirstAsync();
                    if (findRecord != null)
                    {
                        //Return resource ID
                        response.StringValue = findRecord.ResourceID;

                        //Delete local file
                        if (request.ResourceFile.ArchiveMode == 2 && !string.IsNullOrWhiteSpace(archiveFolder))
                        {
                            string fileName = archiveFolder + request.ResourceFile.ServerFileName;
                            //
                            MyFile.Delete(fileName);
                        }

                        //Delete record
                        await DB.DeleteAsync<mdResourceFile>(findRecord.ID);
                    }
                }
            }
            catch (Exception ex)
            {
                response.ReturnCode = GrpcReturnCode.Error_ByServer;
                response.MsgCode = ex.Message;
                MyAppLog.WriteLog(MyConstant.LogLevel_Critical, "ResourceService", "SaveResourceFile", "1", response.ReturnCode, ex.Message);
            }
            return await Task.FromResult(response);
        }

        //-------------------------------------------------------------------------------------------------------/
        // GetResourceFile
        //-------------------------------------------------------------------------------------------------------/
        public override async Task<GetResourceFile_Response> GetResourceFile(Resource.Services.String_Request request, ServerCallContext context)
        {
            var response = new GetResourceFile_Response();
            response.ReturnCode = GrpcReturnCode.OK;
            response.MsgCode = "";
            //
            try
            {
                var findRecord = await DB.Find<mdResourceFile>()
                                          .Match(a => a.ResourceID == request.StringValue)
                                          .ExecuteFirstAsync();
                //
                if (findRecord == null)
                {
                    response.ReturnCode = GrpcReturnCode.Error_201;
                    return await Task.FromResult(response);
                }
                else
                {
                    //archiveFolder
                    SettingMasterModel settingMaster;
                    string archiveFolder = "";
                    if (findRecord.ArchiveMode == 2)
                    {
                        settingMaster = await SettingMaster.GetSetting("001");
                        archiveFolder = settingMaster == null ? "" : settingMaster.StringValue1;
                    }

                    //OK
                    response.ResourceFile = new grpcResourceFileModel();
                    ClassHelper.CopyPropertiesData(findRecord, response.ResourceFile);

                    //Load file content
                    if (findRecord.ArchiveMode == 2 && !string.IsNullOrWhiteSpace(archiveFolder))
                    {
                        string fileName = archiveFolder + findRecord.ServerFileName;
                        //
                        response.ResourceFile.FileContent = ClassHelper.ByteString_FromByteArray(MyFile.Load_ToByteArray(fileName));
                    }
                }
            }
            catch (Exception ex)
            {
                response.ReturnCode = GrpcReturnCode.Error_ByServer;
                response.MsgCode = ex.Message;
                MyAppLog.WriteLog(MyConstant.LogLevel_Critical, "ResourceService", "GetResourceFile", "1", response.ReturnCode, ex.Message);
            }
            //
            return await Task.FromResult(response);
        }






    }//End class
}//End name space
