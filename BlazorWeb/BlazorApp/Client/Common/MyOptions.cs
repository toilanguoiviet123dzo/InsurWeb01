using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorApp.Client.Common
{
    public static class MyOptions
    {
        public static DialogOptions GetPopupEditFormOptions(MaxWidth size = MaxWidth.Large)
        {
            var options = new DialogOptions()
            {
                MaxWidth = size,
                Position = DialogPosition.Center,
                CloseOnEscapeKey = true,
                DisableBackdropClick = true,
                CloseButton = true,
                FullWidth = true
            };
            return options;
        }
    }
}
