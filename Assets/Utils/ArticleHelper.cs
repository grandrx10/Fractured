namespace Utils
{
    public static class ArticleHelper
    {
        /// <summary>
        /// Returns "a {word}" or "an {word}" depending on pronunciation rules.
        /// This covers the majority of English cases.
        /// </summary>
        public static string WithIndefiniteArticle(string noun)
        {
            if (string.IsNullOrWhiteSpace(noun))
                return noun;

            noun = noun.Trim();

            // Words beginning with a vowel sound
            // (simple rule: starts with a vowel letter)
            char first = char.ToLower(noun[0]);
            bool vowelStart = first == 'a' || first == 'e' || first == 'i' || first == 'o' || first == 'u';

            // Edge cases (honest → 'an', unicorn → 'a', etc.)
            // Add exceptions as needed for your game.
            if (StartsWithSilentH(noun))
                return "an " + noun;

            if (StartsWithYooSound(noun))
                return "a " + noun;

            return (vowelStart ? "an " : "a ") + noun;
        }

        private static bool StartsWithSilentH(string word)
        {
            // "honest", "hour", "honor", etc.
            string[] silentH = { "honest", "honor", "hour", "heir" };
            foreach (var s in silentH)
                if (word.StartsWith(s, System.StringComparison.OrdinalIgnoreCase))
                    return true;
            return false;
        }

        private static bool StartsWithYooSound(string word)
        {
            // "unicorn", "university", "unit", "user", "europe", etc.
            string[] yoo = { "uni", "use", "user", "eur", "uro", "ufo" };
            foreach (var s in yoo)
                if (word.StartsWith(s, System.StringComparison.OrdinalIgnoreCase))
                    return true;
            return false;
        }
    }
}