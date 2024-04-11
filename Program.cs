namespace K213961
{
    internal class Program
    {
        static void Main(string[] args)
        {
            List<string> vowels = new List<string> { "a", "e", "i", "o", "u" };
            HashSet<string> stopWords = ReadStopWords("Stopword-List.txt");
            List<string> tokens = new List<string>();
            Dictionary<string, Dictionary<int, int>> invertedIndex;// = new Dictionary<string, Dictionary<int, int>>();
            HashSet<int> allDocumentIds;// = new HashSet<int>();
            //InvertedIndex.CreateInvertedIndex(vowels, stopWords, invertedIndex, allDocumentIds);
            //InvertedIndex.SaveInvertedIndex(invertedIndex, allDocumentIds, "invertedIndex.json");
            var (loadedInvertedIndex, loadedAllDocumentIds) = InvertedIndex.LoadInvertedIndex("invertedIndex.json");
            invertedIndex = loadedInvertedIndex;
            allDocumentIds = loadedAllDocumentIds;
            Dictionary<string, Dictionary<int, double>> tfMatrix;// = TF_IDF.CreateTFMatrix(invertedIndex);
            Dictionary<string, double> idfMatrix;// = TF_IDF.CreateIDFMatrix(invertedIndex, allDocumentIds.Count());
            //TF_IDF.SaveTFMatrix(tfMatrix, "tfMatrix.json");
            //TF_IDF.SaveIDFMatrix(idfMatrix, "idfMatrix.json");
            //tfMatrix = TF_IDF.LoadTFMatrix("tfMatrix.json");
            //idfMatrix = TF_IDF.LoadIDFMatrix("idfMatrix.json");
            Dictionary<string, Dictionary<int, double>> tfidfMatrix;// = TF_IDF.CreateTFIDFMatrix(tfMatrix, idfMatrix);
            //TF_IDF.SaveTFIDFMatrix(tfidfMatrix, "tfidfMatrix.json");
            tfidfMatrix = TF_IDF.LoadTFIDFMatrix("tfidfMatrix.json");
            VSMquery(tfidfMatrix, invertedIndex, allDocumentIds);
        }
        static void VSMquery(Dictionary<string, Dictionary<int, double>> tfidfMatrix, Dictionary<string, Dictionary<int, int>> invertedIndex, HashSet<int> allDocumentIds)
        {
            String query = "";
            while(query != "exit")
            {
                Console.Write("Query: ");
                query = Console.ReadLine();
                Dictionary<string, double> queryVector = TF_IDF.GenerateQueryVector(query, invertedIndex, allDocumentIds.Count());
                List<int> result = CosineSimilarity.ComputeCosineSimilarity(queryVector, tfidfMatrix);
                Console.Write("Result-Set: ");
                if (result.Count == 0)
                    Console.WriteLine("NIL");
                else
                {
                    foreach (var docId in result)
                    {
                        Console.Write($"{docId}, ");
                    }
                    Console.Write("\n");
                }
            }
        }
        static HashSet<string> ReadStopWords(string stopWordsFileName)
        {
            return new HashSet<string>(File.ReadAllLines(stopWordsFileName), StringComparer.OrdinalIgnoreCase);
        }
    }
}
