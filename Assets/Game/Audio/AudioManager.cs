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

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        // Pre-create audio source pool
        for (int i = 0; i < poolSize; i++)
        {
            CreateAudioSource();
        }
    }

    private void Update()
    {
        // Clean up finished audio sources
        for (int i = activeAudioSources.Count - 1; i >= 0; i--)
        {
            if (!activeAudioSources[i].isPlaying)
            {
                ReturnToPool(activeAudioSources[i]);
                activeAudioSources.RemoveAt(i);
            }
        }
    }

    private AudioSource CreateAudioSource()
    {
        GameObject obj = new GameObject("PooledAudioSource");
        obj.transform.SetParent(transform);
        AudioSource source = obj.AddComponent<AudioSource>();
        source.playOnAwake = false;
        source.spatialBlend = 1f; // 3D sound
        obj.SetActive(false);
        audioSourcePool.Enqueue(source);
        return source;
    }

    private AudioSource GetAudioSource()
    {
        if (audioSourcePool.Count == 0)
        {
            return CreateAudioSource();
        }
        
        AudioSource source = audioSourcePool.Dequeue();
        source.gameObject.SetActive(true);
        return source;
    }

    private void ReturnToPool(AudioSource source)
    {
        source.Stop();
        source.clip = null;
        source.gameObject.SetActive(false);
        audioSourcePool.Enqueue(source);
    }

    public void PlayOneShot(AudioClip clip, Vector3 position, float volume = 1f, bool randomizePitch = true)
    {
        if (clip == null) return;

        AudioSource source = GetAudioSource();
        source.transform.position = position;
        source.clip = clip;
        source.volume = volume * masterVolume;
        source.pitch = randomizePitch ? Random.Range(minPitch, maxPitch) : 1f;
        source.Play();

        activeAudioSources.Add(source);
    }

    /// <summary>
    /// Plays a looping AudioClip at a position and returns the AudioSource so it can be stopped manually.
    /// </summary>
    public AudioSource PlayLooping(AudioClip clip, Vector3 position, float volume = 1f, bool randomizePitch = true)
    {
        if (clip == null) return null;

        AudioSource source = GetAudioSource();
        source.transform.position = position;
        source.clip = clip;
        source.volume = volume * masterVolume;
        source.pitch = randomizePitch ? Random.Range(minPitch, maxPitch) : 1f;
        source.loop = true;
        source.Play();

        activeAudioSources.Add(source);
        return source;
    }

    /// <summary>
    /// Stops a looping sound and returns it to the pool.
    /// </summary>
    public void StopLooping(AudioSource source)
    {
        if (source == null) return;

        if (activeAudioSources.Contains(source))
        {
            activeAudioSources.Remove(source);
        }

        source.Stop();
        source.loop = false;
        ReturnToPool(source);
    }


    public void StopAllSounds()
    {
        foreach (var source in activeAudioSources)
        {
            ReturnToPool(source);
        }
        activeAudioSources.Clear();
    }
}