using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Global Settings")]
    [Range(0f, 1f)]
    public float masterVolume = 1f;

    [Header("Pitch Variation")]
    public float minPitch = 0.95f;
    public float maxPitch = 1.05f;

    [Header("Pooling")]
    [SerializeField] private int poolSize = 10;

    private Queue<AudioSource> audioSourcePool = new Queue<AudioSource>();
    private List<AudioSource> activeAudioSources = new List<AudioSource>();

    private Dictionary<AudioSource, Transform> followTargets = new Dictionary<AudioSource, Transform>();
    private HashSet<AudioSource> _justStarted = new HashSet<AudioSource>();

    [Header("Soundtrack")]
    [SerializeField] private AudioSource soundtrackSource;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        GameObject soundtrackObj = new GameObject("SoundtrackAudioSource");
        soundtrackObj.transform.SetParent(transform);
        soundtrackSource = soundtrackObj.AddComponent<AudioSource>();
        soundtrackSource.playOnAwake = false;
        soundtrackSource.loop = true;
        soundtrackSource.spatialBlend = 0f;

        for (int i = 0; i < poolSize; i++)
            CreateAudioSource();
    }

    private void Update()
    {
        foreach (var kvp in followTargets)
        {
            if (kvp.Key != null && kvp.Value != null)
                kvp.Key.transform.position = kvp.Value.position;
        }

        for (int i = activeAudioSources.Count - 1; i >= 0; i--)
        {
            AudioSource src = activeAudioSources[i];

            if (_justStarted.Contains(src))
                continue;

            if (!src.isPlaying)
            {
                ReturnToPool(src);
                activeAudioSources.RemoveAt(i);
            }
        }

        _justStarted.Clear();
    }

    private AudioSource CreateAudioSource()
    {
        GameObject obj = new GameObject("PooledAudioSource");
        obj.transform.SetParent(transform);
        AudioSource source = obj.AddComponent<AudioSource>();
        source.playOnAwake = false;
        obj.SetActive(false);
        audioSourcePool.Enqueue(source);
        return source;
    }

    private AudioSource GetAudioSource()
    {
        if (audioSourcePool.Count == 0)
            return CreateAudioSource();

        AudioSource source = audioSourcePool.Dequeue();
        source.gameObject.SetActive(true);
        return source;
    }

    private void ReturnToPool(AudioSource source)
    {
        source.Stop();
        source.clip = null;
        source.loop = false;
        source.spatialBlend = 1f;

        followTargets.Remove(source);

        source.gameObject.SetActive(false);
        audioSourcePool.Enqueue(source);
    }

    public void PlayOneShot(
        AudioClip clip,
        Vector3 position,
        float volume = 1f,
        bool randomizePitch = true,
        float spatialBlend = 1f
    )
    {
        if (clip == null) return;

        AudioSource source = GetAudioSource();
        source.transform.position = position;
        source.clip = clip;
        source.volume = volume * masterVolume;
        source.pitch = randomizePitch ? Random.Range(minPitch, maxPitch) : 1f;
        source.spatialBlend = spatialBlend;
        source.loop = false;
        source.Play();

        _justStarted.Add(source);
        activeAudioSources.Add(source);
    }

    public AudioSource PlayLooping(
        AudioClip clip,
        Vector3 position,
        float volume = 1f,
        bool randomizePitch = true,
        float spatialBlend = 1f
    )
    {
        if (clip == null) return null;

        AudioSource source = GetAudioSource();
        source.transform.position = position;
        source.clip = clip;
        source.volume = volume * masterVolume;
        source.pitch = randomizePitch ? Random.Range(minPitch, maxPitch) : 1f;
        source.spatialBlend = spatialBlend;
        source.loop = true;
        source.Play();

        _justStarted.Add(source);
        activeAudioSources.Add(source);
        return source;
    }

    public void PlayFollowingOneShot(
        AudioClip clip,
        Transform followTarget,
        float volume = 1f,
        bool randomizePitch = true,
        float spatialBlend = 1f
    )
    {
        if (clip == null || followTarget == null) return;

        AudioSource source = GetAudioSource();
        source.clip = clip;
        source.volume = volume * masterVolume;
        source.pitch = randomizePitch ? Random.Range(minPitch, maxPitch) : 1f;
        source.spatialBlend = spatialBlend;
        source.loop = false;

        followTargets[source] = followTarget;
        source.transform.position = followTarget.position;
        source.Play();

        _justStarted.Add(source);
        activeAudioSources.Add(source);
    }

    public AudioSource PlayFollowingLoop(
        AudioClip clip,
        Transform followTarget,
        float volume = 1f,
        bool randomizePitch = true,
        float spatialBlend = 1f
    )
    {
        if (clip == null || followTarget == null) return null;

        AudioSource source = GetAudioSource();
        source.clip = clip;
        source.volume = volume * masterVolume;
        source.pitch = randomizePitch ? Random.Range(minPitch, maxPitch) : 1f;
        source.spatialBlend = spatialBlend;
        source.loop = true;

        followTargets[source] = followTarget;
        source.transform.position = followTarget.position;
        source.Play();

        _justStarted.Add(source);
        activeAudioSources.Add(source);
        return source;
    }

    public void StopLooping(AudioSource source)
    {
        if (source == null) return;

        if (activeAudioSources.Contains(source))
            activeAudioSources.Remove(source);

        _justStarted.Remove(source);
        ReturnToPool(source);
    }

    public void StopAllSounds()
    {
        foreach (var source in activeAudioSources)
            ReturnToPool(source);

        activeAudioSources.Clear();
        _justStarted.Clear();
    }

    public AudioSource PlaySoundtrack(
        AudioClip clip,
        float volume = 1f,
        bool loop = true,
        float pitch = 1f
    )
    {
        if (clip == null) return null;

        // Create a new GameObject in the scene
        GameObject obj = new GameObject("SceneSoundtrack");
        obj.transform.position = Vector3.zero;

        // Add an AudioSource to it
        AudioSource source = obj.AddComponent<AudioSource>();
        source.clip = clip;
        source.volume = volume * masterVolume;
        source.pitch = pitch;
        source.loop = loop;
        source.spatialBlend = 0f; // 2D
        source.playOnAwake = false;
        source.Play();

        return source;
    }

    public void StopSoundtrack()
    {
        soundtrackSource.Stop();
        soundtrackSource.clip = null;
    }

    public void PauseSoundtrack()
    {
        if (soundtrackSource.isPlaying)
            soundtrackSource.Pause();
    }

    public void ResumeSoundtrack()
    {
        if (!soundtrackSource.isPlaying && soundtrackSource.clip != null)
            soundtrackSource.Play();
    }
}
