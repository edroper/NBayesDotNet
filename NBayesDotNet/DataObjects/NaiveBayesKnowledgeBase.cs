using System;
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
    /// The NaiveBayesKnowledgeBase Object stores all the fields that the classifier
    /// learns during training.
    /// 
    /// @author Vasilis Vryniotis <bbriniotis at datumbox.com> </summary>
    /// <seealso cref= <a href="http://blog.datumbox.com/developing-a-naive-bayes-text-classifier-in-java/">http://blog.datumbox.com/developing-a-naive-bayes-text-classifier-in-java/</a> </seealso>
    public class NaiveBayesKnowledgeBase
    {
        /// <summary>
        /// number of training observations
        /// </summary>
        public int n = 0;

        /// <summary>
        /// number of categories
        /// </summary>
        public int c = 0;

        /// <summary>
        /// number of features
        /// </summary>
        public int d = 0;

        /// <summary>
        /// log priors for log( P(c) )
        /// </summary>
        public IDictionary<string, double> logPriors = new Dictionary<string, double>();

        /// <summary>
        /// log likelihood for log( P(x|c) ) 
        /// </summary>
        public IDictionary<string, IDictionary<string, double>> logLikelihoods = new Dictionary<string, IDictionary<string, double>>();
    }

}
