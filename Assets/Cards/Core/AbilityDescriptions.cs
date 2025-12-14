using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Cards.Core
{
    public static class AbilityDescriptions
    {
        private static Dictionary<string, string> _descriptions;

        private static void LoadDescriptions()
        {
            if (_descriptions != null) return; // already loaded

            _descriptions = new Dictionary<string, string>();

            // Load TextAsset from Resources folder
            TextAsset textAsset = Resources.Load<TextAsset>("Abilities"); // Abilities.txt in Resources/
            if (textAsset == null)
            {
                Debug.LogError("Abilities.txt not found in Resources folder!");
                return;
            }

            string[] lines = textAsset.text.Split(new[] { '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries);

            string currentName = null;
            List<string> currentDescLines = new List<string>();

            foreach (string rawLine in lines)
            {
                string line = rawLine.Trim();

                if (line.StartsWith("[") && line.EndsWith("]"))
                {
                    // Save previous ability
                    if (currentName != null)
                    {
                        _descriptions[currentName] = string.Join("\n", currentDescLines);
                    }

                    // Start new ability
                    currentName = line.Substring(1, line.Length - 2).Trim();
                    currentDescLines.Clear();
                }
                else
                {
                    if (currentName != null)
                        currentDescLines.Add(line);
                }
            }

            // Save last ability
            if (currentName != null && currentDescLines.Count > 0)
            {
                _descriptions[currentName] = string.Join("\n", currentDescLines);
            }
        }

        public static string GetDescription(string ability)
        {
            LoadDescriptions();

            if (_descriptions != null && _descriptions.TryGetValue(ability, out string desc))
            {
                return $"[{ability}]\n{desc}";
            }

            return "nothin";
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