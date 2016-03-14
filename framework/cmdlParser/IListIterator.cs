using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CeeEmDeeEl
{
    public interface IListIterator<T>
    {
        void Add(T element);
        T GetNext();
        T GetPrevious();
        bool HasNext { get; }
        bool HasPrevious { get; }
        T Next { get; }
        int NextIndex { get; }
        T Previous { get; }
        int PreviousIndex { get; }
        void Remove();
        void Set(T element);
    }
}
