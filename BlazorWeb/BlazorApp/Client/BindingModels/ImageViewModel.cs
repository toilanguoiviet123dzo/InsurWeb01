using System;
using System.ComponentModel.DataAnnotations;

namespace BlazorApp.Client.BindingModels
{
    public class ImageViewModel
    {
        public string ResourceID1 { get; set; } = "";
        public string ResourceID2 { get; set; } = "";
        public string ResourceID3 { get; set; } = "";
        public string ResourceID4 { get; set; } = "";
        public byte[] FileContent1 { get; set; } = new byte[] { };
        public byte[] FileContent2 { get; set; } = new byte[] { };
        public byte[] FileContent3 { get; set; } = new byte[] { };
        public byte[] FileContent4 { get; set; } = new byte[] { };
        public byte[] Thumbnail1 { get; set; } = new byte[] { };
        public byte[] Thumbnail2 { get; set; } = new byte[] { };
        public byte[] Thumbnail3 { get; set; } = new byte[] { };
        public byte[] Thumbnail4 { get; set; } = new byte[] { };
        public string FileType1 { get; set; } = "";
        public string FileType2 { get; set; } = "";
        public string FileType3 { get; set; } = "";
        public string FileType4 { get; set; } = "";
        public string FileName1 { get; set; } = "";
        public string FileName2 { get; set; } = "";
        public string FileName3 { get; set; } = "";
        public string FileName4 { get; set; } = "";
    }
}
