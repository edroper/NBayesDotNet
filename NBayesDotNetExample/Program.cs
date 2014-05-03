using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using NBayesDotNet.Classifiers;
using NBayesDotNet.DataObjects;

namespace NBayesDotNetExample
{
    class Program
    {
        public static string[] readLines(string filelocation)
        {


            //Reader fileReader = new InputStreamReader(url.openStream(), Charset.forName("UTF-8"));

            IList<string> lines;

            using (StreamReader sr = new StreamReader(filelocation))
            {
                lines = new List<string>();
                string line;
                while ((line = sr.ReadLine()) != null) 
                     {
                    lines.Add(line); 
                }
            }
            return lines.ToArray();
        }

        static void Main(string[] args)
        {
            IDictionary<string, string> trainingFiles = new Dictionary<string, string>();
            trainingFiles["English"] = @"D:\Dropbox\Code\NaiveBayes\NaiveBayes\datasets\training.language.en.txt";
            trainingFiles["French"] = @"D:\Dropbox\Code\NaiveBayes\NaiveBayes\datasets\training.language.fr.txt";
            trainingFiles["German"] = @"D:\Dropbox\Code\NaiveBayes\NaiveBayes\datasets\training.language.de.txt";

            //loading examples in memory
            IDictionary<string, String[]> trainingExamples = new Dictionary<string, String[]>();
            foreach (KeyValuePair<string, string> entry in trainingFiles)
            {
                trainingExamples[entry.Key] = readLines(entry.Value);

            }

            //train classifier
            NaiveBayes nb = new NaiveBayes();
            nb.ChisquareCriticalValue = 6.63; //0.01 pvalue
            nb.train(trainingExamples);

            //get trained classifier knowledgeBase
            NaiveBayesKnowledgeBase knowledgeBase = nb.KnowledgeBase;

            nb = null;
            trainingExamples = null;


            //Use classifier
            nb = new NaiveBayes(knowledgeBase);
            string exampleEn = "Hello, my name is ed and I like to eat bagels. Please don't hurt me, the apples are in my soul.";
            string outputEn = nb.predict(exampleEn);

            Console.WriteLine("The sentence \"{0}\" was classified as \"{1}\"", exampleEn, outputEn);

            string exampleFr = "Bonjour, mon nom est Ed et moi aiment manger des bagels. S'il vous plaît ne me fait pas de mal, les pommes sont dans mon âme.";
            string outputFr = nb.predict(exampleFr);

            Console.Write("The sentence \"{0}\" was classified as \"{1}\"", exampleFr, outputFr);

            string exampleDe = "Hallo, mein Name ist ed und Ich mag Bagels essen. Bitte tu mir nicht weh, die Äpfel sind in meiner Seele.";
            string outputDe = nb.predict(exampleDe);

            Console.Write("The sentence \"{0}\" was classified as \"{1}\"", exampleDe, outputDe);


        }
    }
}
