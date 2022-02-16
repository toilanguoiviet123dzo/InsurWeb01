using System;
using System.ComponentModel.DataAnnotations;

namespace BlazorApp.Client.BindingModels
{
    public class UpLoadFileModel
    {
        public string ID { get; set; } = "";
        public string OwnerID { get; set; } = "";
        public string CategoryID { get; set; } = "";
        public string ResourceID { get; set; } = "";
        public string FileType { get; set; } = "";
        public string Title { get; set; } = "";
        [Required(ErrorMessage = "Chưa chọn file đính kèm")]
        public string FileName { get; set; } = "";
        public byte[] FileContent { get; set; } = new byte[] { };
        public int ArchiveMode { get; set; }
        [Required(ErrorMessage = "Bắt buộc nhập")]
        public int SecureLevel { get; set; }
        public string SecureLevelName { get; set; }
        public string AccountID { get; set; } = "";
        public DateTime IssueDate { get; set; }
        public int UpdMode { get; set; }
        public bool IsFileChanged { get; set; } = false;
    }
}
