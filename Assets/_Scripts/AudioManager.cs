using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UIElements;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] AudioMixer audioMixer;

    AudioSource audioSourcePrefab;
    static Dictionary<string, AudioSource> activeSources;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        InitializeSources();
    }

    void InitializeSources()
    {
        GameObject sourceGO = new GameObject($"Audio Source Base");
        sourceGO.transform.SetParent(transform);
        audioSourcePrefab = sourceGO.AddComponent<AudioSource>();

        audioSourcePrefab.loop = false;
        audioSourcePrefab.playOnAwake = false;
        audioSourcePrefab.spatialBlend = 1f;                       // 3D sound
        audioSourcePrefab.rolloffMode = AudioRolloffMode.Linear;
        audioSourcePrefab.minDistance = 1f;                        // Default min distance
        audioSourcePrefab.maxDistance = 20f;                       // Default max distance
        audioSourcePrefab.dopplerLevel = 0f;                       // Disable doppler effect by default

        activeSources = new ();
    }

    bool IsFreeSource(AudioSource source)
    {
        return !source.isPlaying;
    }

    // For global audio like music and global sfx
    public string PlayClip2D(AudioPlayable a, string id)
    {
        return PlayClip(a.audioResource, id, Vector2.zero, 0, 0, 1, a.MixerGroupName);
    }

    // For local audio like most sfx played at specific point
    public string PlayClip3D(AudioPlayable a, string id)
    {
        return PlayClip(a.audioResource, id, a.Position, 1, a.MinDistance, a.MaxDistance, a.MixerGroupName);
    }

    string PlayClip(AudioResource audioResource, string id, Vector3 position, float spatialBlend, float minDistance, float maxDistance,
        string mixerGroupName = null)
    {
        AudioSource source = ObjectPoolManager.SpawnObject(audioSourcePrefab, position, Quaternion.identity, ObjectPoolManager.AUDIO_POOL_ID);

        ConfigureSource(source, audioResource, position, spatialBlend, minDistance, maxDistance, GetMixerGroupByName(mixerGroupName));

        string sourceID = id;
        if (activeSources.ContainsKey(id))
        sourceID = id + Time.time;
        activeSources[sourceID] = source;

        source.Play();
        return sourceID;
    }

    private AudioMixerGroup GetMixerGroupByName(string mixerGroupName)
    {
        if (audioMixer == null)
        {
            Debug.LogError("AudioMixer is not assigned in the AudioManager.");
            return null;
        }

        if (!string.IsNullOrEmpty(mixerGroupName))
        {
            AudioMixerGroup[] groups = audioMixer.FindMatchingGroups(mixerGroupName);
            if (groups != null && groups.Length > 0)
            {
                return groups[0]; // Return the first matching group
            }
            else
            {
                Debug.LogWarning($"Mixer group '{mixerGroupName}' not found in the assigned AudioMixer.");
            }
        }

        return null; // No mixer group specified or found
    }

    private void ConfigureSource(AudioSource source, AudioResource audioResource, Vector3 position, float spatialBlend, float minDistance, float maxDistance, AudioMixerGroup mixerGroup)
    {
        source.transform.position = position;
        source.spatialBlend = spatialBlend;

        source.resource = audioResource;
        source.minDistance = minDistance;
        source.maxDistance = maxDistance;
        source.outputAudioMixerGroup = mixerGroup;
    }

    // Pause a specific sound
    public void PauseSound(string sourceID)
    {
        if (activeSources.TryGetValue(sourceID, out AudioSource source))
        {
            source.Pause();
        }
    }

    // Resume a paused sound
    public void ResumeSound(string sourceID)
    {
        if (activeSources.TryGetValue(sourceID, out AudioSource source))
        {
            source.UnPause();
        }
    }

    // Stop a specific sound
    public void StopSound(string sourceID, bool stopAll)
    {
        if (stopAll)
        {
            foreach (string id in activeSources.Keys.Where(x => x.StartsWith(sourceID)).ToArray())
            {
                activeSources[id].Stop();
                ObjectPoolManager.ReturnObjectToPool(activeSources[id].gameObject, ObjectPoolManager.AUDIO_POOL_ID);
                activeSources.Remove(id);
            }
        }
        else
        {
            if (activeSources.TryGetValue(sourceID, out AudioSource source))
            {
                source.Stop();
                ObjectPoolManager.ReturnObjectToPool(source.gameObject, ObjectPoolManager.AUDIO_POOL_ID);
                activeSources.Remove(sourceID);
            }
        }


    }

    // Clean up finished sources
    private void FixedUpdate()
    {
        List<string> toRemove = new List<string>();
        foreach (var kvp in activeSources)
        {
            if (IsFreeSource(kvp.Value))
                toRemove.Add(kvp.Key);
        }

        foreach (string id in toRemove)
        {
            if(activeSources.ContainsKey(id))
                StopSound(id, false);
        }
    }
}

[Serializable]
public struct AudioPlayable
{
    public AudioResource audioResource;
    public Vector3 Position;
    public float SpacialBlend;
    public float MinDistance;
    public float MaxDistance;
    public string MixerGroupName;

    public AudioPlayable(AudioClip audioResource, string mixerGroupName) : this()
    {
        this.audioResource = audioResource;
        MixerGroupName = mixerGroupName;
    }

    public AudioPlayable(AudioClip audioResource, Vector3 position, float spacialBlend, float minDistance, float maxDistance, string mixerGroupName)
    {
        this.audioResource = audioResource;
        Position = position;
        SpacialBlend = spacialBlend;
        MinDistance = minDistance;
        MaxDistance = maxDistance;
        MixerGroupName = mixerGroupName;
    }
}