using Newtonsoft.Json;
namespace K213961
{
    internal class InvertedIndex
    {
        public static void CreateInvertedIndex(List<string> vowels, HashSet<string> stopWords, Dictionary<string, Dictionary<int, int>> invertedIndex, HashSet<int> allDocumentIds)
        {
            string folderPath = "ResearchPapers";

            foreach (int documentId in Enumerable.Range(1, 26))
            {
                string filePath = Path.Combine(folderPath, documentId + ".txt");

                if (File.Exists(filePath))
                {
                    allDocumentIds.Add(documentId);
                    string content = File.ReadAllText(filePath);
                    string[] documentTokens = TokenProcessing.Tokenize(content);

                    foreach (string originalTerm in documentTokens)
                    {
                        string term = originalTerm.ToLower();

                        if (!stopWords.Contains(term))
                        {
                            TokenProcessing.StemmatizeToken(ref term, vowels, vowels.Select(v => v[0]).ToList());

                            if (!invertedIndex.ContainsKey(term))
                            {
                                invertedIndex[term] = new Dictionary<int, int>();
                                invertedIndex[term][documentId] = 1; // Initialize term frequency for this document
                            }
                            else
                            {
                                if (!invertedIndex[term].ContainsKey(documentId))
                                    invertedIndex[term][documentId] = 1;
                                else
                                    invertedIndex[term][documentId]++;
                            }
                        }
                    }
                }
                else
                {
                    continue;
                }
            }
            // Calculate document frequencies after processing all documents
            foreach (var termDocumentFreqPair in invertedIndex)
            {
                string term = termDocumentFreqPair.Key;
                int docFreq = termDocumentFreqPair.Value.Count;
                // Store document frequency for each term
                invertedIndex[term][-1] = docFreq;
            }
        }
        public static void SaveInvertedIndex(Dictionary<string, Dictionary<int, int>> invertedIndex, HashSet<int> allDocumentIds, string filePath)
        {
            // Create a new object to hold both the inverted index and allDocumentIds
            var invertedIndexData = new
            {
                InvertedIndex = invertedIndex,
                AllDocumentIds = allDocumentIds.ToList() // Convert HashSet to List for serialization
            };

            string json = JsonConvert.SerializeObject(invertedIndexData, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }
        public static (Dictionary<string, Dictionary<int, int>>, HashSet<int>) LoadInvertedIndex(string filePath)
        {
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                var invertedIndexData = JsonConvert.DeserializeObject<dynamic>(json);

                // Extract the inverted index and allDocumentIds
                var loadedInvertedIndex = invertedIndexData.InvertedIndex.ToObject<Dictionary<string, Dictionary<int, int>>>();
                var loadedAllDocumentIds = new HashSet<int>(invertedIndexData.AllDocumentIds.ToObject<List<int>>());

                return (loadedInvertedIndex, loadedAllDocumentIds);
            }

            return (new Dictionary<string, Dictionary<int, int>>(), new HashSet<int>());
        }
    }
}
