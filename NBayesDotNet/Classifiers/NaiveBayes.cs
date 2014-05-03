using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NBayesDotNet.DataObjects;
using NBayesDotNet.Features;

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

namespace NBayesDotNet.Classifiers
{

    /// <summary>
    /// Implements a basic form of Multinomial Naive Bayes Text Classifier as described at
    /// http://blog.datumbox.com/machine-learning-tutorial-the-naive-bayes-text-classifier/
    /// 
    /// @author Vasilis Vryniotis <bbriniotis at datumbox.com> </summary>
    /// <seealso cref= <a href="http://blog.datumbox.com/developing-a-naive-bayes-text-classifier-in-java/">http://blog.datumbox.com/developing-a-naive-bayes-text-classifier-in-java/</a> </seealso>
    public class NaiveBayes
    {
        private double chisquareCriticalValue = 10.83; //equivalent to pvalue 0.001. It is used by feature selection algorithm

        private NaiveBayesKnowledgeBase knowledgeBase;

        /// <summary>
        /// This constructor is used when we load an already train classifier
        /// </summary>
        /// <param name="knowledgeBase">  </param>
        public NaiveBayes(NaiveBayesKnowledgeBase knowledgeBase)
        {
            this.knowledgeBase = knowledgeBase;
        }

        /// <summary>
        /// This constructor is used when we plan to train a new classifier.
        /// </summary>
        public NaiveBayes()
            : this(null)
        {
        }

        /// <summary>
        /// Gets the knowledgebase parameter
        /// </summary>
        /// <returns>  </returns>
        public virtual NaiveBayesKnowledgeBase KnowledgeBase
        {
            get
            {
                return knowledgeBase;
            }
        }

        /// <summary>
        /// Gets the chisquareCriticalValue paramter.
        /// </summary>
        /// <returns>  </returns>
        public virtual double ChisquareCriticalValue
        {
            get
            {
                return chisquareCriticalValue;
            }
            set
            {
                this.chisquareCriticalValue = value;
            }
        }


        /// <summary>
        /// Preprocesses the original dataset and converts it to a List of Documents.
        /// </summary>
        /// <param name="trainingDataset"> </param>
        /// <returns>  </returns>
        private IList<Document> preprocessDataset(IDictionary<string, String[]> trainingDataset)
        {
            IList<Document> dataset = new List<Document>();

            string category;
            string[] examples;

            Document doc;

            IEnumerator<KeyValuePair<string, String[]>> it = trainingDataset.GetEnumerator();

            //loop through all the categories and training examples
            while (it.MoveNext())
            {
                KeyValuePair<string, String[]> entry = it.Current;
                category = entry.Key;
                examples = entry.Value;

                for (int i = 0; i < examples.Length; ++i)
                {
                    //for each example in the category tokenize its text and convert it into a Document object.
                    doc = TextTokenizer.tokenize(examples[i]);
                    doc.category = category;
                    dataset.Add(doc);

                    //examples[i] = null; //try freeing some memory
                }

                //it.remove(); //try freeing some memory
            }

            return dataset;
        }

        /// <summary>
        /// Gathers the required counts for the features and performs feature selection
        /// on the above counts. It returns a FeatureStats object that is later used 
        /// for calculating the probabilities of the model.
        /// </summary>
        /// <param name="dataset"> </param>
        /// <returns>  </returns>
        private FeatureStats selectFeatures(IList<Document> dataset)
        {
            FeatureExtraction featureExtractor = new FeatureExtraction();

            //the FeatureStats object contains statistics about all the features found in the documents
            FeatureStats stats = featureExtractor.extractFeatureStats(dataset); //extract the stats of the dataset

            //we pass this information to the feature selection algorithm and we get a list with the selected features
            IDictionary<string, double?> selectedFeatures = featureExtractor.chisquare(stats, chisquareCriticalValue);

            //clip from the stats all the features that are not selected
            IEnumerator<KeyValuePair<string, IDictionary<string, int>>> it = stats.featureCategoryJointCount.GetEnumerator();
            while (it.MoveNext())
            {
                string feature = it.Current.Key;

                if (selectedFeatures.ContainsKey(feature) == false)
                {
                    //if the feature is not in the selectedFeatures list remove it
                    it.Current.Value.Remove(feature);

                }
            }

            return stats;
        }

