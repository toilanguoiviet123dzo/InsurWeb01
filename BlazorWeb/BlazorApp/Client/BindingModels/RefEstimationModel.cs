using System;
using System.ComponentModel.DataAnnotations;

namespace BlazorApp.Client.BindingModels
{
    public class RefEstimationModel
    {
        public string TemplateName { get; set; } = "";
        public string CompenNo { get; set; } = "";
        public string ReqPersonName { get; set; } = "";
        public string BranchName { get; set; } = "";
        public string RepairerName { get; set; } = "";
        
    }
}
