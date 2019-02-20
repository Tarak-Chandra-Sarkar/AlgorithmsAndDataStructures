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
 * along with CSFundamentalAlgorithms.  If not, see <http://www.gnu.org/licenses/>.
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;
using CSFundamentalAlgorithms.SearchingAlgorithms.StringSearch;
using System.Collections.Generic;

namespace CSFundamentalAlgorithmsTests.SearchingAlgorithmsTests.StringSearchTests
{
    [TestClass]
    public class NaiveSearchTests
    {
        [TestMethod]
        public void NaiveSearch_Search_Test()
        {
            Assert.AreEqual(-1, NaiveSearch.Search(string.Empty, string.Empty));
            Assert.AreEqual(0, NaiveSearch.Search("a", string.Empty));
            Assert.IsTrue(new List<int> { 0, 3, 4 }.Contains(NaiveSearch.Search("abcaab", "a")));
            Assert.IsTrue(new List<int> { 0 }.Contains(NaiveSearch.Search("abcaab", "abc")));
            Assert.AreEqual(-1, NaiveSearch.Search("aaabbbdaacbb", "kjh"));
        }
    }
}