        /// <summary>
        /// Trains a Naive Bayes classifier by using the Multinomial Model by passing
        /// the trainingDataset and the prior probabilities.
        /// </summary>
        /// <param name="trainingDataset"> </param>
        /// <param name="categoryPriors"> </param>
        /// <exception cref="IllegalArgumentException">  </exception>
        public virtual void train(IDictionary<string, String[]> trainingDataset, IDictionary<string, double> categoryPriors)
        {
            //preprocess the given dataset
            IList<Document> dataset = preprocessDataset(trainingDataset);


            //produce the feature stats and select the best features
            FeatureStats featureStats = selectFeatures(dataset);


            //intiliaze the knowledgeBase of the classifier
            knowledgeBase = new NaiveBayesKnowledgeBase();
            knowledgeBase.n = featureStats.n; //number of observations
            knowledgeBase.d = featureStats.featureCategoryJointCount.Count; //number of features


            //check is prior probabilities are given
            if (categoryPriors == null)
            {
                //if not estimate the priors from the sample
                knowledgeBase.c = featureStats.categoryCounts.Count; //number of cateogries
                knowledgeBase.logPriors = new Dictionary<string, double>();

                string category;
                int count;
                foreach (KeyValuePair<string, int> entry in featureStats.categoryCounts)
                {
                    category = entry.Key;
                    count = entry.Value;

                    knowledgeBase.logPriors[category] = Math.Log((double)count / knowledgeBase.n);
                }
            }
            else
            {
                //if they are provided then use the given priors
                knowledgeBase.c = categoryPriors.Count;

                //make sure that the given priors are valid
                if (knowledgeBase.c != featureStats.categoryCounts.Count)
                {
                    throw new System.ArgumentException("Invalid priors Array: Make sure you pass a prior probability for every supported category.");
                }

                string category;
                double priorProbability;
                foreach (KeyValuePair<string, double> entry in categoryPriors)
                {
                    category = entry.Key;
                    priorProbability = entry.Value;
                    if (priorProbability == null)
                    {
                        throw new System.ArgumentException("Invalid priors Array: Make sure you pass a prior probability for every supported category.");
                    }
                    else if (priorProbability < 0 || priorProbability > 1)
                    {
                        throw new System.ArgumentException("Invalid priors Array: Prior probabilities should be between 0 and 1.");
                    }

                    knowledgeBase.logPriors[category] = Math.Log(priorProbability);
                }
            }

            //We are performing laplace smoothing (also known as add-1). This requires to estimate the total feature occurrences in each category
            IDictionary<string, double> featureOccurrencesInCategory = new Dictionary<string, double>();

            int occurrences;
            double featureOccSum;
            foreach (string category in knowledgeBase.logPriors.Keys)
            {
                featureOccSum = 0.0;
                foreach (IDictionary<string, int> categoryListOccurrences in featureStats.featureCategoryJointCount.Values)
                {
                   

                    if (categoryListOccurrences.ContainsKey(category))
                    {
                        occurrences = categoryListOccurrences[category];
                        featureOccSum += occurrences;
                    }
                    
                }
                featureOccurrencesInCategory[category] = featureOccSum;
            }

            //estimate log likelihoods
            string feature;
            int likelycount;
            IDictionary<string, int> featureCategoryCounts;
            double logLikelihood;
            foreach (string category in knowledgeBase.logPriors.Keys)
            {
                foreach (KeyValuePair<string, IDictionary<string, int>> entry in featureStats.featureCategoryJointCount)
                {
                    feature = entry.Key;
                    featureCategoryCounts = entry.Value;

                    if (featureCategoryCounts.ContainsKey(category) != true)
                    {
                        likelycount = 0;
                    }
                    else
                    {
                        likelycount = featureCategoryCounts[category];
                    }


                    logLikelihood = Math.Log((likelycount + 1.0) / (featureOccurrencesInCategory[category] + knowledgeBase.d));
                    if (knowledgeBase.logLikelihoods.ContainsKey(feature) == false)
                    {
                        knowledgeBase.logLikelihoods[feature] = new Dictionary<string, double>();
                    }
                    knowledgeBase.logLikelihoods[feature][category] = logLikelihood;
                }
            }
            featureOccurrencesInCategory = null;
        }

        /// <summary>
        /// Wrapper method of train() which enables the estimation of the prior 
        /// probabilities based on the sample.
        /// </summary>
        /// <param name="trainingDataset">  </param>
        public virtual void train(IDictionary<string, String[]> trainingDataset)
        {
            train(trainingDataset, null);
        }

        /// <summary>
        /// Predicts the category of a text by using an already trained classifier
        /// and returns its category.
        /// </summary>
        /// <param name="text"> </param>
        /// <returns> </returns>
        /// <exception cref="IllegalArgumentException"> </exception>
        public virtual string predict(string text)
        {
            if (knowledgeBase == null)
            {
                throw new System.ArgumentException("Knowledge Bases missing: Make sure you train first a classifier before you use it.");
            }

            //Tokenizes the text and creates a new document
            Document doc = TextTokenizer.tokenize(text);


            string category;
            string feature;
            int occurrences;
            double? logprob;

            string maxScoreCategory = null;
            double? maxScore = double.NegativeInfinity;

            //Map<String, Double> predictionScores = new HashMap<>();
            foreach (KeyValuePair<string, double> entry1 in knowledgeBase.logPriors)
            {
                category = entry1.Key;
                logprob = entry1.Value; //intialize the scores with the priors

                //foreach feature of the document
                foreach (KeyValuePair<string, int> entry2 in doc.tokens)
                {
                    feature = entry2.Key;

                    if (!knowledgeBase.logLikelihoods.ContainsKey(feature))
                    {
                        continue; //if the feature does not exist in the knowledge base skip it
                    }

                    occurrences = entry2.Value; //get its occurrences in text

                    logprob += occurrences * knowledgeBase.logLikelihoods[feature][category]; //multiply loglikelihood score with occurrences
                }
                //predictionScores.put(category, logprob); 

                if (logprob > maxScore)
                {
                    maxScore = logprob;
                    maxScoreCategory = category;
                }
            }

            return maxScoreCategory; //return the category with heighest score
        }
    }

}
