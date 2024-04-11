namespace K213961
{
    internal class CosineSimilarity
    {
        public static List<int> ComputeCosineSimilarity(Dictionary<string, double> queryVector, Dictionary<string, Dictionary<int, double>> tfidfMatrix)
        {
            var similarityScores = new Dictionary<int, double>();
            double queryNorm = Math.Sqrt(queryVector.Values.Sum(x => x * x));

            foreach (var queryTermVector in queryVector)
            {
                string queryTerm = queryTermVector.Key;
                double queryTfIdf = queryTermVector.Value;

                if (!tfidfMatrix.ContainsKey(queryTerm))
                    continue; // Skip terms not present in the TF-IDF matrix

                foreach (var documentVector in tfidfMatrix[queryTerm])
                {
                    int docId = documentVector.Key;
                    double documentTfIdf = documentVector.Value;

                    if (!similarityScores.ContainsKey(docId))
                    {
                        similarityScores[docId] = 0.0;
                    }

                    similarityScores[docId] += (queryTfIdf * documentTfIdf);
                }
            }

            // Calculate the magnitude for each document vector and the cosine similarity
            foreach (var docId in similarityScores.Keys.ToList())
            {
                double docNorm = Math.Sqrt(tfidfMatrix.Values.SelectMany(d => d.Where(kvp => kvp.Key == docId)).Sum(kvp => kvp.Value * kvp.Value));
                similarityScores[docId] = similarityScores[docId] / (queryNorm * docNorm);
            }

            var sortedDocs = similarityScores.OrderByDescending(x => x.Value).Select(x => x.Key).ToList();
            return sortedDocs;
        }
    }
}