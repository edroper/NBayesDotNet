﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/* 
 * Copyright (C) 2014 Vasilis Vryniotis <bbriniotis at datumbox.com>
 * Converted to C# from Java and modified 2014 by Ed Roper < ed at edroper.com > 
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */
namespace NBayesDotNet.DataObjects
{

    /// <summary>
    /// The FeatureStats Object stores all the fields generated by the FeatureExtraction
    /// class.
    /// 
    /// @author Vasilis Vryniotis <bbriniotis at datumbox.com> </summary>
    /// <seealso cref= <a href="http://blog.datumbox.com/developing-a-naive-bayes-text-classifier-in-java/">http://blog.datumbox.com/developing-a-naive-bayes-text-classifier-in-java/</a> </seealso>
    public class FeatureStats
    {
        /// <summary>
        /// total number of Observations
        /// </summary>
        public int n;

        /// <summary>
        /// It stores the co-occurrences of Feature and Category values
        /// </summary>
        public IDictionary<string, IDictionary<string, int>> featureCategoryJointCount;

        /// <summary>
        /// Measures how many times each category was found in the training dataset.
        /// </summary>
        public IDictionary<string, int> categoryCounts;

        /// <summary>
        /// Constructor
        /// </summary>
        public FeatureStats()
        {
            n = 0;
            featureCategoryJointCount = new Dictionary<string, IDictionary<string, int>>();
            categoryCounts = new Dictionary<string, int>();
        }
    }
}