using System;
using compartments;

using NUnit.Framework;

namespace cmsunittests
{
    [TestFixture, Description("PriorityQueue Tests")]
    class PriorityQueueTest : AssertionHelper
    {
        [Test, Description("Call the Node<int>(priority, payload) constructor")]
        public void InitializeNodeInt()
        {
            const double priority = 1.0;
            const int payload = 1;

            var node = new PriorityQueue<int>.Node<int>(priority, payload);
            Assert.AreEqual(priority, node.Priority);
            Assert.AreEqual(payload, node.Payload);
        }

        [Test, Description("PriorityQueue without Nodes")]
        [ExpectedException("System.ArgumentException")]
        public void PriorityQueueSizeEqualsZero()
        {
            var pq = new PriorityQueue<int>(0);
            //The above line should trigger an exception, if not, the test will fail.
        }

        [Test, Description("PriorityQueue.ShowEntry() with negative index")]
        [ExpectedException("System.ArgumentException")]
        public void PriorityQueueShowEntryWithNegativeIndex()
        {
            const int size = 1;

            var pq = new PriorityQueue<int>(size);

            double showEntryReturn = pq.ShowEntry(-1);
            //The above line should trigger an exception, if not, the test will fail.
        }

        [Test, Description("PriorityQueue.ShowEntry() with index equal to size")]
        [ExpectedException("System.ArgumentException")]
        public void PriorityQueueShowEntryWithIndexEqualsSize()
        {
            const int size = 1;

            var pq = new PriorityQueue<int>(size);

            double showEntryReturn = pq.ShowEntry(size);
            //The above line should trigger an exception, if not, the test will fail.
        }

        [Test, Description("PriorityQueue.UpdateIndex() with Payload value not in the Queue")]
        [ExpectedException("System.ArgumentException")]
        public void PriorityQueueUpdateIndexWithPayloadNotInQueue()
        {
            const int size = 1;

            var pq = new PriorityQueue<int>(size);
            
            pq.Add(1.0, size);
            pq.UpdateIndex(1.4, size + 1);
            //The above line should trigger an exception, if not, the test will fail.
        }

        [Test, Description("PriorityQueue.Clear()")]
        public void PriorityQueueClear()
        {
            const int size = 2;

            var pq = new PriorityQueue<int>(size);

            pq.Add(1.0, 0);
            pq.Add(2.0, 1);
            Assert.AreEqual(1, pq.LastIndex);
            pq.Clear();
            Assert.AreEqual(-1, pq.LastIndex);
            pq.Add(1.0, 2);
            pq.Add(2.0, 3);
            Assert.AreEqual(1, pq.LastIndex);
        }

        [Test, Description(("PriorityQueue with one Node"))]
        public void PriorityQueueSizeEqualsOne()
        {
            const int size = 1;

            var pq = new PriorityQueue<int>(size);
            Assert.AreEqual(size, pq.Size);
            Assert.AreEqual(-1, pq.LastIndex);

            pq.Add(1.0, size);
            Assert.AreEqual(0, pq.LastIndex);
            PriorityQueue<int>.Node<int> node = pq.First;
            double priority;
            int payload;
            pq.Top(out priority, out payload);
            Assert.AreEqual(node.Priority, priority);
            Assert.AreEqual(node.Payload, payload);
            double showEntryReturn = pq.ShowEntry(size - 1);
            Assert.AreEqual(priority, showEntryReturn);

            Console.WriteLine("Priority: " + priority);
            Console.WriteLine("Payload: " + payload);
            Console.WriteLine("Node.Priority: " + node.Priority);
            Console.WriteLine("Node.Payload: " + node.Payload);
            Console.WriteLine("ShowEntry.Priority: " + showEntryReturn);

            pq.UpdateIndex(1.4, size);
            node = pq.First;
            pq.Top(out priority, out payload);
            showEntryReturn = pq.ShowEntry(size-1);
            Assert.AreEqual(node.Priority, priority);
            Assert.AreEqual(node.Payload, payload);
            Assert.AreEqual(priority, showEntryReturn);

            Console.WriteLine("Priority: " + priority);
            Console.WriteLine("Payload: " + payload);
            Console.WriteLine("Node.Priority: " + node.Priority);
            Console.WriteLine("Node.Payload: " + node.Payload);
            Console.WriteLine("ShowEntry.Priority: " + showEntryReturn);
        }

        [Test, Description("PriorityQueue with two Nodes")]
        public void PriorityQueueSizeEqualsTwo()
        {
            const int size = 2;

            var pq = new PriorityQueue<int>(size);
            Assert.AreEqual(size, pq.Size);
            Assert.AreEqual(-1, pq.LastIndex);

            double priority;
            int payload;

            for (int i = 0; i < size; i++)
            {
                pq.Add(i, i);
                Assert.AreEqual((double)i, pq.ShowEntry(i));
                Assert.AreEqual(i, pq.LastIndex);
            }

            PriorityQueue<int>.Node<int> node = pq.First;
            pq.Top(out priority, out payload);
            Assert.AreEqual(node.Priority, priority);
            Assert.AreEqual(node.Payload, payload);
            Assert.AreEqual(priority, pq.ShowEntry(0));

            pq.UpdateIndex(1.5, 0);
            node = pq.First;
            pq.Top(out priority, out payload);
            Assert.AreEqual(node.Priority, priority);
            Assert.AreEqual(node.Payload, payload);
            Assert.AreEqual(priority, pq.ShowEntry(0));
            Assert.AreEqual(1.0, priority);
            Assert.AreEqual(1, payload);
            Assert.AreEqual(1.5, pq.ShowEntry(1));
        }

