﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DQueue.Event
{
    public delegate void ReceiveEventHandler(object src, ReceiveEventArgs e);
}
