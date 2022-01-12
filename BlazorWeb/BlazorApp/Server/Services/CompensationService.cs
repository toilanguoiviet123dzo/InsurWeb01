using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Google.Protobuf.WellKnownTypes;
using Cores.Grpc.Client;
using Cores.Compensation.Services;

namespace BlazorApp.Server.Services
{
    public class CompensationService : grpcCompensationService.grpcCompensationServiceBase
    {
        // SaveCompenRequest
        public override async Task<String_Response> SaveCompenRequest(SaveCompenRequest_Request request, ServerCallContext context)
        {
            return await GrpcClientFactory.CallServiceAsync<String_Response>(ServiceList.Compensation, async channel =>
            {
                var client = new grpcCompensationService.grpcCompensationServiceClient(channel);
                return await client.SaveCompenRequestAsync(request);
            });
        }

        // GetCompenRequest
        public override async Task<GetCompenRequest_Response> GetCompenRequest(String_Request request, ServerCallContext context)
        {
            return await GrpcClientFactory.CallServiceAsync<GetCompenRequest_Response>(ServiceList.Compensation, async channel =>
            {
                var client = new grpcCompensationService.grpcCompensationServiceClient(channel);
                return await client.GetCompenRequestAsync(request);
            });
        }
        // GetCompenRequestList
        public override async Task<GetCompenRequestList_Response> GetCompenRequestList(GetCompenRequestList_Request request, ServerCallContext context)
        {
            return await GrpcClientFactory.CallServiceAsync<GetCompenRequestList_Response>(ServiceList.Compensation, async channel =>
            {
                var client = new grpcCompensationService.grpcCompensationServiceClient(channel);
                return await client.GetCompenRequestListAsync(request);
            });
        }
        // SaveRepairEstimation
        public override async Task<Empty_Response> SaveRepairEstimation(SaveRepairEstimation_Request request, ServerCallContext context)
        {
            return await GrpcClientFactory.CallServiceAsync<Empty_Response>(ServiceList.Compensation, async channel =>
            {
                var client = new grpcCompensationService.grpcCompensationServiceClient(channel);
                return await client.SaveRepairEstimationAsync(request);
            });
        }
        // GetRepairEstimation
        public override async Task<GetRepairEstimation_Response> GetRepairEstimation(String_Request request, ServerCallContext context)
        {
            return await GrpcClientFactory.CallServiceAsync<GetRepairEstimation_Response>(ServiceList.Compensation, async channel =>
            {
                var client = new grpcCompensationService.grpcCompensationServiceClient(channel);
                return await client.GetRepairEstimationAsync(request);
            });
        }
        // SaveAttachFiles
        public override async Task<Empty_Response> SaveAttachFiles(SaveAttachFiles_Request request, ServerCallContext context)
        {
            return await GrpcClientFactory.CallServiceAsync<Empty_Response>(ServiceList.Compensation, async channel =>
            {
                var client = new grpcCompensationService.grpcCompensationServiceClient(channel);
                return await client.SaveAttachFilesAsync(request);
            });
        }

        // GetAttachFiles
        public override async Task<GetAttachFiles_Response> GetAttachFiles(String_Request request, ServerCallContext context)
        {
            return await GrpcClientFactory.CallServiceAsync<GetAttachFiles_Response>(ServiceList.Compensation, async channel =>
            {
                var client = new grpcCompensationService.grpcCompensationServiceClient(channel);
                return await client.GetAttachFilesAsync(request);
            });
        }
        // GetAttachFileCount
        public override async Task<Int_Response> GetAttachFileCount(GetAttachFileCount_Request request, ServerCallContext context)
        {
            return await GrpcClientFactory.CallServiceAsync<Int_Response>(ServiceList.Compensation, async channel =>
            {
                var client = new grpcCompensationService.grpcCompensationServiceClient(channel);
                return await client.GetAttachFileCountAsync(request);
            });
        }
        // SaveRepairerMaster
        public override async Task<String_Response> SaveRepairerMaster(SaveRepairerMaster_Request request, ServerCallContext context)
        {
            return await GrpcClientFactory.CallServiceAsync<String_Response>(ServiceList.Compensation, async channel =>
            {
                var client = new grpcCompensationService.grpcCompensationServiceClient(channel);
                return await client.SaveRepairerMasterAsync(request);
            });
        }
        // GetRepairerMaster
        public override async Task<GetRepairerMaster_Response> GetRepairerMaster(Empty_Request request, ServerCallContext context)
        {
            return await GrpcClientFactory.CallServiceAsync<GetRepairerMaster_Response>(ServiceList.Compensation, async channel =>
            {
                var client = new grpcCompensationService.grpcCompensationServiceClient(channel);
                return await client.GetRepairerMasterAsync(request);
            });
        }
        // UpdateTotalCompenRequest
        public override async Task<Empty_Response> UpdateTotalCompenRequest(UpdateTotalCompenRequest_Request request, ServerCallContext context)
        {
            return await GrpcClientFactory.CallServiceAsync<Empty_Response>(ServiceList.Compensation, async channel =>
            {
                var client = new grpcCompensationService.grpcCompensationServiceClient(channel);
                return await client.UpdateTotalCompenRequestAsync(request);
            });
        }
        // UpdateStatusCompenRequest
        public override async Task<Empty_Response> UpdateStatusCompenRequest(UpdateStatusCompenRequest_Request request, ServerCallContext context)
        {
            return await GrpcClientFactory.CallServiceAsync<Empty_Response>(ServiceList.Compensation, async channel =>
            {
                var client = new grpcCompensationService.grpcCompensationServiceClient(channel);
                return await client.UpdateStatusCompenRequestAsync(request);
            });
        }
        // SaveBranchMaster
        public override async Task<String_Response> SaveBranchMaster(SaveBranchMaster_Request request, ServerCallContext context)
        {
            return await GrpcClientFactory.CallServiceAsync<String_Response>(ServiceList.Compensation, async channel =>
            {
                var client = new grpcCompensationService.grpcCompensationServiceClient(channel);
                return await client.SaveBranchMasterAsync(request);
            });
        }
        // GetBranchMaster
        public override async Task<GetBranchMaster_Response> GetBranchMaster(Empty_Request request, ServerCallContext context)
        {
            return await GrpcClientFactory.CallServiceAsync<GetBranchMaster_Response>(ServiceList.Compensation, async channel =>
            {
                var client = new grpcCompensationService.grpcCompensationServiceClient(channel);
                return await client.GetBranchMasterAsync(request);
            });
        }
        // GetRefEstimationList
        public override async Task<GetRefEstimationList_Response> GetRefEstimationList(Empty_Request request, ServerCallContext context)
        {
            return await GrpcClientFactory.CallServiceAsync<GetRefEstimationList_Response>(ServiceList.Compensation, async channel =>
            {
                var client = new grpcCompensationService.grpcCompensationServiceClient(channel);
                return await client.GetRefEstimationListAsync(request);
            });
        }




        //
    }
}
