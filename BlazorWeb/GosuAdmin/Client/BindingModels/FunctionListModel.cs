using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GosuAdmin.Client.BindingModels
{
    public class FunctionListModel
    {
        public string ID { get; set; } = "";
        public string PageID { get; set; } = "";
        public string PageName { get; set; } = "";
        public string Discriptions { get; set; } = "";
        public string F1 { get; set; } = "";
        public string F2 { get; set; } = "";
        public string F3 { get; set; } = "";
        public string F4 { get; set; } = "";
        public string F5 { get; set; } = "";
        public bool CheckF1 { get; set; }
        public bool CheckF2 { get; set; }
        public bool CheckF3 { get; set; }
        public bool CheckF4 { get; set; }
        public bool CheckF5 { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
        public int UpdMode { get; set; }
        public string RoleDetail_ID { get; set; } = "";
        public DateTime RoleDetail_CreatedOn { get; set; }
        public bool IsGranted { get; set; }
    }
}
