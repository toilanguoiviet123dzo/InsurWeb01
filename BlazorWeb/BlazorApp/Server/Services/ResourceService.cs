using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using MongoDB.Entities;
using MongoDB.Driver.Linq;
using MongoDB.Driver;
using MongoDB.Bson;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using System.Text;
using Grpc.Net.Client;
using Resource.Services;
using Cores.Grpc.Client;
using BlazorApp.Server.Common;
using BlazorApp.Server.Models;
using Cores.Helpers;
using Cores.Utilities;

namespace BlazorApp.Server.Services
{
    public class ResourceService : grpcResourceService.grpcResourceServiceBase
    {
        private readonly ILogger<ResourceService> _logger;
        private static int ArchiveMode = 2;    //1: DB 2:Disk

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
                var resourceFile = new grpcResourceFileModel();
                ClassHelper.CopyPropertiesData(request.ResourceFile, resourceFile);
                response.StringValue = await SaveResourceFile(resourceFile);
            }
            catch (Exception ex)
            {
                response.ReturnCode = GrpcReturnCode.Error_ByServer;
                response.MsgCode = ex.Message;
                MyAppLog.WriteLog(MyConstant.LogLevel_Critical, "ResourceService", "SaveResourceFile", "Exception", response.ReturnCode, ex.Message);
            }
            return await Task.FromResult(response);
        }

        public static async Task<string> SaveResourceFile(grpcResourceFileModel resourceFile)
        {
            string resourceID = "";
            try
            {
                //Archive mode
                resourceFile.ArchiveMode = ArchiveMode;

                //archiveFolder
                SettingMasterModel settingMaster;
                string archiveFolder = "";
                if (resourceFile.ArchiveMode == 2)
                {
                    settingMaster = await SettingMaster.GetSetting("001");
                    archiveFolder = settingMaster == null ? "" : settingMaster.StringValue1;
                }

                //Insert/Update
                if (resourceFile.UpdMode == 1 || resourceFile.UpdMode == 2)
                {
                    var saveRecord = new mdResourceFile();
                    ClassHelper.CopyPropertiesData(resourceFile, saveRecord);
                    //Insert
                    if (resourceFile.UpdMode == 1)
                    {
                        saveRecord.ResourceID = MyCodeGenerator.GenResourceID();
                        saveRecord.ID = "";
                    }
                    saveRecord.ModifiedOn = DateTime.UtcNow;

                    //Save to local file
                    if (resourceFile.ArchiveMode == 2 && !string.IsNullOrWhiteSpace(archiveFolder))
                    {
                        saveRecord.ServerFileName = saveRecord.ResourceID + "_" + saveRecord.FileName;
                        string fileName = archiveFolder + saveRecord.ServerFileName;
                        //Save file
                        MyFile.Write_ToBinary(fileName, saveRecord.FileContent);

                        //Clear file content for DB
                        saveRecord.FileContent = null;
                    }
                    //
                    await saveRecord.SaveAsync();

                    //Return resource ID
                    resourceID = saveRecord.ResourceID;
                }

                //Delete
                if (resourceFile.UpdMode == 3)
                {
                    //Get ServerFileName
                    var findRecord = await DB.Find<mdResourceFile>()
                                          .Match(a => a.ResourceID == resourceFile.ResourceID)
                                          .ExecuteFirstAsync();
                    if (findRecord != null)
                    {
                        //Return resource ID
                        resourceID = findRecord.ResourceID;

                        //Delete local file
                        if (resourceFile.ArchiveMode == 2 && !string.IsNullOrWhiteSpace(archiveFolder))
                        {
                            string fileName = archiveFolder + resourceFile.ServerFileName;
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
                MyAppLog.WriteLog(MyConstant.LogLevel_Critical, "ResourceService", "SaveResourceFile", "Exception", 500, ex.Message);
                return "";
            }
            //
            return resourceID;
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
                MyAppLog.WriteLog(MyConstant.LogLevel_Critical, "ResourceService", "GetResourceFile", "Exception", response.ReturnCode, ex.Message);
            }
            //
            return await Task.FromResult(response);
        }






    }//End class
}//End name space
