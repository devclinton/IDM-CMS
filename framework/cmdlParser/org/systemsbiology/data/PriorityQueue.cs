using System;
using System.Diagnostics;
namespace org.systemsbiology.data
{
	/*
	* Copyright (C) 2003 by Institute for Systems Biology,
	* Seattle, Washington, USA.  All rights reserved.
	*
	* This source code is distributed under the GNU Lesser
	* General Public License, the text of which is available at:
	*   http://www.gnu.org/copyleft/lesser.html
	*/


	public class PriorityQueue:Queue
	{
		//UPGRADE_NOTE: Field 'EnclosingInstance' was added to class 'Node' to access its enclosing instance. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1019'"
		//UPGRADE_NOTE: The access modifier for this class or class field has been changed in order to prevent compilation errors due to the visibility level. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1296'"
		public class Node
		{
			private void  InitBlock(PriorityQueue enclosingInstance)
			{
				this.enclosingInstance = enclosingInstance;
			}
			private PriorityQueue enclosingInstance;
			public PriorityQueue Enclosing_Instance
			{
				get
				{
					return enclosingInstance;
				}

			}
			protected internal int mSubtreePopulation;
			protected internal Node mFirstChild;
			protected internal Node mSecondChild;
			protected internal Node mParent;
			protected internal System.Object mPayload;

			public Node(PriorityQueue enclosingInstance, System.Object pPayload)
			{
				InitBlock(enclosingInstance);
				mPayload = pPayload;
				clearTreeLinks();
			}

			public virtual void  clearTreeLinks()
			{
				mParent = null;
				mSubtreePopulation = 0;
				mFirstChild = null;
				mSecondChild = null;
			}
		}

		protected internal AbstractComparator mAbstractComparator;
		protected internal Node mRoot;

		public PriorityQueue(AbstractComparator pAbstractComparator)
		{
			mAbstractComparator = pAbstractComparator;
			mRoot = null;
		}

		public override System.Object peekNext()
		{
			System.Object retObj = null;
			if (null != mRoot)
			{
				retObj = mRoot.mPayload;
			}
			return (retObj);
		}

		public virtual void  checkIntegrity(Node pNode)
		{
			if (null != pNode)
			{
				Debug.Assert(null != pNode.mPayload, "null payload");

				if (null != pNode.mParent)
				{
					Debug.Assert(null != pNode.mParent.mFirstChild, "invalid parent-child link");
					Debug.Assert(pNode.mParent.mFirstChild.Equals(pNode) || pNode.mParent.mSecondChild.Equals(pNode), "parent-child link broken");
					Debug.Assert(mAbstractComparator.compare(pNode.mPayload, pNode.mParent.mPayload) >= 0.0, "parent has a value greater than child");
				}

				if (null != pNode.mFirstChild)
				{
					checkIntegrity(pNode.mFirstChild);
					if (null != pNode.mSecondChild)
					{
						checkIntegrity(pNode.mSecondChild);
					}
				}
				else
				{
					Debug.Assert(null == pNode.mSecondChild, "second child without first child");
				}
			}
		}

		protected internal void  remove(Node pNode, AbstractComparator pAbstractComparator)
		{
			Node firstChild = pNode.mFirstChild;
			Node secondChild = pNode.mSecondChild;
			Node replacement = null;

			Node parent = pNode.mParent;

			if (null != firstChild)
			{
				if (null != secondChild)
				{
					int comp = pAbstractComparator.compare(firstChild.mPayload, secondChild.mPayload);
					if (comp > 0)
					{
						// second child is smaller than first child
						insert(secondChild, firstChild, pAbstractComparator);
						replacement = secondChild;
					}
					else
					{
						// first child is smaller than second child
						insert(firstChild, secondChild, pAbstractComparator);
						replacement = firstChild;
					}
				}
				else
				{
					replacement = firstChild;
				}
				replacement.mParent = parent;
			}
			else
			{
				// do nothing, leave variable "replacement" as null
			}

			if (null != parent)
			{
				Debug.Assert(null != parent.mFirstChild, "parent-child relationship broken");
				if (parent.mFirstChild.Equals(pNode))
				{
					if (null != replacement)
					{
						parent.mFirstChild = replacement;
					}
					else
					{
						parent.mFirstChild = parent.mSecondChild;
						parent.mSecondChild = null;
					}
				}
				else
				{
					parent.mSecondChild = replacement;
				}
				parent.mSubtreePopulation--;
				Debug.Assert(parent.mSubtreePopulation >= 0, "invalid subtree population");
			}
			else
			{
				mRoot = replacement;
			}
		}

