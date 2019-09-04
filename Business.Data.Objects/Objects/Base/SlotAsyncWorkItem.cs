﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Bdo.Objects.Base
{
    internal class SlotAsyncWorkItem
    {
        public int Pos { get; set; }
        public int Offset { get; set; }
        public BusinessSlot Slot { get; set; }
        public DataListBase Slice { get; set; }
        public Thread Thd { get; set; }

        public Delegate Func;

        public bool Ack;
        public Exception Exception { get; set; }
       
    }
}
