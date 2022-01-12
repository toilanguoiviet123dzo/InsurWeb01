using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Google.Protobuf.WellKnownTypes;
using Cores.Grpc.Client;
using Cores.Resource.Services;

namespace BlazorApp.Server.Services
{
    public class ResourceService : grpcResourceService.grpcResourceServiceBase
    {
        // SaveCompenRequest
        public override async Task<String_Response> SaveResourceFile(SaveResourceFile_Request request, ServerCallContext context)
        {
            return await GrpcClientFactory.CallServiceAsync<String_Response>(ServiceList.Resource, async channel =>
            {
                var client = new grpcResourceService.grpcResourceServiceClient(channel);
                return await client.SaveResourceFileAsync(request);
            });
        }

        // GetCompenRequest
        public override async Task<GetResourceFile_Response> GetResourceFile(String_Request request, ServerCallContext context)
        {
            return await GrpcClientFactory.CallServiceAsync<GetResourceFile_Response>(ServiceList.Resource, async channel =>
            {
                var client = new grpcResourceService.grpcResourceServiceClient(channel);
                return await client.GetResourceFileAsync(request);
            });
        }


    }
}