        [Test, Description("Priority > leftChildTau for index != 0")]
        public void PriorityQueuePriorityGreaterThanLeftChildTauForIndexNotZero()
        {
            const int size = 5;
            var pq = new PriorityQueue<int>(size);
            Assert.AreEqual(size, pq.Size);

            pq.Add(0.0, 0);
            pq.Add(5.0, 1);
            pq.Add(3.0, 2);
            pq.Add(4.0, 3);
            pq.Add(2.0, 4);
            PrintNodes(pq);

            pq.UpdateIndex(3.5, 1);
            pq.UpdateIndex(2.5, 1);
            pq.UpdateIndex(3.5, 0);
            PrintNodes(pq);

            PriorityQueue<int>.Node<int>[] nodes = pq.Nodes;
            Assert.AreEqual(2, nodes[0].Priority);
            Assert.AreEqual(2.5, nodes[1].Priority);
            Assert.AreEqual(3, nodes[2].Priority);
            Assert.AreEqual(3.5, nodes[3].Priority);
            Assert.AreEqual(4, nodes[4].Priority);
        }

        [Test, Description("Priority > rightChildTau for index != 0")]
        public void PriorityQueuePriorityGreaterThanRightChildTauForIndexNotZero()
        {
            const int size = 5;
            var pq = new PriorityQueue<int>(size);
            Assert.AreEqual(size, pq.Size);

            pq.Add(2.0, 4);
            pq.Add(2.5, 1);
            pq.Add(3.0, 2);
            pq.Add(6.0, 0);
            pq.Add(4.0, 3);
            PrintNodes(pq);

            pq.UpdateIndex(5.0, 4);
            PrintNodes(pq);
            PriorityQueue<int>.Node<int>[] nodes = pq.Nodes;
            Assert.AreEqual(2.5, nodes[0].Priority);
            Assert.AreEqual(4, nodes[1].Priority);
            Assert.AreEqual(3, nodes[2].Priority);
            Assert.AreEqual(6, nodes[3].Priority);
            Assert.AreEqual(5, nodes[4].Priority);
        }

        [Test, Description("Priority > leftChildTau && Priority > rightChildTau for index != 0")]
        public void PriorityQueuePriorityGreaterThanLeftChildTauAndGreaterThanRightChildTauForIndexNotZero()
        {
            const int size = 5;
            var pq = new PriorityQueue<int>(size);
            Assert.AreEqual(size, pq.Size);

            pq.Add(0.0, 0);
            pq.Add(2.0, 4);
            pq.Add(3.0, 2);
            pq.Add(2.5, 1);
            pq.Add(4.0, 3);
            PrintNodes(pq);

            pq.UpdateIndex(6.0, 0);
            PrintNodes(pq);

            PriorityQueue<int>.Node<int>[] nodes = pq.Nodes;
            Assert.AreEqual(2, nodes[0].Priority);
            Assert.AreEqual(2.5, nodes[1].Priority);
            Assert.AreEqual(3, nodes[2].Priority);
            Assert.AreEqual(6, nodes[3].Priority);
            Assert.AreEqual(4, nodes[4].Priority);
        }

        [Test, Description("UpdateIndex, first node => second node < first node < third node")]
        public void PriorityQueueUpdateIndexTest12()
        {
            var pq = new PriorityQueue<int>(3);
            pq.Add(1.0, 1);
            pq.Add(2.0, 2);
            pq.Add(3.0, 3);

            // Update 1st node (payload 1) to be between 2nd and 3rd nodes
            pq.UpdateIndex(2.5, 1);

            var nodes = pq.Nodes;
            Assert.AreEqual(2.0, nodes[0].Priority);
            Assert.AreEqual(2.5, nodes[1].Priority);
            Assert.AreEqual(3.0, nodes[2].Priority);
        }

        [Test, Description("UpdateIndex, first node => last node")]
        public void PriorityQueueUpdateIndexTest13()
        {
            var pq = new PriorityQueue<int>(3);
            pq.Add(1.0, 1);
            pq.Add(3.0, 2);
            pq.Add(2.0, 3);

            // Update 1st node (payload 1) to be last
            pq.UpdateIndex(4.0, 1);

            var nodes = pq.Nodes;
            Assert.AreEqual(2.0, nodes[0].Priority);
            Assert.AreEqual(3.0, nodes[1].Priority);
            Assert.AreEqual(4.0, nodes[2].Priority);
        }

        private void PrintNodes<T>(PriorityQueue<T> pq)
        {
            PriorityQueue<T>.Node<T>[] nodes = pq.Nodes;

            for (int i = 0; i < nodes.Length; i++)
            {
                Console.WriteLine("nodes[{0}]: {1} {2}", i, nodes[i].Priority, nodes[i].Payload);
            }
            Console.WriteLine();
        }
    }
}
