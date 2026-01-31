using UnityEngine;

public class PlayOnSpawn : MonoBehaviour
{
    public enum AudioMode
    {
        PlayAtSpawn,
        FollowObjectLoop,
        FollowObjectOneShot,
        Soundtrack   // NEW: plays on the AudioManager soundtrack
    }

    [Header("Audio")]
    public AudioClip clip;
    public AudioMode mode = AudioMode.PlayAtSpawn;
    [Range(0f, 1f)] public float volume = 1f;
    public bool randomizePitch = true;

    [Header("Spatial Blend")]
    [Range(0f, 1f)]
    public float spatialBlend = 1f;

    [Header("Playback Timing")]
    public bool playWhenFirstEnabled = false;

    private AudioSource activeSource;
    private bool hasPlayed = false;

    void Start()
    {
        if (!playWhenFirstEnabled)
            TryPlay();
    }

    void OnEnable()
    {
        if (playWhenFirstEnabled && !hasPlayed)
            TryPlay();
    }

    private void TryPlay()
    {
        if (hasPlayed || clip == null)
            return;

        if (AudioManager.Instance == null)
        {
            // World not ready yet — try again next frame
            StartCoroutine(WaitForAudioManager());
            return;
        }

        hasPlayed = true;

        switch (mode)
        {
            case AudioMode.PlayAtSpawn:
                AudioManager.Instance.PlayOneShot(
                    clip,
                    transform.position,
                    volume,
                    randomizePitch,
                    spatialBlend
                );
                break;

            case AudioMode.FollowObjectOneShot:
                AudioManager.Instance.PlayFollowingOneShot(
                    clip,
                    transform,
                    volume,
                    randomizePitch,
                    spatialBlend
                );
                break;

            case AudioMode.FollowObjectLoop:
                activeSource = AudioManager.Instance.PlayFollowingLoop(
                    clip,
                    transform,
                    volume,
                    randomizePitch,
                    spatialBlend
                );
                break;

            case AudioMode.Soundtrack:
                // Pitch is supported, spatialBlend is ignored for soundtrack
                AudioManager.Instance.PlaySoundtrack(
                    clip,
                    volume,
                    loop: true,
                    pitch: randomizePitch ? Random.Range(AudioManager.Instance.minPitch, AudioManager.Instance.maxPitch) : 1f
                );
                break;
        }
    }

    private System.Collections.IEnumerator WaitForAudioManager()
    {
        yield return null;

        while (AudioManager.Instance == null)
            yield return null;

        TryPlay();
    }

    void OnDestroy()
    {
        if (activeSource != null && AudioManager.Instance != null)
        {
            AudioManager.Instance.StopLooping(activeSource);
        }
    }
}
