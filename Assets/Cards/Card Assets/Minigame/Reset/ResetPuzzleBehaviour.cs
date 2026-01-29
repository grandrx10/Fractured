using Cards;
using Cards.Core;
using Cards.Core.Behaviors;
using Cards.Core.BehaviorTags;
using Cards.Environments;
using UnityEngine;

namespace Cards.Card_Assets.Minigame.Reset
{
    [CreateAssetMenu(fileName = "ResetPuzzle", menuName = "Behaviors/Reset Puzzle")]
    public class ResetPuzzleBehavior : Behavior, IBehaviorUseListener
    {
        public float range;
        public bool Use(Card card, CardEnv env, Agent agent)
        {
            if (env is not OpenWorldEnv)
            {
                Debug.LogWarning("ResetPuzzleBehavior used outside OpenWorldEnv.");
                return false;
            }

            // Find all puzzle managers in the scene
            PuzzleManager[] puzzles = Object.FindObjectsOfType<PuzzleManager>();

            if (puzzles.Length == 0)
            {
                Debug.LogWarning("No PuzzleManager instances found in scene.");
                return false;
            }

            foreach (PuzzleManager puzzle in puzzles)
            {
                if (Vector3.Distance(agent.transform.position, puzzle.transform.position) <= range) puzzle.ResetPuzzle();
            }

            Debug.Log($"Reset {puzzles.Length} puzzle(s).");
            return true;
        }

        public override string GetDescription(Card card)
        {
            return "<b>(Active)</b> Reset all puzzles in the scene.";
        }
    }
}
