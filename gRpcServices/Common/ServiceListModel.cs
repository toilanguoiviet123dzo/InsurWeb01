﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cores.Common
{
    public class ServiceListModel
    {
        public string ServiceName { get; set; } = "";
        public string Descriptions { get; set; } = "";
        public string Host { get; set; } = "";
        public int Port { get; set; }
        public string Url { get; set; }

    }
}
