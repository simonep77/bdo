using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Bdo.Objects.Base
{
    internal class SlotAsyncWorkItem<T>
    {
        public int Page { get; set; }
        public int Pos { get; set; }
        public int Offset { get; set; }
        public BusinessSlot Slot { get; set; }
        public IEnumerable<T> Slice { get; set; }
        public Task Runtask { get; set; }

        public Delegate Func;

        public Exception Exception { get; set; }
       
    }
}
