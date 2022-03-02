using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Admin.Services;
using Claim.Services;
using Cores.GrpcClient.Authentication;
using BlazorApp.Client.BindingModels;
using Cores.Helpers;
using Cores.Grpc.Client;

namespace BlazorApp.Client.Services
{
    public class MasterService
    {
        private readonly grpcAdminService.grpcAdminServiceClient _adminServiceClient;
        private readonly grpcClaimService.grpcClaimServiceClient _ClaimServiceClient;
        
        public MasterService(grpcAdminService.grpcAdminServiceClient adminServiceClient,
                             grpcClaimService.grpcClaimServiceClient ClaimServiceClient)
        {
            _adminServiceClient = adminServiceClient;
            _ClaimServiceClient = ClaimServiceClient;
        }
        //OptionLists
        private List<OptionListModel> OptionLists = new List<OptionListModel>();
        public async Task<List<OptionListModel>> Load_OptionList(string ListCode)
        {
            //Get from DB
            if (OptionLists.Count == 0)
            {
                try
                {
                    var request = new Admin.Services.String_Request()
                    {
                        Credential = new Admin.Services.UserCredential()
                        {
                            Username = WebUserCredential.Username,
                            RoleID = WebUserCredential.RoleID,
                            AccessToken = WebUserCredential.AccessToken,
                            ApiKey = WebUserCredential.ApiKey
                        },
                        StringValue = ""

                    };
                    //Get data from DB
                    var response = await _adminServiceClient.GetOptionListAsync(request);
                    if (response != null && response.ReturnCode == 200)
                    {
                        foreach (var item in response.OptionList)
                        {
                            var dataRow = new OptionListModel();
                            ClassHelper.CopyPropertiesDataDateConverted(item, dataRow);
                            OptionLists.Add(dataRow);
                        }
                    }
                    
                }
                catch { }
            }
            //Ger list by ListCode
            var result = new List<OptionListModel>();
            foreach (var record in OptionLists)
            {
                if (record.ListCode == ListCode)
                {
                    var dataRow = new OptionListModel();
                    ClassHelper.CopyPropertiesDataDateConverted(record, dataRow);
                    result.Add(dataRow);
                }
            }
            //Order
            if (result.Count > 0)
            {
                result = result.OrderBy(x => x.DspOrder).ToList<OptionListModel>();
            }
            //
            return result;
        }

        //UserList
        private List<UserAccountModel> UserLists = new List<UserAccountModel>();
        public async Task<List<UserAccountModel>> Load_UserList()
        {
            try
            {
                if (UserLists.Count == 0)
                {
                    var request = new Admin.Services.String_Request();
                    request.Credential = new Admin.Services.UserCredential()
                    {
                        Username = WebUserCredential.Username,
                        RoleID = WebUserCredential.RoleID,
                        AccessToken = WebUserCredential.AccessToken,
                        ApiKey = WebUserCredential.ApiKey
                    };
                    //
                    var response = await _adminServiceClient.GetUserAccountAsync(request);
                    if (response != null && response.ReturnCode == GrpcReturnCode.OK)
                    {
                        foreach (var item in response.UserAccounts)
                        {
                            //Parrent grid
                            var dataRow = new UserAccountModel();
                            ClassHelper.CopyPropertiesDataDateConverted(item, dataRow);
                            //
                            UserLists.Add(dataRow);
                        }
                    }
                    //Order
                    if (UserLists.Count > 0)
                    {
                        UserLists = UserLists.OrderBy(x => x.Fullname).ToList<UserAccountModel>();
                    }
                }
            }
            catch { }
            //
            return UserLists;
        }

        //RepairerLists
        private List<RepairerMasterModel> RepairerLists = new List<RepairerMasterModel>();
        public async Task<List<RepairerMasterModel>> Load_RepairerList()
        {
            try
            {
                if (RepairerLists.Count == 0)
                {
                    var request = new Claim.Services.Empty_Request();
                    request.Credential = new Claim.Services.UserCredential()
                    {
                        Username = WebUserCredential.Username,
                        RoleID = WebUserCredential.RoleID,
                        AccessToken = WebUserCredential.AccessToken,
                        ApiKey = WebUserCredential.ApiKey
                    };
                    //
                    var response = await _ClaimServiceClient.GetRepairerMasterAsync(request);
                    if (response != null && response.ReturnCode == GrpcReturnCode.OK)
                    {
                        foreach (var item in response.Records)
                        {
                            if (item.Status == 1)
                            {
                                var dataRow = new RepairerMasterModel();
                                ClassHelper.CopyPropertiesDataDateConverted(item, dataRow);
                                //
                                RepairerLists.Add(dataRow);
                            }
                        }
                    }
                    //Order
                    if (RepairerLists.Count > 0)
                    {
                        RepairerLists = RepairerLists.OrderBy(x => x.DspOrder).ToList<RepairerMasterModel>();
                    }
                }
            }
            catch { }
            //
            return RepairerLists;
        }
        public async Task<string> Get_RepairAddress(string repairerID)
        {
            if (string.IsNullOrWhiteSpace(repairerID)) return "";
            if (BranchLists.Count == 0)
            {
                await Load_RepairerList();
            }

            //Get address
            var repairer = RepairerLists.Find(x => x.RepairerID == repairerID);
            if (repairer != null) return repairer.Address;
            //
            return "";
        }

        //BranchLists
        private List<BranchMasterModel> BranchLists = new List<BranchMasterModel>();
        public async Task<List<BranchMasterModel>> Load_BranchList()
        {
            try
            {
                if (BranchLists.Count == 0)
                {
                    var request = new Claim.Services.Empty_Request();
                    request.Credential = new Claim.Services.UserCredential()
                    {
                        Username = WebUserCredential.Username,
                        RoleID = WebUserCredential.RoleID,
                        AccessToken = WebUserCredential.AccessToken,
                        ApiKey = WebUserCredential.ApiKey
                    };
                    //
                    var response = await _ClaimServiceClient.GetBranchMasterAsync(request);
                    if (response != null && response.ReturnCode == GrpcReturnCode.OK)
                    {
                        foreach (var item in response.Records)
                        {
                            if (item.Status)
                            {
                                var dataRow = new BranchMasterModel();
                                ClassHelper.CopyPropertiesDataDateConverted(item, dataRow);
                                //
                                BranchLists.Add(dataRow);
                            }
                        }
                    }
                    //Order
                    if (BranchLists.Count > 0)
                    {
                        BranchLists = BranchLists.OrderBy(x => x.DspOrder).ToList<BranchMasterModel>();
                    }
                }
            }
            catch { }
            //
            return BranchLists;
        }
        //
        public async Task<string> Get_BrancheAddress(string brancheID)
        {
            if (string.IsNullOrWhiteSpace(brancheID)) return "";
            if (BranchLists.Count == 0)
            {
                await Load_BranchList();
            }

            //Get address
            var branche = BranchLists.Find(x => x.BranchID == brancheID);
            if (branche != null) return branche.Address;
            //
            return "";
        }


    }
}
