using System.Collections.Generic;
using Cards.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Cards.Visual
{
    public class CardDisplayPrefabQuests : CardDisplayPrefab
    {
        [Tooltip("Maximum number of lines per page")]
        public int maxLinesPerPage = 6;

        private List<string> _pages = new();
        private int _currentPage = 0;
        public TextMeshProUGUI pageCounter;
        public override void SetDesc(string desc)
        {
            _pages.Clear();
            _currentPage = 0;

            // Split ONLY on explicit newlines
            string[] logicalLines = desc.Split('\n');

            var currentLines = new List<string>();

            foreach (var line in logicalLines)
            {
                currentLines.Add(line);

                string testText = string.Join("\n", currentLines);
                description.text = testText;
                description.ForceMeshUpdate();

                if (description.textInfo.lineCount > maxLinesPerPage)
                {
                    // Remove last line and finalize previous page
                    currentLines.RemoveAt(currentLines.Count - 1);

                    _pages.Add(string.Join("\n", currentLines));

                    // Start new page with the line that didn't fit
                    currentLines.Clear();
                    currentLines.Add(line);
                }
            }

            // Add final page
            if (currentLines.Count > 0)
                _pages.Add(string.Join("\n", currentLines));

            ApplyPage();
        }

        public void NextPage()
        {
            if (_pages.Count == 0)
                return;

            if (_currentPage < _pages.Count - 1)
            {
                _currentPage++;
                ApplyPage();
            }
        }

        public void PrevPage()
        {
            if (_pages.Count == 0)
                return;

            if (_currentPage > 0)
            {
                _currentPage--;
                ApplyPage();
            }
        }
        
        private void ApplyPage()
        {
            description.text = _pages[_currentPage];

            if (pageCounter != null)
                pageCounter.text = $"{_currentPage + 1} / {_pages.Count}";
        }
    }
}