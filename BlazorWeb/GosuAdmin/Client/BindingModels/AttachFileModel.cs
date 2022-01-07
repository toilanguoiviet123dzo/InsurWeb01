using System;
using System.ComponentModel.DataAnnotations;

namespace GosuAdmin.Client.BindingModels
{
    public class AttachFileModel
    {
        public string ID { get; set; } = "";
        public string VoucherNo { get; set; } = "";
        public DateTime IssueDateTime { get; set; }
        public string CategoryID { get; set; } = "";
        public string CategoryName { get; set; } = "";
        [Required(ErrorMessage = "Chưa chọn file đính kèm")]
        public string FileName { get; set; } = "";
        public string Notes { get; set; } = "";
        public string ResourceID { get; set; } = "";
        public byte[] FileContent { get; set; }
        public bool IsFileChanged { get; set; } = false;
        public int UpdMode { get; set; }
        [Required (ErrorMessage ="Bắt buộc nhập")]
        public int DocumentLevel { get; set; }
        public string DataOwnerID { get; set; }
        public string DocumentLevelName { get; set; } = "";
    }
}
