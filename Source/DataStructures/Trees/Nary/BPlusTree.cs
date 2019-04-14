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
using System.Diagnostics.Contracts;
using CSFundamentals.DataStructures.Trees.Nary.API;
using CSFundamentals.Decoration;

// TODO: update summaries: they are copies of the B-Tree and have issues, 

namespace CSFundamentals.DataStructures.Trees.Nary
{
    [DataStructure("B+ Tree")]
    public class BPlusTree<TKey, TValue> :
        BTreeBase<BPlusTreeNode<TKey, TValue>, TKey, TValue>
        where TKey : IComparable<TKey>
    {
        public BPlusTree(int maxBranchingDegree) : base(maxBranchingDegree)
        {
        }

        // TODO: which parts are repeated code, and how can it be shared, ... 
        public override bool Delete(BPlusTreeNode<TKey, TValue> node, TKey key)
        {
            /* In B+ Tree deletes always happen in leaf nodes, because internal nodes do  not store values. */
            Contract.Assert(node.IsLeaf());

            /* Get information about the node that might be needed later, but impossible to retrieve after the key is removed from the node. */
            BPlusTreeNode<TKey, TValue> leftSibling = node.IsRoot() ? null : node.HasLeftSibling() ? node.GetLeftSibling() : null;
            BPlusTreeNode<TKey, TValue> rightSibling = node.IsRoot() ? null : node.HasRightSibling() ? node.GetRightSibling() : null;
            int separatorWithLeftSiblingIndex = leftSibling == null ? -1 : leftSibling.GetIndexAtParentChildren();
            int separatorWithRighthSiblingIndex = rightSibling == null ? -1 : node.GetIndexAtParentChildren();

            /* Remove key from leaf node.*/
            node.RemoveKey(key);

            if (node.IsEmpty() && node.IsRoot())
            {
                Root = null;
                return true;
            }

            if (node.IsUnderFlown())
            {
                ReBalance(node, leftSibling, rightSibling, separatorWithLeftSiblingIndex, separatorWithRighthSiblingIndex);
            }
            return true;


            // TODO: Remember to update the links when joining two leaf nodes, ... 
            // TODO: When is the tree empty because we are not deleting the indexes!

        }

        /// <summary>
        /// Re-balances the tree to restore back its properties. This method is called when node is underFlown, and thus must be fixed. 
        /// </summary>
        /// <param name="node">Specifies an underFlown node. </param>
        /// <param name="leftSibling">Is the left sibling of the underFlown node. </param>
        /// <param name="rightSibling">Is the right sibling of the underFlown node. </param>
        /// <param name="separatorWithLeftSiblingIndex">Is the index of the key in parent that separates node from its left sibling. </param>
        /// <param name="separatorWithRightSiblingIndex">Is the index of the key in parent that separates node from its right sibling. </param>
        [TimeComplexity(Case.Best, "O(1)", When = "There is no need to re-balance, or re-balance does not propagate to upper layers.")]
        [TimeComplexity(Case.Worst, "O(Log(n))")]
        [TimeComplexity(Case.Average, "O(Log(n))")]
        public void ReBalance(BPlusTreeNode<TKey, TValue> node, BPlusTreeNode<TKey, TValue> leftSibling, BPlusTreeNode<TKey, TValue> rightSibling, int separatorWithLeftSiblingIndex, int separatorWithRightSiblingIndex)
        {
            if (node.IsUnderFlown() && !node.IsRoot()) /* B-TRee allows UnderFlown roots*/
            {
                var parent = node.GetParent();
                var parentLeftSibling = parent == null || parent.IsRoot() ? null : parent.HasLeftSibling() ? parent.GetLeftSibling() : null;
                var parentRightSibling = parent == null || parent.IsRoot() ? null : parent.HasRightSibling() ? parent.GetRightSibling() : null;
                int parentSeparatorWithLeftSiblingIndex = parentLeftSibling == null ? -1 : parentLeftSibling.GetIndexAtParentChildren();
                int parentSeparatorWithRightSiblingIndex = parentRightSibling == null ? -1 : parent.GetIndexAtParentChildren();

                if (leftSibling != null && leftSibling.IsMinOneFull())
                {
                    RotateRight(node, leftSibling, separatorWithLeftSiblingIndex);
                    return;
                }
                else if (rightSibling != null && rightSibling.IsMinOneFull())
                {
                    RotateLeft(node, rightSibling, separatorWithRightSiblingIndex);
                    return;
                }
                else if (rightSibling != null && rightSibling.IsMinFull()) /* Meaning rotation wont work, as borrowing key from the siblings via parent will leave the sibling UnderFlown.*/
                {
                    node = Join(rightSibling, node);
                }
                else if (leftSibling != null && leftSibling.IsMinFull())
                {
                    node = Join(node, leftSibling);
                }
                else if (rightSibling == null && leftSibling == null && parent.IsRoot() && node.IsEmpty())
                {
                    // Means this is the last real key-value of the tree, set the root to null. 
                    Root = null;
                    return;
                }

                if (node == null)
                {
                    return;
                }

                ReBalance(node, parentLeftSibling, parentRightSibling, parentSeparatorWithLeftSiblingIndex, parentSeparatorWithRightSiblingIndex);
            }
        }

        /// <summary>
        /// Rotates a key from the left sibling of the node via their parent to the node.
        /// The cost of this operation is at inserting keys and children, in right position (to preserve order), Which at worst is O(K), Where K is the maximum number of keys in a node, and thus is constant. 
        /// </summary>
        /// <param name="node">Is the receiver of a new key. </param>
        /// <param name="leftSibling">The node that lends a key to the process. This key moves to parent, and a key from parent moves to node.</param>
        [TimeComplexity(Case.Best, "O(1)")]
        [TimeComplexity(Case.Worst, "O(K)")] // Constant time as is independent of n: number of keys in tree. 
        [TimeComplexity(Case.Average, "O(K)")] // Constant time as is independent of n: number of keys in tree. 
        internal void RotateRight(BPlusTreeNode<TKey, TValue> node, BPlusTreeNode<TKey, TValue> leftSibling, int separatorIndex)
        {
            if (node.IsLeaf())
            {
                /* 1- Move the last (maximum) key from the left sibling to the underFlown node. */
                node.InsertKeyValue(leftSibling.GetMaxKey());

                /* 2- Remove the last (maximum) key from the left sibling. */
                leftSibling.RemoveKey(leftSibling.GetMaxKey().Key);

                /* 3- Replace separator key in the parent with the current last key of the left sibling. */
                node.GetParent().RemoveKeyByIndex(separatorIndex);
                node.GetParent().InsertKey(leftSibling.GetMaxKey().Key);

                /* Check validity. At this point both the node and its left sibling must be MinFull (have exactly MinKeys keys). */
                Contract.Assert(leftSibling.IsMinFull());
                Contract.Assert(node.IsMinFull());
            }
            else
            {
                /* 1- Move the separator key in the parent to the underFlown node. */
                node.InsertKeyValue(node.GetParent().GetKeyValue(separatorIndex));
                node.GetParent().RemoveKeyByIndex(separatorIndex);

                /* 2- Replace separator key in the parent with the last key of the left sibling. */
                node.GetParent().InsertKeyValue(leftSibling.GetMaxKey());

                /* 3- Remove the last (maximum) key from the left sibling, and move its child to node. */
                leftSibling.RemoveKey(leftSibling.GetMaxKey().Key);
                if (leftSibling.ChildrenCount >= 1)
                {
                    node.InsertChild(leftSibling.GetChild(leftSibling.ChildrenCount - 1));
                    leftSibling.RemoveChildByIndex(leftSibling.ChildrenCount - 1);
                }

                /* Check validity. At this point both the node and its left sibling must be MinFull (have exactly MinKeys keys). */
                Contract.Assert(leftSibling.IsMinFull());
                Contract.Assert(node.IsMinFull());
            }
        }

        /// <summary>
        /// Rotates a key from the right sibling of the node via their parent to the node. 
        /// The cost of this operation is at inserting keys and children, in right position (to preserve order), Which at worst is O(K), Where K is the maximum number of keys in a node, and thus is constant. 
        /// </summary>
        /// <param name="node">Is the receiver of a new key. </param>
        /// <param name="rightSibling">The node that lends a key to the process. This key moves to parent, and a key from parent moves to node. </param>
        [TimeComplexity(Case.Best, "O(1)")]
        [TimeComplexity(Case.Worst, "O(K)")] // Constant time as is independent of n: number of keys in tree. 
        [TimeComplexity(Case.Average, "O(K)")] // Constant time as is independent of n: number of keys in tree. 
        internal void RotateLeft(BPlusTreeNode<TKey, TValue> node, BPlusTreeNode<TKey, TValue> rightSibling, int separatorIndex)
        {
            if (node.IsLeaf())
            {
                /* 1- Move the first (minimum) key from the right sibling to the underFlown node. */
                node.InsertKeyValue(rightSibling.GetMinKey());

                /* 2- Remove the first (minimum) key from the right sibling. */
                rightSibling.RemoveKey(rightSibling.GetMinKey().Key);

                /* 3- Replace separator key in the parent with the current maximum key of the node.*/
                node.GetParent().RemoveKeyByIndex(separatorIndex);
                node.GetParent().InsertKey(node.GetMaxKey().Key);

                /* Check Validity. At this point both the node and its right sibling must be MinFull (have exactly MinKeys keys). */
                Contract.Assert(rightSibling.IsMinFull());
                Contract.Assert(node.IsMinFull());
            }
            else
            {
                /* 1- Move the separator key in the parent to the underFlown node. */
                node.InsertKeyValue(node.GetParent().GetKeyValue(separatorIndex));
                node.GetParent().RemoveKeyByIndex(separatorIndex);

                /* 2- Replace separator key in the parent with the first key of the right sibling.*/
                node.GetParent().InsertKeyValue(rightSibling.GetMinKey());

                /* 3- Remove the first (minimum) key from the right sibling, and move its child to node. */
                rightSibling.RemoveKey(rightSibling.GetMinKey().Key);
                if (rightSibling.ChildrenCount >= 1)
                {
                    node.InsertChild(rightSibling.GetChild(0));
                    rightSibling.RemoveChildByIndex(0);
                }

                /* Check Validity. At this point both the node and its right sibling must be MinFull (have exactly MinKeys keys). */
                Contract.Assert(rightSibling.IsMinFull());
                Contract.Assert(node.IsMinFull());
            }
        }

        /// <summary>
        /// Merges node with its left sibling, such that node can be dropped. Also borrows a key from parent. 
        /// </summary>
        /// <param name="node">The node that will be dissolved at the end of operation. </param>
        /// <param name="leftSibling">The node that will contain keys of the node, its current keys, and a key from parent. </param>
        /// <returns>Parent of the nodes. </returns>
        [TimeComplexity(Case.Best, "O(1)")]
        [TimeComplexity(Case.Worst, "O(K)")] // Constant time as is independent of n: number of keys in tree. 
        [TimeComplexity(Case.Average, "O(K)")] // Constant time as is independent of n: number of keys in tree. 
        internal BPlusTreeNode<TKey, TValue> Join(BPlusTreeNode<TKey, TValue> node, BPlusTreeNode<TKey, TValue> leftSibling)
        {
            var parent = node.GetParent();

            if (!node.IsLeaf())
            {
                // 1- Move separator key to the left node
                int nodeAndLeftSiblingSeparatorKeyAtParentIndex = leftSibling.GetIndexAtParentChildren();
                leftSibling.InsertKeyValue(parent.GetKeyValue(nodeAndLeftSiblingSeparatorKeyAtParentIndex));

                // 2- Remove separator key in the parent, and disconnect parent from node. 
                parent.RemoveKeyByIndex(nodeAndLeftSiblingSeparatorKeyAtParentIndex);
                parent.RemoveChildByIndex(nodeAndLeftSiblingSeparatorKeyAtParentIndex + 1);

                // 3- Join node with leftSibling: Move all the keys and children of node to its left sibling.
                for (int i = 0; i < node.KeyCount; i++)
                {
                    leftSibling.InsertKeyValue(node.GetKeyValue(i));
                }
                for (int i = 0; i < node.ChildrenCount; i++)
                {
                    leftSibling.InsertChild(node.GetChild(i));
                }

                /* Clear node. */
                node.Clear();

                if (parent.IsEmpty() && parent.IsRoot()) /* Can happen if parent is root*/
                {
                    leftSibling.SetParent(null);
                    Root = leftSibling;
                }

                // Since parent has lent a key to its children, it might be UnderFlown now, thus return the parent for additional checks.
                return leftSibling.GetParent();
            }
            else
            {
                int nodeAndLeftSiblingSeparatorKeyAtParentIndex = leftSibling.GetIndexAtParentChildren();

                // 1- Disconnect parent from node. 
                parent.RemoveChildByIndex(nodeAndLeftSiblingSeparatorKeyAtParentIndex + 1);


                // 3- Join node with leftSibling: Move all the keys of node to its left sibling.
                for (int i = 0; i < node.KeyCount; i++)
                {
                    leftSibling.InsertKeyValue(node.GetKeyValue(i));
                }

                // 4- Update the next pointer of the left sibling. 
                leftSibling.NextLeaf = node.NextLeaf;

                /* Clear node. */
                node.Clear();

                // 2- Remove separator key in the parent if needed
                parent.RemoveKeyByIndex(nodeAndLeftSiblingSeparatorKeyAtParentIndex);

                if (parent == Root && parent.ChildrenCount == 1)
                {
                    parent.InsertKey(leftSibling.GetMaxKey().Key);
                }

                /*if (parent.IsEmpty() && parent.IsRoot()) 
                {
                    leftSibling.SetParent(null);
                    Root = leftSibling;
                }*/

                // Since parent has lent a key to its children, it might be UnderFlown now, thus return the parent for additional checks.
                return leftSibling.GetParent();
            }
        }

        public override BPlusTreeNode<TKey, TValue> InsertInLeaf(BPlusTreeNode<TKey, TValue> leaf, KeyValuePair<TKey, TValue> keyValue)
        {
            /* Means this is the first element of the tree, and we should create root. */
            if (leaf == null && Root == null)
            {
                /* 1- Create a leaf node (aka. record) that can contain value besides key.*/
                BPlusTreeNode<TKey, TValue> leafContainingValue = new BPlusTreeNode<TKey, TValue>(MaxBranchingDegree, keyValue);

                /* 2- Create an internal node (here root) that will contain only a copy of the key, and the record as its child. */
                Root = new BPlusTreeNode<TKey, TValue>(
                    MaxBranchingDegree,
                    new List<KeyValuePair<TKey, TValue>>
                    {
                        new KeyValuePair<TKey, TValue>(keyValue.Key, default(TValue))
                    },
                    new List<BPlusTreeNode<TKey, TValue>>
                    {
                        leafContainingValue
                    });
            }
            else if (leaf == null && Root != null) /* This means we are inserting in a root child. */
            {
                leaf = new BPlusTreeNode<TKey, TValue>(MaxBranchingDegree, keyValue);
                Root.InsertChild(leaf);
                int indexAtParent = Root.GetChildIndex(leaf);
                if (indexAtParent > 0)
                {
                    var leftSibling = Root.GetChild(indexAtParent - 1);
                    leftSibling.NextLeaf = leaf;
                    leaf.PreviousLeaf = leftSibling;
                }
            }
            else
            {
                leaf.InsertKeyValue(keyValue);
                Split_Repair(leaf);
            }

            return Root;
        }

        internal void Split_Repair(BPlusTreeNode<TKey, TValue> node)
        {
            while (node.IsOverFlown())
            {
                BPlusTreeNode<TKey, TValue> sibling = node.Split();
                KeyValuePair<TKey, TValue> keyValueToMoveToParent = node.KeyValueToMoveUp();

                if (!node.IsLeaf())
                {
                    node.RemoveKey(keyValueToMoveToParent.Key);
                }
                else // Adjust next and previous links for the leaf nodes.
                {
                    sibling.NextLeaf = node.NextLeaf;
                    sibling.PreviousLeaf = node;
                    node.NextLeaf = sibling;
                }

                var parent = node.GetParent();

                if (parent == null) /* Meaning the overflown node is the root. */
                {
                    /* Create a new root for the tree. */
                    Root = new BPlusTreeNode<TKey, TValue>(MaxBranchingDegree);
                    Root.InsertKey(keyValueToMoveToParent.Key); /* Notice dropping value, as internal nodes in B+Tree do not store values.*/

                    /* Update children of the new node. */
                    /* Notice that InsertChild() method adjusts node's parent internally. */
                    Root.InsertChild(node);
                    Root.InsertChild(sibling);
                    break;
                }
                else
                {
                    parent.InsertKey(keyValueToMoveToParent.Key);
                    parent.InsertChild(sibling);

                    /* Update node, and repeat while loop with the parent node that might itself be overflown now after inserting a new key.*/
                    node = parent;
                }
            }
        }

        /// <summary>
        /// Traverses the doubly linked list at the level of leaves and returns the list of all the key-values in leaves in a sorted order (sorted by key)
        /// </summary>
        /// <returns></returns>
        public override List<KeyValuePair<TKey, TValue>> GetSortedKeyValues(BPlusTreeNode<TKey, TValue> node)
        {
            List<KeyValuePair<TKey, TValue>> keyValues = new List<KeyValuePair<TKey, TValue>>();

            BPlusTreeNode<TKey, TValue> minLeaf = GetMinNode(node);
            while (minLeaf != null)
            {
                keyValues.AddRange(minLeaf.GetKeyValues());
                minLeaf = minLeaf.NextLeaf;
            }
            return keyValues;
        }

        /// <summary>
        /// Starting from the given root, recursively traverses tree top-down to find the proper leaf node, at which <paramref name="key"/> can be inserted. 
        /// </summary>
        /// <param name="root">Is the top-most node at which search for the leaf starts.</param>
        /// <param name="key">Is the key for which a container leaf is being searched. </param>
        /// <returns>Leaf node to insert the key. </returns>
        [TimeComplexity(Case.Best, "O(1)", When = "There is no node in the tree or only one node.")]
        [TimeComplexity(Case.Worst, "O(Log(n))")] // todo
        [TimeComplexity(Case.Average, "O(Log(n))")] // todo 
        public override BPlusTreeNode<TKey, TValue> FindLeafToInsertKey(BPlusTreeNode<TKey, TValue> root, TKey key)
        {
            if (root == null || root.IsLeaf())
            {
                return root;
            }
            for (int i = 0; i < root.KeyCount; i++)
            {
                if (key.CompareTo(root.GetKey(i)) < 0)
                {
                    return FindLeafToInsertKey(root.GetChild(i), key);
                }
                else if (key.CompareTo(root.GetKey(i)) == 0) /* means a node with such key already exists.*/
                {
                    throw new ArgumentException("A node with this key exists in the tree. Duplicate keys are not allowed.");
                }
                else if (i == root.KeyCount - 1 && key.CompareTo(root.GetKey(i)) > 0) /*Last key is treated differently because it also has a child to its right.*/
                {
                    if (root.IsRoot() && root.ChildrenCount <= root.KeyCount)
                    {
                        //return FindLeafToInsertKey(root.GetChild(i), key);
                        //return null;
                        return FindLeafToInsertKey(null, key);
                    }
                    return FindLeafToInsertKey(root.GetChild(i + 1), key);
                }
            }
            return null;
        }


        /// <summary>
        ///  Searchers the given key in leaf nodes of the (sub)tree rooted at node <paramref name="root">.
        /// </summary>
        /// <param name="root">The root of the (sub) tree at which search starts. </param>
        /// <param name="key">Is the key to search for.</param>
        /// <returns>The leaf node containing the key if it exists. Otherwise throws an exception. </returns>
        [TimeComplexity(Case.Best, "O(1)", When = "Key is the first item of the first node to visit.")]
        [TimeComplexity(Case.Worst, "O(LogD Log(n)Base(D))")] // Each search with in a node uses binary-search which is Log(K) cost, and since it is constant is not included in this value. 
        [TimeComplexity(Case.Average, "O(Log(d) Log(n)Base(d))")]
        public override BPlusTreeNode<TKey, TValue> Search(BPlusTreeNode<TKey, TValue> root, TKey key)
        {
            if (root == null)
            {
                throw new KeyNotFoundException($"{key.ToString()} is not found in the tree.");
            }

            /* Perform a binary search in the leaf node to find the key, or throw an exception if it is not found. */
            if (root.IsLeaf())
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

                /* Means the leaf does not contain the key*/
                if (startIndex > endIndex)
                {
                    throw new KeyNotFoundException($"{key.ToString()} is not found in the tree.");
                }
            }

            /* Root is an internal node. Choose the right child to traverse down to find a container leaf. */
            for (int i = 0; i < root.KeyCount; i++)
            {
                if (key.CompareTo(root.GetKey(i)) <= 0)
                {
                    return Search(root.GetChild(i), key);
                }
                else if (i == root.KeyCount - 1 && i <= root.ChildrenCount - 1)
                {
                    return Search(root.GetChild(i + 1), key);
                }
            }

            throw new KeyNotFoundException($"{key.ToString()} is not found in the tree.");
        }
    }
}
