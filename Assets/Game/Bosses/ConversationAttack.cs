using UnityEngine;
using Game.Bosses;
using Characters.Dialogue;

namespace Game.Bosses
{
    [CreateAssetMenu(menuName = "BossAttacks/Common/DialogueAttack")]
    public class DialogueAttack : BossAttack
    {
        [Header("Dialogue")]
        public string conversationName;

        private Boss cachedBoss;

        public override void StartAttack(GameObject boss)
        {
            base.StartAttack(boss);

            manualSkip = true; // REQUIRED

            cachedBoss = boss.GetComponent<Boss>();

            if (DialogueManager.Instance == null)
            {
                Debug.LogError("DialogueAttack: DialogueManager not found");
                EndSelf();
                return;
            }

            DialogueManager.Instance.OnConversationEnded += OnDialogueFinished;
            DialogueManager.Instance.StartConversation(conversationName);
        }

        private void OnDialogueFinished()
        {
            DialogueManager.Instance.OnConversationEnded -= OnDialogueFinished;
            EndSelf();
        }

        private void EndSelf()
        {
            if (cachedBoss != null)
            {
                cachedBoss.EndCurrentAttack();
            }
        }

        public override void EndAttack(GameObject boss)
        {
            base.EndAttack(boss);

            if (DialogueManager.Instance != null)
                DialogueManager.Instance.OnConversationEnded -= OnDialogueFinished;
        }
    }
}
