﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TcpServer.commands
{
    public class ToggleCommand
    {
        public bool state;
        public List<string> handlers;
        public string description;
    }
}
