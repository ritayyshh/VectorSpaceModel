using Newtonsoft.Json;
namespace K213961
{
    internal class TF_IDF
    {
        public static Dictionary<string, Dictionary<int, double>> CreateTFMatrix(Dictionary<string, Dictionary<int, int>> invertedIndex)
        {
            Dictionary<string, Dictionary<int, double>> tfMatrix = new Dictionary<string, Dictionary<int, double>>();

            foreach (var termDocumentFreqPair in invertedIndex)
            {
                string term = termDocumentFreqPair.Key;
                int docFreq = termDocumentFreqPair.Value[-1]; // Get document frequency
                foreach (var docFreqPair in termDocumentFreqPair.Value)
                {
                    if (docFreqPair.Key == -1) // Skip the document frequency entry
                        continue;

                    int docId = docFreqPair.Key;
                    int termFreq = docFreqPair.Value;
                    double tf = 1 + Math.Log10(termFreq);

                    if (!tfMatrix.ContainsKey(term))
                    {
                        tfMatrix[term] = new Dictionary<int, double>();
                    }
                    tfMatrix[term][docId] = tf;
                }
            }

            return tfMatrix;
        }
        public static Dictionary<string, double> CreateIDFMatrix(Dictionary<string, Dictionary<int, int>> invertedIndex, int totalDocuments)
        {
            Dictionary<string, double> idfMatrix = new Dictionary<string, double>();

            foreach (var termDocumentFreqPair in invertedIndex)
            {
                string term = termDocumentFreqPair.Key;
                int docFreq = termDocumentFreqPair.Value[-1]; // Get document frequency
                double idf = Math.Log10((double)totalDocuments / docFreq);
                idfMatrix[term] = idf;
            }

            return idfMatrix;
        }
        public static Dictionary<string, Dictionary<int, double>> CreateTFIDFMatrix(Dictionary<string, Dictionary<int, double>> tfMatrix, Dictionary<string, double> idfMatrix)
        {
            Dictionary<string, Dictionary<int, double>> tfidfMatrix = new Dictionary<string, Dictionary<int, double>>();

            foreach (var termTfPair in tfMatrix)
            {
                string term = termTfPair.Key;
                foreach (var docTfPair in termTfPair.Value)
                {
                    int docId = docTfPair.Key;
                    double tf = docTfPair.Value;
                    double idf = idfMatrix[term];
                    double tfidf = tf * idf;

                    if (!tfidfMatrix.ContainsKey(term))
                    {
                        tfidfMatrix[term] = new Dictionary<int, double>();
                    }
                    tfidfMatrix[term][docId] = tfidf;
                }
            }

            return tfidfMatrix;
        }
        public static Dictionary<string, double> GenerateQueryVector(string query, Dictionary<string, Dictionary<int, int>> invertedIndex, int totalDocuments)
        {
            // Tokenize and stem the query
            List<string> vowels = new List<string> { "a", "e", "i", "o", "u" }; // Sample list of vowels
            string[] queryTokens = TokenProcessing.Tokenize(query);
            List<string> stemmedQueryTokens = queryTokens.Select(token =>
            {
                string stemmedToken = token.ToLower();
                TokenProcessing.StemmatizeToken(ref stemmedToken, vowels, vowels.Select(v => v[0]).ToList());
                return stemmedToken;
            }).ToList();

            // Compute TF for the query
            Dictionary<string, int> queryTermFrequency = new Dictionary<string, int>();
            foreach (string term in stemmedQueryTokens)
            {
                if (queryTermFrequency.ContainsKey(term))
                {
                    queryTermFrequency[term]++;
                }
                else
                {
                    queryTermFrequency[term] = 1;
                }
            }

            // Compute TFIDF for the query
            Dictionary<string, double> queryTFIDF = new Dictionary<string, double>();
            foreach (var termFreqPair in queryTermFrequency)
            {
                string term = termFreqPair.Key;
                int termFreq = termFreqPair.Value;
                double tf = 1 + Math.Log10(termFreq);
                double idf = invertedIndex.ContainsKey(term) ? Math.Log10((double)totalDocuments / invertedIndex[term][-1]) : 0;
                double tfidf = tf * idf;
                queryTFIDF[term] = tfidf;
            }

            // Print query vector on console
            /*Console.WriteLine("Query Vector:");
            foreach (var termTfidfPair in queryTFIDF)
            {
                Console.WriteLine($"{termTfidfPair.Key}: {termTfidfPair.Value}");
            }*/

            return queryTFIDF;
        }
        public static void SaveTFMatrix(Dictionary<string, Dictionary<int, double>> tfMatrix, string filePath)
        {
            string json = JsonConvert.SerializeObject(tfMatrix, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }

        public static void SaveIDFMatrix(Dictionary<string, double> idfMatrix, string filePath)
        {
            string json = JsonConvert.SerializeObject(idfMatrix, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }
        public static void SaveTFIDFMatrix(Dictionary<string, Dictionary<int, double>> tfidfMatrix, string filePath)
        {
            string json = JsonConvert.SerializeObject(tfidfMatrix, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }
        public static Dictionary<string, Dictionary<int, double>> LoadTFMatrix(string filePath)
        {
            string json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<Dictionary<string, Dictionary<int, double>>>(json);
        }
        public static Dictionary<string, double> LoadIDFMatrix(string filePath)
        {
            string json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<Dictionary<string, double>>(json);
        }
        public static Dictionary<string, Dictionary<int, double>> LoadTFIDFMatrix(string filePath)
        {
            string json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<Dictionary<string, Dictionary<int, double>>>(json);
        }
    }
}
