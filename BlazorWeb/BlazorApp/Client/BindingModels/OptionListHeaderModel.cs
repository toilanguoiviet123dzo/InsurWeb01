﻿using System;
using System.ComponentModel.DataAnnotations;

namespace BlazorApp.Client.BindingModels
{
    public class OptionListHeaderModel
    {
        public string ID { get; set; } = "";
        public string ListCode { get; set; } = "";
        public string ListName { get; set; } = "";
        public string Description { get; set; } = "";
        public int UpdMode { get; set; }
    }
}