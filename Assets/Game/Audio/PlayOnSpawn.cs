using UnityEngine;

public class PlayOnSpawn : MonoBehaviour
{
    public enum AudioMode
    {
        PlayAtSpawn,
        FollowObject
    }

    [Header("Audio")]
    public AudioClip clip;
    public AudioMode mode = AudioMode.PlayAtSpawn;
    [Range(0f, 1f)] public float volume = 1f;
    public bool randomizePitch = true;

    [Header("Spatial Blend")]
    [Range(0f, 1f)]
    public float spatialBlend = 1f; // 0 = 2D, 1 = 3D

    [Header("Playback Timing")]
    public bool playWhenFirstEnabled = false;

    private AudioSource activeSource;
    private bool hasPlayed = false;

    void Start()
    {
        // If we're NOT using playWhenFirstEnabled, play immediately on spawn
        if (!playWhenFirstEnabled)
        {
            TryPlay();
        }
    }

    void OnEnable()
    {
        // Play only the FIRST time the object becomes enabled
        if (playWhenFirstEnabled && !hasPlayed)
        {
            TryPlay();
        }
    }

    private void TryPlay()
    {
        if (hasPlayed || AudioManager.Instance == null || clip == null)
            return;

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

            case AudioMode.FollowObject:
                activeSource = AudioManager.Instance.PlayLooping(
                    clip,
                    transform.position,
                    volume,
                    randomizePitch,
                    spatialBlend
                );
                break;
        }
    }

    void Update()
    {
        if (activeSource != null)
        {
            activeSource.transform.position = transform.position;
        }
    }

    void OnDestroy()
    {
        if (activeSource != null && AudioManager.Instance != null)
        {
            AudioManager.Instance.StopLooping(activeSource);
        }
    }
}
