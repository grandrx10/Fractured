using UnityEngine;
using Game.Bosses;

[CreateAssetMenu(fileName = "SoundtrackAttack", menuName = "BossAttacks/Common/SoundtrackAttack")]
public class SoundtrackAttack : BossAttack
{
    [Header("Soundtrack Settings")]
    public AudioClip soundtrack;   // The clip to play
    public float volume = 1f;
    public bool loop = true;
    public float pitch = 1f;

    public override void StartAttack(GameObject boss)
    {
        base.StartAttack(boss);

        // Play the soundtrack
        if (soundtrack != null)
        {
            Debug.Log("PLAYED SOUND!");
            AudioManager.Instance.PlaySoundtrack(soundtrack, volume, loop, pitch);
        }
    }
}
