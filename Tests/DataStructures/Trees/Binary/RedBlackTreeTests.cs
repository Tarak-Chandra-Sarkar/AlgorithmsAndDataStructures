﻿/* 
 * Copyright (c) 2019 (PiJei) 
 * 
 * This file is part of CSFundamentalAlgorithms project.
 *
 * CSFundamentalAlgorithms is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * CSFundamentalAlgorithms is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with CSFundamentals.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using CSFundamentals.DataStructures.Trees.Binary;
using CSFundamentalsTests.DataStructures.Trees.Binary.API;
using Microsoft.VisualStudio.TestTools.UnitTesting;

//TODO: Add more tests with bigger trees.

namespace CSFundamentalsTests.DataStructures.Trees.Binary
{
    [TestClass]
    public class RedBlackTreeTests
    {
        /// <summary>
        /// Is a RedBlack tree (A form of balanced BST). 
        /// To visualize this tree built as in <see cref="Init()"/> method, please <see cref="images\redblack-bst.png"/> in current directory. 
        /// </summary>
        private RedBlackTree<int, string> _tree;
        private RedBlackTreeNode<int, string> _root;

        [TestInitialize]
        public void Init()
        {
            _tree = new RedBlackTree<int, string>();
            _root = _tree.Build(Constants.KeyValues);
        }

        [TestMethod]
        public void Build_ExpectsCorrectRedBlackTree()
        {
            HasRedBlackTreeProperties(_tree, _root, 10);
        }

        /// <summary>
        /// For a step by step transition of the RedBlack tree while inserting these keys, please <see cref="images\redblack-bst-insert-stepByStep.png"/>.
        /// </summary>
        [TestMethod]
        public void Insert_SeveralKeysConsecutively_ExpectsACorrectTreeAfterEachInsertion()
        {
            RedBlackTreeNode<int, string> root = null;
            var tree = new RedBlackTree<int, string>();

            var E = new RedBlackTreeNode<int, string>(40, "E");
            root = tree.Insert(root, E);
            HasRedBlackTreeProperties(tree, root, 1);

            var C = new RedBlackTreeNode<int, string>(50, "C");
            root = _tree.Insert(root, C);
            HasRedBlackTreeProperties(tree, root, 2);

            var A = new RedBlackTreeNode<int, string>(47, "A");
            root = tree.Insert(root, A);
            HasRedBlackTreeProperties(tree, root, 3);

            var G = new RedBlackTreeNode<int, string>(45, "G");
            root = tree.Insert(root, G);
            HasRedBlackTreeProperties(tree, root, 4);

            var D = new RedBlackTreeNode<int, string>(20, "D");
            root = tree.Insert(root, D);
            HasRedBlackTreeProperties(tree, root, 5);

            var F = new RedBlackTreeNode<int, string>(35, "F");
            root = tree.Insert(root, F);
            HasRedBlackTreeProperties(tree, root, 6);

            var B = new RedBlackTreeNode<int, string>(30, "B");
            root = tree.Insert(root, B);
            HasRedBlackTreeProperties(tree, root, 7);

            var H = new RedBlackTreeNode<int, string>(10, "H");
            root = tree.Insert(root, H);
            HasRedBlackTreeProperties(tree, root, 8);

            var I = new RedBlackTreeNode<int, string>(80, "I");
            root = tree.Insert(root, I);
            HasRedBlackTreeProperties(tree, root, 9);

            var J = new RedBlackTreeNode<int, string>(42, "J");
            root = tree.Insert(root, J);
            HasRedBlackTreeProperties(tree, root, 10);
        }

        [TestMethod]
        public void Delete_RedNodeWithTWoChildren_ExpectsToBeRepalcedBy50WhichIsBlackWithARedRightChild()
        {
            _root = _tree.Delete(_root, 47);
            HasRedBlackTreeProperties(_tree, _root, 9);
        }

        [TestMethod]
        public void Delete_RedNodeWithTwoChildren_ExpectsToBeReplacedBy35AndIsSubjectToLastCaseBlackSiblingWithLeftRedChild()
        {
            _root = _tree.Delete(_root, 30);
            HasRedBlackTreeProperties(_tree, _root, 9);
        }

        [TestMethod]
        public void Delete_BlackNodeWithOneRedRightChild_ReplaceWithTheRightRedChildWith80AsKeyAndColorItBlack()
        {
            _root = _tree.Delete(_root, 50);
            HasRedBlackTreeProperties(_tree, _root, 9);
        }

        [TestMethod]
        public void Delete_BlackNodeWithOneRedLeftChild_RepalceWithLeftRedChildWith10AsKeyAndColorItBlack()
        {
            _root = _tree.Delete(_root, 20);
            HasRedBlackTreeProperties(_tree, _root, 9);
        }

        [TestMethod]
        public void Delete_Root_ExpectsToBeReplacedBy47WhichIsARedLeafAndHasSimpleDeletion()
        {
            _root = _tree.Delete(_root, 40);
            HasRedBlackTreeProperties(_tree, _root, 9);
            Assert.AreEqual(42, _root.Key);
        }

        [TestMethod]
        public void Delete_BlackLeafNode_BlackSiblingWithARedLeftChild_ExpectsRightRotate()
        {
            _root = _tree.Delete(_root, 35);
            HasRedBlackTreeProperties(_tree, _root, 9);
        }

        [TestMethod]
        public void Delete_BlackNodeWithLeftRedChild_ReplaceWithLeftChildAs42AndColorItBlack()
        {
            _root = _tree.Delete(_root, 45);
            HasRedBlackTreeProperties(_tree, _root, 9);
        }

        [TestMethod]
        public void Delete_RedLeafNode_ExpectsSimpleDelete()
        {
            _root = _tree.Delete(_root, 10);
            HasRedBlackTreeProperties(_tree, _root, 9);
        }

        [TestMethod]
        public void Delete_MultipleKyesConsecutively_ExpectsCorrectTreeAfterEachStep()
        {
            _root = _tree.Delete(_root, 30);
            HasRedBlackTreeProperties(_tree, _root, 9);

            _root = _tree.Delete(_root, 40);
            HasRedBlackTreeProperties(_tree, _root, 8);

            _root = _tree.Delete(_root, 10);
            HasRedBlackTreeProperties(_tree, _root, 7);

            _root = _tree.Delete(_root, 80);
            HasRedBlackTreeProperties(_tree, _root, 6);

            _root = _tree.Delete(_root, 47);
            HasRedBlackTreeProperties(_tree, _root, 5);

            _root = _tree.Delete(_root, 20);
            HasRedBlackTreeProperties(_tree, _root, 4);

            _root = _tree.Delete(_root, 45);
            HasRedBlackTreeProperties(_tree, _root, 3);

            _root = _tree.Delete(_root, 42);
            HasRedBlackTreeProperties(_tree, _root, 2);

            _root = _tree.Delete(_root, 35);
            HasRedBlackTreeProperties(_tree, _root, 1);

            _root = _tree.Delete(_root, 50);
            HasRedBlackTreeProperties(_tree, _root, 0);
        }

        [TestMethod]
        public void IsRed_DefaultColor_ExpectsTrue()
        {
            var node1 = new RedBlackTreeNode<int, string>(10, "string1");
            Assert.IsTrue(_tree.IsRed(node1));
            node1.Color = Color.Black;
            Assert.IsFalse(_tree.IsRed(node1));
        }

        [TestMethod]
        public void IsRed_ColoredBlack_ExpectsFalse()
        {
            var node1 = new RedBlackTreeNode<int, string>(10, "string1")
            {
                Color = Color.Black
            };
            Assert.IsFalse(_tree.IsRed(node1));
        }

        [TestMethod]
        public void IsBlack_DefaultColor_ExpectsFalse()
        {
            var node1 = new RedBlackTreeNode<int, string>(10, "string1");
            Assert.IsFalse(_tree.IsBlack(node1));
        }

        [TestMethod]
        public void IsBlack_ColoredBlack_ExpectsTrue()
        {
            var node1 = new RedBlackTreeNode<int, string>(10, "string1")
            {
                Color = Color.Black
            };
            Assert.IsTrue(_tree.IsBlack(node1));
        }

        [TestMethod]
        public void UpdateParentWithNullingChild()
        {
            var node1 = new RedBlackTreeNode<int, string>(10, "string1");
            var node2 = new RedBlackTreeNode<int, string>(5, "string2");
            var node3 = new RedBlackTreeNode<int, string>(15, "string3");

            node1.Parent = null;
            node1.LeftChild = node2;
            node1.RightChild = node3;

            node2.Parent = node1;
            node2.LeftChild = null;
            node2.RightChild = null;

            node3.Parent = node1;
            node3.LeftChild = null;
            node3.RightChild = null;

            Assert.IsNotNull(node1.LeftChild);
            _tree.UpdateParentWithNullingChild(node1, node2);
            Assert.IsNull(node1.LeftChild);

            var node4 = new RedBlackTreeNode<int, string>(15, "string4");
            _tree.UpdateParentWithNullingChild(node1, node4);
            Assert.IsNull(node1.LeftChild);
            Assert.IsNotNull(node1.RightChild);
        }

        public static void HasRedBlackTreeProperties<TKey, TValue>(RedBlackTree<TKey, TValue> tree, RedBlackTreeNode<TKey, TValue> root, int expectedNodeCount) where TKey : IComparable<TKey>, IEquatable<TKey>
        {
            var inOrderTraversal = new List<RedBlackTreeNode<TKey, TValue>>();
            tree.InOrderTraversal(root, inOrderTraversal);

            // Check order properties.
            Assert.IsTrue(BinarySearchTreeBaseTests.HasBinarySearchTreeOrderProperty<RedBlackTreeNode<TKey, TValue>, TKey, TValue>(root));

            //Check to make sure nodes are not orphaned in the insertion or deletion process. 
            Assert.AreEqual(expectedNodeCount, inOrderTraversal.Count);

            // Check color properties.
            if (root != null)
            {
                Assert.IsTrue(root.Color == Color.Black);
            }

            foreach (RedBlackTreeNode<TKey, TValue> node in inOrderTraversal)
            {
                Assert.IsTrue(node.Color == Color.Red || node.Color == Color.Black);

                if (node.Color == Color.Red)
                {
                    if (node.LeftChild != null)
                    {
                        Assert.AreEqual(Color.Black, node.LeftChild.Color);
                    }
                    if (node.RightChild != null)
                    {
                        Assert.AreEqual(Color.Black, node.RightChild.Color);
                    }

                    /* If node N is red, then its parent must be black. As otherwise its parent is red, and the children of a red parent should all be black, in our case node N, which we assumed is red. */
                    Assert.IsTrue(node.Parent.Color == Color.Black);
                }
            }

            // all paths from a node to its null (leaf) descendants contain the same number of black nodes. 
            foreach (RedBlackTreeNode<TKey, TValue> node in inOrderTraversal)
            {
                List<List<RedBlackTreeNode<TKey, TValue>>> paths = tree.GetAllPathToLeaves(node);
                int shortestPathLength = int.MaxValue;
                int longestPathLength = int.MinValue;
                int firstPathBlackNodeCount = 0;
                if (paths.Count >= 0)
                {
                    firstPathBlackNodeCount = paths[0].Count(n => n.Color == Color.Black);
                }

                for (int i = 1; i < paths.Count; i++)
                {
                    Assert.AreEqual(firstPathBlackNodeCount, paths[i].Count(n => n.Color == Color.Black));
                    if (paths[i].Count > longestPathLength)
                    {
                        longestPathLength = paths[i].Count;
                    }
                    if (paths[i].Count < shortestPathLength)
                    {
                        shortestPathLength = paths[i].Count;
                    }
                }

                // Ensure longest path of a node is not more than twice the shortest path. In the extreme case, shortest path might be all black nodes, and longest path would be alternating between red and black nodes
                Assert.IsTrue(longestPathLength <= 2 * shortestPathLength);
            }
        }
    }
}