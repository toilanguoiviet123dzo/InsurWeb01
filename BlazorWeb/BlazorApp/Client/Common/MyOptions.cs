﻿using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorApp.Client.Common
{
    public static class MyOptions
    {
        public static DialogOptions GetMyFormOptions(MaxWidth size = MaxWidth.Large)
        {
            var options = new DialogOptions()
            {
                MaxWidth = size,
                Position = DialogPosition.Center,
                CloseOnEscapeKey = true,
                DisableBackdropClick = true,
                CloseButton = false,
                FullWidth = true
            };
            return options;
        }

        public static DialogOptions GetShowImageOptions(MaxWidth size = MaxWidth.Large)
        {
            var options = new DialogOptions()
            {
                MaxWidth = size,
                Position = DialogPosition.Center,
                CloseOnEscapeKey = true,
                DisableBackdropClick = false,
                CloseButton = true,
                FullWidth = true
            };
            return options;
        }

        public static DialogOptions GetEditFormOptions(MaxWidth size = MaxWidth.Small)
        {
            var options = new DialogOptions()
            {
                MaxWidth = size,
                Position = DialogPosition.Center,
                CloseOnEscapeKey = true,
                DisableBackdropClick = false,
                CloseButton = false,
                FullWidth = true
            };
            return options;
        }

        public static DialogOptions GetMessageBoxOptions(MaxWidth size = MaxWidth.ExtraSmall)
        {
            var options = new DialogOptions()
            {
                MaxWidth = size,
                Position = DialogPosition.Center,
                CloseOnEscapeKey = true,
                DisableBackdropClick = true,
                CloseButton = false,
                FullWidth = true
            };
            return options;
        }
    }
}
