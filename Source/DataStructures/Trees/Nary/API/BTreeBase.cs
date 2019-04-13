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
using CSFundamentals.Decoration;

namespace CSFundamentals.DataStructures.Trees.Nary.API
{
    public abstract class BTreeBase<TNode, TKey, TValue>
        where TNode : IBTreeNode<TNode, TKey, TValue>, IComparable<TNode>
        where TKey : IComparable<TKey>
    {
        /// <summary>
        /// Is the root of the tree. 
        /// </summary>
        public TNode Root = default(TNode);

        /// <summary>
        /// Is the maximum number of children for a non-leaf node in this B-Tree. 
        /// </summary>
        public int MaxBranchingDegree { get; private set; }

        public BTreeBase(int maxBranchingDegree)
        {
            MaxBranchingDegree = maxBranchingDegree;
        }

        /// <summary>
        /// Given the set of key values, builds a b-tree by inserting all the key-value pairs. 
        /// </summary>
        /// <param name="keyValues">Is the list of key values to be inserted in the tree. </param>
        /// <returns>Root of the tree. </returns>
        [TimeComplexity(Case.Best, "O(1)")]
        [TimeComplexity(Case.Worst, "O(nLog(n))")]// todo: bases are incorrect
        [TimeComplexity(Case.Average, "O(n(Log(n))")] // todo
        public TNode Build(Dictionary<TKey, TValue> keyValues)
        {
            foreach (KeyValuePair<TKey, TValue> keyValue in keyValues)
            {
                Insert(keyValue);
            }
            return Root;
        }

        // TODO: In time complexities subscripts and superscripts do not look good.
        /// <summary>
        /// Inserts a new key-value pair in the tree and returns root of the tree. 
        /// </summary>
        /// <param name="keyValue">Is the key-value pair to be inserted in the tree. </param>
        /// <returns>Root of the tree. </returns>
        [TimeComplexity(Case.Best, "O(1)", When = "Fist key in the tree is inserted.")]
        [TimeComplexity(Case.Worst, "O(D Log(n)(base:D)")] // where D is max branching factor of the tree. 
        [TimeComplexity(Case.Average, "O(d Log(n)(base d))")] // where d is min branching factor of the tree.  
        public TNode Insert(KeyValuePair<TKey, TValue> keyValue)
        {
            /* Find the leaf node that should contain the new key-value pair. The leaf is found such that the order property of the B-Tree is preserved. */
            TNode leaf = FindLeafToInsertKey(Root, keyValue.Key);

            /* Insert the new keyValue pair in the leaf node. */
            InsertInLeaf(leaf, keyValue);

            return Root;
        }

        public abstract TNode InsertInLeaf(TNode leaf, KeyValuePair<TKey, TValue> keyValue);

        public abstract bool Delete(TKey key);

        /// <summary>
        /// Gets the sorted list of all the key-values in the tree rooted at <paramref name="node">. 
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public abstract List<KeyValuePair<TKey, TValue>> GetSortedKeyValues(TNode node);

        public abstract  TNode FindLeafToInsertKey(TNode root, TKey key);

        /// <summary>
        ///  Searchers the given key in (sub)tree rooted at node <paramref name="root">.
        /// </summary>
        /// <param name="root">The root of the (sub) tree at which search starts. </param>
        /// <param name="key">Is the key to search for.</param>
        /// <returns>The node containing the key if it exists. Otherwise throws an exception. </returns>
        [TimeComplexity(Case.Best, "O(1)", When = "Key is the first item of the first node to visit.")]
        [TimeComplexity(Case.Worst, "O(LogD Log(n)Base(D))")] // Each search with in a node uses binary-search which is Log(K) cost, and since it is constant is not included in this value. 
        [TimeComplexity(Case.Average, "O(Log(d) Log(n)Base(d))")]
        public TNode Search(TNode root, TKey key)
        {
            if (root != null)
            {
                int startIndex = 0;
                int endIndex = root.KeyCount - 1;
                while (startIndex <= endIndex)
                {
                    int middleIndex = (startIndex + endIndex) / 2;
                    if (root.GetKey(middleIndex).CompareTo(key) == 0)
                    {
                        return root;
                    }
                    else if (root.GetKey(middleIndex).CompareTo(key) > 0) /* search left-half of the root.*/
                    {
                        endIndex = middleIndex - 1;
                    }
                    else if (root.GetKey(middleIndex).CompareTo(key) < 0) /* search right-half of the root. */
                    {
                        startIndex = middleIndex + 1;
                    }
                }
                if (startIndex < root.ChildrenCount)
                {
                    return Search(root.GetChild(startIndex), key);
                }
            }
            throw new KeyNotFoundException($"{key.ToString()} is not found in the tree.");
        }

        /// <summary>
        /// Finds the node that contains the maximum key of the subtree rooted at node.
        /// </summary>
        /// <param name="node">The node at which (sub)tree is rooted. </param>
        /// <returns>The node containing the maximum key of the (sub)tree rooted at <paramref name="node">. </returns>
        [TimeComplexity(Case.Best, "O(1)", When = "when node is leaf.")]
        [TimeComplexity(Case.Worst, "O(Log(n))")] // todo base is wrong
        [TimeComplexity(Case.Average, "O(Log(n))")]// todo: base is wrong, .. .
        public TNode GetMaxNode(TNode node)
        {
            if (node.IsLeaf())
            {
                return node;
            }

            return GetMaxNode(node.GetChild(node.ChildrenCount - 1));
        }

        /// <summary>
        /// Finds the node that contains the minimum key of the subtree rooted at <paramref name="node">.
        /// </summary>
        /// <param name="node">The node at which (sub)tree is rooted.</param>
        /// <returns>The node containing the minimum key of the (sub)tree rooted at <paramref name="node">.</returns>
        public TNode GetMinNode(TNode node)
        {
            if (node.IsLeaf())
            {
                return node;
            }
            return GetMinNode(node.GetChild(0));
        }

    }
}
