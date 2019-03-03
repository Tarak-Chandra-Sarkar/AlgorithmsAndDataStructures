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
using CSFundamentals.Styling;

/* Similar to MinBinaryHeap. Refer to that class for missing explanations. */

namespace CSFundamentals.DataStructures.BinaryHeaps
{
    /// <summary>
    /// Implements a Max Binary Heap, and its main operations.
    /// </summary>
    [DataStructure("MaxBinaryHeap")]
    public class MaxBinaryHeap : BinaryHeapBase
    {
        public MaxBinaryHeap(List<int> array) : base(array)
        {

        }

        /// <summary>
        /// Builds an in-place max heap on the given array. 
        /// </summary>
        /// <param name="heapArrayLength">Specifies the length/size of the heap array. </param>
        public override void BuildHeap_Recursively(int heapArrayLength)
        {
            for (int i = heapArrayLength / 2; i >= 0; i--)
            {
                BubbleDown_Recursively(i, heapArrayLength);
            }
        }

        /// <summary>
        /// Is the iterative version of BuildHeap_Recursively. Expect to see exact same results for these two methods. 
        /// </summary>
        /// <param name="heapArrayLength">Specifies the length/size of the heap array. </param>
        public override void BuildHeap_Iteratively(int heapArrayLength)
        {
            for (int i = heapArrayLength / 2; i >= 0; i--)
            {
                BubbleDown_Iteratively(i, heapArrayLength);
            }
        }

        /// <summary>
        /// Inserts a new value into the Max Heap. 
        /// </summary>
        /// <param name="newValue">Specifies the new value to be inserted in the tree.</param>
        /// <param name="heapArrayLength">Specifies the length/size of the heap array. </param>
        public override void Insert(int value, int heapArrayLength)
        {
            HeapArray.Add(value);// means gets added to the end of the list. 

            // Bubble up this element/node
            int nodeIndex = heapArrayLength;
            BubbleUp_Iteratively(nodeIndex, heapArrayLength + 1); // Notice that the size of the array is grown by one now. 
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="heapArrayLength">Specifies the length/size of the heap array. </param>
        public override void BubbleUp_Iteratively(int index, int heapArrayLength)
        {
            int parentIndex = GetParentIndex(index);

            if (parentIndex < 0 || parentIndex >= heapArrayLength) /* Checks for corner cases. */
            {
                return;
            }

            while (index != 0 && HeapArray[parentIndex] < HeapArray[index])
            {
                Swap(HeapArray, parentIndex, index);
                index = parentIndex;
                parentIndex = GetParentIndex(index);
            }
        }

        /// <summary>
        /// Removes the max element from the heap.
        /// </summary>
        /// <param name="rootValue">If the operation is successful, contains the maximum element in the array.</param>
        /// <param name="heapArrayLength">Specifies the length/size of the heap array. </param>
        /// <returns>True in case of success, and false otherwise</returns>
        public override bool TryRemoveRoot(out int rootValue, int heapArrayLength)
        {
            rootValue = Int32.MaxValue;

            if (heapArrayLength == 0)
            {
                return false;
            }
            if (heapArrayLength == 1)
            {
                rootValue = HeapArray[0];
                HeapArray.Clear();
                return true;
            }

            rootValue = HeapArray[0];
            HeapArray[0] = HeapArray[heapArrayLength - 1];
            HeapArray.RemoveAt(heapArrayLength - 1);
            BubbleDown_Recursively(0, heapArrayLength - 1); /* notice that the array is shorter by one value now, thus the new array length is one smaller. */

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rootValue"></param>
        /// <param name="heapArrayLength">Specifies the length/size of the heap array. </param>
        /// <returns></returns>
        public override bool TryFindRoot(out int rootValue, int heapArrayLength)
        {
            if (HeapArray.Any())
            {
                rootValue = HeapArray[0];
                return true;
            }
            rootValue = int.MinValue;
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rootIndex"></param>
        /// <param name="heapArrayLength">Specifies the length/size of the heap array. </param>
        public override void BubbleDown_Recursively(int rootIndex, int heapArrayLength)
        {
            int leftChildIndex = GetLeftChildIndexInHeapArray(rootIndex);
            int rightChildIndex = GetRightChildIndexInHeapArray(rootIndex);
            int maxElementIndex = rootIndex;

            if (TryFindMaxIndex(HeapArray, heapArrayLength, new List<int> { leftChildIndex, rightChildIndex }, HeapArray[maxElementIndex], out int maxIndex))
            {
                maxElementIndex = maxIndex;
            }

            if (maxElementIndex != rootIndex)
            {
                Swap(HeapArray, maxElementIndex, rootIndex);

                if (GetLeftChildIndexInHeapArray(maxElementIndex) < heapArrayLength)
                {
                    BubbleDown_Recursively(maxElementIndex, heapArrayLength);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rootIndex"></param>
        /// <param name="heapArrayLength">Specifies the length/size of the heap array. </param>
        public override void BubbleDown_Iteratively(int rootIndex, int heapArrayLength)
        {
            while (GetLeftChildIndexInHeapArray(rootIndex) < heapArrayLength)
            {
                int leftChildIndex = GetLeftChildIndexInHeapArray(rootIndex);
                int rightChildIndex = GetRightChildIndexInHeapArray(rootIndex);
                int maxElementIndex = rootIndex;

                if (TryFindMaxIndex(HeapArray, heapArrayLength, new List<int> { leftChildIndex, rightChildIndex }, HeapArray[rootIndex], out int maxIndex))
                {
                    maxElementIndex = maxIndex;
                }

                if (maxElementIndex != rootIndex)
                {
                    Swap(HeapArray, maxElementIndex, rootIndex);
                    rootIndex = maxElementIndex;
                }
                else
                {
                    if (TryFindMaxIndex(HeapArray, heapArrayLength, new List<int> { leftChildIndex, rightChildIndex }, Int32.MinValue, out int maxChildIndex))
                    {
                        rootIndex = maxChildIndex;
                    }
                    else
                    {
                        break; // This branch of the code is not expected to be executed.
                    }
                }
            }
        }
    }
}