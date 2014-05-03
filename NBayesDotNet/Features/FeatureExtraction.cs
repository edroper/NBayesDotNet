using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NBayesDotNet.DataObjects;

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

namespace NBayesDotNet.Features
{
    /// <summary>
    /// FeatureExtraction class which is used to generate the FeatureStats Object 
    /// from the dataset and perform feature selection by using the Chisquare test.
    /// 
    /// @author Vasilis Vryniotis <bbriniotis at datumbox.com> </summary>
    /// <seealso cref= <a href="http://blog.datumbox.com/developing-a-naive-bayes-text-classifier-in-java/">http://blog.datumbox.com/developing-a-naive-bayes-text-classifier-in-java/</a> </seealso>
    public class FeatureExtraction
    {

        /// <summary>
        /// Generates a FeatureStats Object with metrics about he occurrences of the
        /// keywords in categories, the number of category counts and the total number 
        /// of observations. These stats are used by the feature selection algorithm.
        /// </summary>
        /// <param name="dataset"> </param>
        /// <returns>  </returns>
        public virtual FeatureStats extractFeatureStats(IList<Document> dataset)
        {
            FeatureStats stats = new FeatureStats();

            int categoryCount = 0;
            string category;
            int featureCategoryCount = 0;
            string feature;
            IDictionary<string, int> featureCategoryCounts;
            foreach (Document doc in dataset)
            {
                ++stats.n; //increase the number of observations
                category = doc.category;


                //increase the category counter by one

                if (stats.categoryCounts.ContainsKey(category) != true)
                {
                    stats.categoryCounts[category] = 1;
                }
                else
                {
                    stats.categoryCounts[category] = categoryCount + 1;
                }
                              
             
                foreach (KeyValuePair<string, int> entry in doc.tokens)
                {
                    feature = entry.Key;

                    //get the counts of the feature in the categories

                    if (stats.featureCategoryJointCount.ContainsKey(feature) != true)
                    {
                        stats.featureCategoryJointCount[feature] = new Dictionary<string, int>();
                        featureCategoryCount = 0;
                    }
                    

                    featureCategoryCounts = stats.featureCategoryJointCount[feature];
                    if (featureCategoryCounts.ContainsKey(category))
                    {
                        featureCategoryCount = featureCategoryCounts[category];
                    }                   
                    

                    //increase the number of occurrences of the feature in the category
                    stats.featureCategoryJointCount[feature][category] = ++featureCategoryCount;
                }
            }

            return stats;
        }

        /// <summary>
        /// Perform feature selection by using the chisquare non-parametrical 
        /// statistical test.
        /// </summary>
        /// <param name="stats"> </param>
        /// <param name="criticalLevel"> </param>
        /// <returns>  </returns>
        public virtual IDictionary<string, double?> chisquare(FeatureStats stats, double criticalLevel)
        {
            IDictionary<string, double?> selectedFeatures = new Dictionary<string, double?>();

            string feature;
            string category;
            IDictionary<string, int> categoryList;

            int N1dot, N0dot, N00, N01, N10, N11;
            double chisquareScore;
            double? previousScore;
            foreach (KeyValuePair<string, IDictionary<string, int>> entry1 in stats.featureCategoryJointCount)
            {
                feature = entry1.Key;
                categoryList = entry1.Value;

                //calculate the N1. (number of documents that have the feature)
                N1dot = 0;
                foreach (int count in categoryList.Values)
                {
                    N1dot += count;
                }

                //also the N0. (number of documents that DONT have the feature)
                N0dot = stats.n - N1dot;

                foreach (KeyValuePair<string, int> entry2 in categoryList)
                {
                    category = entry2.Key;
                    N11 = entry2.Value; //N11 is the number of documents that have the feature and belong on the specific category
                    N01 = stats.categoryCounts[category] - N11; //N01 is the total number of documents that do not have the particular feature BUT they belong to the specific category

                    N00 = N0dot - N01; //N00 counts the number of documents that don't have the feature and don't belong to the specific category
                    N10 = N1dot - N11; //N10 counts the number of documents that have the feature and don't belong to the specific category

                    //calculate the chisquare score based on the above statistics
                    chisquareScore = stats.n * Math.Pow(N11 * N00 - N10 * N01, 2) / ((N11 + N01) * (N11 + N10) * (N10 + N00) * (N01 + N00));

                    //if the score is larger than the critical value then add it in the list
                    if (chisquareScore >= criticalLevel)
                    {
                        //previousScore = selectedFeatures[feature];

                        previousScore = 0;
                        if (selectedFeatures.ContainsKey(feature) != true)
                        {
                            previousScore = 0;

                        }
                        else
                        {

                            previousScore = selectedFeatures[feature];
                        }


                        if (previousScore == 0 || chisquareScore > previousScore)
                        {
                            selectedFeatures[feature] = chisquareScore;
                        }
                    }
                }
            }

            return selectedFeatures;
        }
    }


}
