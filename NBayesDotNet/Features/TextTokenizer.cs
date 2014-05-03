using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NBayesDotNet.DataObjects;
using System.Text.RegularExpressions;

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
    /// TextTokenizer class used to tokenize the texts and store them as Document
    /// objects.
    /// 
    /// @author Vasilis Vryniotis <bbriniotis at datumbox.com> </summary>
    /// <seealso cref= <a href="http://blog.datumbox.com/developing-a-naive-bayes-text-classifier-in-java/">http://blog.datumbox.com/developing-a-naive-bayes-text-classifier-in-java/</a> </seealso>
    public class TextTokenizer
    {

        /// <summary>
        /// Preprocess the text by removing punctuation, duplicate spaces and 
        /// lowercasing it.
        /// </summary>
        /// <param name="text"> </param>
        /// <returns>  </returns>
        public static string preprocess(string text)
        {
            //return text.replaceAll("\\p{P}", " ").replaceAll("\\s+", " ").ToLower(Locale.Default);
            return Regex.Replace(text, @"\W|_", " ").ToLower(Thread.CurrentThread.CurrentCulture);

        }

        /// <summary>
        /// A simple method to extract the keywords from the text. For real world 
        /// applications it is necessary to extract also keyword combinations.
        /// </summary>
        /// <param name="text"> </param>
        /// <returns>  </returns>
        public static string[] extractKeywords(string text)
        {
            return text.Split(' ');
        }

        /// <summary>
        /// Counts the number of occurrences of the keywords inside the text.
        /// </summary>
        /// <param name="keywordArray"> </param>
        /// <returns>  </returns>
        public static IDictionary<string, int> getKeywordCounts(string[] keywordArray)
        {
            IDictionary<string, int> counts = new Dictionary<string, int>();

            int counter = 0;
            for (int i = 0; i < keywordArray.Length; ++i)
            {

                if (counts.ContainsKey(keywordArray[i]) != true)
                {
                    counter = 0;
                }
                else
                {
                    counter = counts[keywordArray[i]];
                }
              
                counts[keywordArray[i]] = ++counter; //increase counter for the keyword
            }

            return counts;
        }

        /// <summary>
        /// Tokenizes the document and returns a Document Object.
        /// </summary>
        /// <param name="text"> </param>
        /// <returns>  </returns>
        public static Document tokenize(string text)
        {
            string preprocessedText = preprocess(text);
            string[] keywordArray = extractKeywords(preprocessedText);

            Document doc = new Document();
            doc.tokens = getKeywordCounts(keywordArray);
            return doc;
        }
    }

}
