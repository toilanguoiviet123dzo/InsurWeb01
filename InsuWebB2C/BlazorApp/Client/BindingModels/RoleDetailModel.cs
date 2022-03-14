using System;
using System.ComponentModel.DataAnnotations;

namespace BlazorApp.Client.BindingModels
{
    public class RoleDetailModel
    {
        public string ID { get; set; } = "";
        public string SystemID { get; set; } = "";
        public string RoleID { get; set; }
        public string PageID { get; set; } = "";
        public string PageName { get; set; } = "";
        public string Discriptions { get; set; } = "";
        public bool F1 { get; set; }
        public bool F2 { get; set; }
        public bool F3 { get; set; }
        public bool F4 { get; set; }
        public bool F5 { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
        public int UpdMode { get; set; }
    }
}