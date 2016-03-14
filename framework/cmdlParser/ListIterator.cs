using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CeeEmDeeEl
{
    public class ListIterator<T> : IListIterator<T>
    {
        IList<T> list;
        int index;
        int lastIndex;
        int maximumIndex;
        LastAction lastAction;

        private enum LastAction
        {
            none,
            add,
            next,
            previous,
            remove,
            set
        }

        public ListIterator(IList<T> list)
        {
            this.list    = list;
            index        = -1;
            lastIndex    = -1;
            maximumIndex = list.Count - 1;
            lastAction   = LastAction.none;
        }

        private bool CanRemove { get { return (lastAction == LastAction.next || lastAction == LastAction.previous || lastAction == LastAction.set); } }
        private bool CanSet    { get { return (lastAction == LastAction.next || lastAction == LastAction.previous); } }

        public bool HasNext     { get { return (index < maximumIndex); } }
        public bool HasPrevious { get { return (index >= 0); } }

        public T Next
        {
            get
            {
                if (!HasNext)
                    throw new ApplicationException("No Next() list element.");

                lastAction = LastAction.next;

                return list[lastIndex = ++index];
            }
        }

        public T Previous
        {
            get
            {
                if (!HasPrevious)
                    throw new ApplicationException("No Previous() list element.");

                lastAction = LastAction.previous;

                return list[lastIndex = index--];
            }
        }

        public int NextIndex { get { return index + 1; } }
        public int PreviousIndex { get { return index; } }

        public void Add(T newElement)
        {
            list.Insert(++index, newElement);

            maximumIndex++;
            lastAction = LastAction.add;
        }

        public void Remove()
        {
            if (!CanRemove)
                throw new ApplicationException("Can't Remove() without Next() or Previous() nor after Add().");

            list.RemoveAt(lastIndex);

            if (lastAction == LastAction.next)
                index--;

            maximumIndex--;
            lastAction = LastAction.remove;
        }

        public void Set(T element)
        {
            if (!CanSet)
                throw new ApplicationException("Can't Set() without Next() or Previous() nor after Add() or Remove().");

            list[index] = element;

            lastAction = LastAction.set;
        }

        public T GetNext() { return Next; }
        public T GetPrevious() { return Previous; }
    }
}
