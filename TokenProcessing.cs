using System.Text.RegularExpressions;
namespace K213961
{
    internal class TokenProcessing
    {
        public static string[] Tokenize(string content)
        {
            return Regex.Split(content, @"\W+")
                        .Where(word => !string.IsNullOrWhiteSpace(word) && !word.StartsWith("_") && !word.EndsWith("_"))
                        .Select(word => word.ToLower())
                        .ToArray();
        }
        public static bool RemoveSuffix(ref string token, string suffix)
        {
            int exists = token.LastIndexOf(suffix);
            if (exists != -1 && exists == token.Length - suffix.Length)
            {
                if (token != suffix)
                {
                    token = token.Substring(0, exists);
                    return true;
                }
            }
            return false;
        }
        // Porter Stemmer :')
        public static void StemmatizeToken(ref string token, List<string> prefixes, List<char> vowels)
        {
            // Suffix Trim 1
            if (!RemoveSuffix(ref token, "sses") && RemoveSuffix(ref token, "ies"))
            {
                RemoveSuffix(ref token, "es");
            }
            else if (token.Length > 1 && token[token.Length - 2] != 's')
            {
                RemoveSuffix(ref token, "s");
            }

            if (RemoveSuffix(ref token, "eed"))
            {
                if (token.Length > 1 && IsVowel(token[token.Length - 2], vowels))
                {
                    RemoveSuffix(ref token, "ed");
                }
            }

            if (token.Length != 1 && token[token.Length - 1] == 'y')
            {
                if (token.Length > 1 && IsVowel(token[token.Length - 2], vowels))
                {
                    token = token.Substring(0, token.Length - 1) + 'i';
                }
            }

            if (token.Length > 4 && !IsVowel(token[token.Length - 4], vowels))
            {
                RemoveSuffix(ref token, "ing");
            }

            if (RemoveSuffix(ref token, "ied"))
            {
                if (token.Length > 1 && IsVowel(token[token.Length - 2], vowels))
                {
                    RemoveSuffix(ref token, "es");
                }
            }

            // Handle consonant doubling before adding suffix
            if (token.Length > 2 && token.EndsWith("ing") && IsConsonant(token[token.Length - 3], vowels))
            {
                token = token.Substring(0, token.Length - 1);
            }

            // Suffix Trim 2
            RemoveSuffix(ref token, "tional");
            RemoveSuffix(ref token, "ization");
            RemoveSuffix(ref token, "fulness");
            RemoveSuffix(ref token, "iveness");
            RemoveSuffix(ref token, "ousness");
            RemoveSuffix(ref token, "ousli");
            RemoveSuffix(ref token, "entli");
            RemoveSuffix(ref token, "biliti");

            // Suffix Trim 3
            RemoveSuffix(ref token, "icate");
            RemoveSuffix(ref token, "ative");
            RemoveSuffix(ref token, "alize");
            RemoveSuffix(ref token, "iciti");
            RemoveSuffix(ref token, "ical");
            RemoveSuffix(ref token, "ful");
            RemoveSuffix(ref token, "iveness");
            RemoveSuffix(ref token, "ness");

            // Suffix Trim 4
            RemoveSuffix(ref token, "al");
            RemoveSuffix(ref token, "ance");
            RemoveSuffix(ref token, "ence");
            RemoveSuffix(ref token, "er");
            RemoveSuffix(ref token, "ic");
            RemoveSuffix(ref token, "ed");
            RemoveSuffix(ref token, "able");
            RemoveSuffix(ref token, "ible");
            RemoveSuffix(ref token, "ant");
            RemoveSuffix(ref token, "ement");
            RemoveSuffix(ref token, "ment");
            RemoveSuffix(ref token, "ent");
            RemoveSuffix(ref token, "sion");
            RemoveSuffix(ref token, "tion");
            RemoveSuffix(ref token, "ou");
            RemoveSuffix(ref token, "ism");
            if (token.Length > 4)
            {
                RemoveSuffix(ref token, "ate");
            }
            RemoveSuffix(ref token, "iti");
            RemoveSuffix(ref token, "ous");
            RemoveSuffix(ref token, "ive");
            RemoveSuffix(ref token, "ize");
            RemoveSuffix(ref token, "ise");

            // Suffix Trim 5
            if (token.Length != 1 && token[token.Length - 1] == 'e')
            {
                if (!IsVowel(token[token.Length - 2], vowels))
                {
                    RemoveSuffix(ref token, "e");
                }
            }
        }
        private static bool IsConsonant(char character, List<char> vowels)
        {
            return !IsVowel(character, vowels);
        }
        private static bool IsVowel(char character, List<char> vowels)
        {
            return vowels.Contains(character);
        }
    }
}
