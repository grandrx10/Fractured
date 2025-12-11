using System.Text.RegularExpressions;
using UnityEngine;

namespace Cards.Visual
{
    public static class AbilityDescriptions
    {
        public static string GetDescription(string ability)
        {
            switch (ability)
            {
                case "Cut":
                    return "Cut: Strikes for 100% damage, interacts with cuttable objects.";
                default:
                    return "nothin";
            }
        }
        
        // Regex: capture [link] (text)
        // - group "link" => content inside []
        // - group "text" => content inside ()
        private static readonly Regex LinkRegex = new Regex(@"\[(?<link>[^\]]+)\]\((?<text>[^)]+)\)",
            RegexOptions.Compiled);

        /// <summary>
        /// Convert occurrences of [link](text) to Unity / TextMeshPro style:
        ///     &lt;link=link&gt;&lt;i&gt;text&lt;/i&gt;&lt;/link&gt;
        /// </summary>
        public static string ConvertBracketLinks(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;

            string result = LinkRegex.Replace(input, m =>
            {
                string link = m.Groups["link"].Value.Trim();
                string text = m.Groups["text"].Value.Trim();

                // Optional: escape '>' or other characters if needed for your use-case.
                // For TextMeshPro/Unity rich text this is usually fine, but if you expect '>' in link
                // you might sanitize here.

                return $"<link={link}><i>{text}</i></link>";
            });

            return result;
        }
    }
}