		public override System.Object getNext()
		{
			System.Object retObj = null;
			if (null != mRoot)
			{
				retObj = mRoot.mPayload;
				remove(mRoot, mAbstractComparator);
			}
			return (retObj);
		}

		protected internal static void  insert(Node pTree, Node pNode, AbstractComparator pAbstractComparator)
		{
			Node child1 = pTree.mFirstChild;
			if (null != child1)
			{
				Node child2 = pTree.mSecondChild;
				if (null != child2)
				{
					if (child2.mSubtreePopulation > child1.mSubtreePopulation)
					{
						if (pAbstractComparator.compare(child1.mPayload, pNode.mPayload) >= 0)
						{
							// pNode is smaller than first child of pTree
							pTree.mFirstChild = pNode;
							pNode.mParent = pTree;
							insert(pNode, child1, pAbstractComparator);
						}
						else
						{
							// pNode is bigger than first child
							insert(child1, pNode, pAbstractComparator);
						}
					}
					else
					{
						if (pAbstractComparator.compare(child2.mPayload, pNode.mPayload) >= 0)
						{
							// pNode is smaller than second child
							pTree.mSecondChild = pNode;
							pNode.mParent = pTree;
							insert(pNode, child2, pAbstractComparator);
						}
						else
						{
							// pNode is bigger than second child
							insert(child2, pNode, pAbstractComparator);
						}
					}
				}
				else
				{
					pTree.mSecondChild = pNode;
					pNode.mParent = pTree;
				}
			}
			else
			{
				pTree.mFirstChild = pNode;
				pNode.mParent = pTree;
			}

			pTree.mSubtreePopulation += pNode.mSubtreePopulation + 1;
		}

		protected internal void  insertRoot(Node pNode)
		{
			if (null != mRoot)
			{
				System.Object rootObj = mRoot.mPayload;

				if (mAbstractComparator.compare(rootObj, pNode.mPayload) < 0)
				{
					insert(mRoot, pNode, mAbstractComparator);
					// pElement is bigger than mRoot
				}
				else
				{
					pNode.mFirstChild = mRoot;
					pNode.mSubtreePopulation = mRoot.mSubtreePopulation + 1;
					mRoot.mParent = pNode;
					mRoot = pNode;
				}
			}
			else
			{
				mRoot = pNode;
			}
		}

		public override bool add(System.Object pElement)
		{
			insertRoot(new Node(this, pElement));
			return (true);
		}

		public virtual int size()
		{
			int retVal = 0;
			if (null != mRoot)
			{
				retVal = mRoot.mSubtreePopulation;
			}
			return (retVal);
		}

		private void  printRecursive(Node pNode, System.Text.StringBuilder pStringBuffer)
		{
			if (null != pNode)
			{
				pStringBuffer.Append(pNode.mPayload);
				pStringBuffer.Append("(child1=");
				if (null != pNode.mFirstChild)
				{
					printRecursive(pNode.mFirstChild, pStringBuffer);
				}
				else
				{
					pStringBuffer.Append("null");
				}
				pStringBuffer.Append(",child2=");
				if (null != pNode.mSecondChild)
				{
					printRecursive(pNode.mSecondChild, pStringBuffer);
				}
				else
				{
					pStringBuffer.Append("null");
				}
				pStringBuffer.Append(")");
			}
		}

		public override System.String ToString()
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			printRecursive(mRoot, sb);
			return (sb.ToString());
		}

		public virtual void  clear()
		{
			mRoot = null;
		}
	}
}