/***************************************************************************************************

Copyright (c) 2018 Intellectual Ventures Property Holdings, LLC (IVPH) All rights reserved.

EMOD is licensed under the Creative Commons Attribution-Noncommercial-ShareAlike 4.0 License.
To view a copy of this license, visit https://creativecommons.org/licenses/by-nc-sa/4.0/legalcode

***************************************************************************************************/

using System;

namespace compartments
{
    public class PriorityQueue<T>
    {
        public struct Node<U>
        {
            private double _priority;
            private U _payload;

            public double Priority
            {
                get { return _priority; }
                set { _priority = value; }
            }

            public U Payload
            {

                get { return _payload; }
                set { _payload = value; }
            }

            public Node(double priority, U payload)
            {
                _priority = priority;
                _payload = payload;
            }
        }

        private int _lastIndex;
        private readonly Node<T>[] _nodes;

        public PriorityQueue(int size)
        {
            // Create the list and counter count
            if (size < 1)
                throw new ArgumentException("Number of reactions is less than one.", "size");

            _nodes     = new Node<T>[size];
            _lastIndex = -1;
        }

        // Test method
        public int Size
        {
            get { return _nodes.Length; }
        }

        // Test method
        public int LastIndex
        {
            get { return _lastIndex; }
        }

        // Test method
        public Node<T>[] Nodes
        {
            get
            {
                Node<T>[] copyOfNodes = new Node<T>[_nodes.Length];
                Array.Copy(_nodes, copyOfNodes, _nodes.Length);
                return copyOfNodes;
            }
        }

        //This will read out the top value 
        public void Top(out double priority, out T payload)
        {
            priority = _nodes[0].Priority;
            payload  = _nodes[0].Payload;
        }

        public Node<T> First
        {
            get { return _nodes[0]; }
        }

        //Find the reaction r, then change the tau to new value.
        private int UpdatePriority(double priority, T target)
        {
            int j;
            Boolean found = false; 
            
            for (j = 0; j <= _lastIndex; j++)
            {
                if (_nodes[j].Payload.Equals(target))
                {
                    _nodes[j].Priority = priority;
                    found = true;
                    break;
                }
            }

            if (!found)
                throw new ArgumentException("The reaction requested is not in the queue.  The payload provided in UpdateIndex() does not exist in the queue.");

            return j;
        }

        //Add a new node then sort it
        public void Add(double priority, T payload)
        {
            _lastIndex++;

            _nodes[_lastIndex].Priority = priority;
            _nodes[_lastIndex].Payload  = payload;

            UpdateIndex(priority, payload);
        }

        public double ShowEntry(int index)
        {
            if ((index > _lastIndex) || (index < 0))
                throw new ArgumentException(String.Format(
                    "Node index {0} is outside of the index range [0,{1}] .", index, _lastIndex));

            return _nodes[index].Priority;
        }

        private void Swap(int index, int swapIndex)
        {
            Node<T> temp      = _nodes[index];
            _nodes[index]     = _nodes[swapIndex];
            _nodes[swapIndex] = temp;
        }

        //Find the tau values of the two children.  If they do not exist, set to positiveinfinity
        private void FindChildren(out double leftChildPriority, out double rightChildPriority, int index)
        {
            int leftChildIndex  = 2 * index + 1;
            int rightChildIndex = 2 * index + 2;

            if (leftChildIndex > _lastIndex)
            {
                leftChildPriority  = double.PositiveInfinity;
                rightChildPriority = double.PositiveInfinity;
            }
            else if (rightChildIndex > _lastIndex)
            {
                leftChildPriority  = _nodes[leftChildIndex].Priority;
                rightChildPriority = double.PositiveInfinity;
            }
            else
            {
                leftChildPriority  = _nodes[leftChildIndex].Priority;
                rightChildPriority = _nodes[rightChildIndex].Priority;
            }
        }

        // This Updates the value of tau for r (if changed), then sorts it relative to parent and child
        public void UpdateIndex(double priority, T payload)
        {
            //Find the Index of the reaction r in the queue.  Also updates Tau if changed.

            int index = UpdatePriority(priority, payload);

            bool finished = false;

            while (!finished)
            {
                //First establish what the two children tau are of node[index]

                double leftChildTau;
                double rightChildTau;
                FindChildren(out leftChildTau, out rightChildTau, index);

                // If the reaction chosen is at the top of the queue, then only look at children.
                // If nothing changes, Set Finished to True, breaking loop.  
                // If a change needs to happen, use Swap(), then switch the Index and start over
                // If Index != 0, then check parent and children for values.  Update accordingly.
                int swapIndex;
                if (index == 0)
                {
                    if ((priority > leftChildTau) || (priority > rightChildTau))
                    {
                        swapIndex = (leftChildTau < rightChildTau) ? (index * 2 + 1) : (index * 2 + 2);
                        Swap(index, swapIndex);
                        index = swapIndex;
                    }
                    else
                    {
                        finished = true;
                    }
                }
                else
                {
                    double parentTau = _nodes[(index - 1) / 2].Priority;

                    if (priority < parentTau)
                    {
                        swapIndex = (index - 1) / 2;
                        Swap(index, swapIndex);
                        index = swapIndex;
                    }
                    else if ((priority > leftChildTau) || (priority > rightChildTau))
                    {
                        swapIndex = (leftChildTau < rightChildTau) ? (index * 2 + 1) : (index * 2 + 2);
                        Swap(index, swapIndex);
                        index = swapIndex;
                    }
                    else
                    {
                        finished = true;
                    }
                }
            }
        }

        public void Clear()
        {
            _lastIndex = -1;
        }
    }
}
