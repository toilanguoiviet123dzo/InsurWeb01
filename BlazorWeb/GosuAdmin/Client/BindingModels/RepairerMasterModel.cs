﻿using System;
using System.ComponentModel.DataAnnotations;

namespace GosuAdmin.Client.BindingModels
{
    public class RepairerMasterModel
    {
        public string ID { get; set; } = "";
        [Required]
        public string RepairerID { get; set; } = "";
        [Required]
        public string RepairerName { get; set; } = "";
        public string PhoneNo { get; set; } = "";
        public string Email { get; set; } = "";
        public string Address { get; set; } = "";
        public string Notes { get; set; } = "";
        public string EstPersonID { get; set; } = "";
        public string EstPersonName { get; set; } = "";
        public int Status { get; set; }
        public string StatusName { get; set; } = "";
        public DateTime ModifiedOn { get; set; }
        public int UpdMode { get; set; }
    }
}
