using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioCenter : MonoBehaviour {
    public Transform dynamicObjects;
    public GameObject oneShotAudioPrefab;

    private Dictionary<AudioClip, LinkedList<AudioSource>> pool;

    public AudioCenter()
    {
        pool = new Dictionary<AudioClip, LinkedList<AudioSource>>();
    }

    public static void PlayRandomClipAtMainCamera(AudioClip[] clips)
    {
        
        if (clips == null || clips.Length == 0) return;
        AudioClip clip = clips[Random.Range(0, clips.Length - 1)];
        if (clip != null) Singleton<AudioCenter>.Instance.PlayOneShot(clip);
    }
    
    public static void PlayClipAtMainCamera(AudioClip clip)
    {
        Singleton<AudioCenter>.Instance.PlayOneShot(clip);
    }

    public void PlayOneShot(AudioClip clip)
    {
        if (clip == null) return;

        LinkedList<AudioSource> list = null;

        if (!pool.TryGetValue(clip, out list)) pool.Add(clip, list = new LinkedList<AudioSource>());

        AudioSource audioSource = null;

        if (list.First != null) for (LinkedListNode<AudioSource> node = list.First; node != list.Last.Next; node = node.Next)
        {
            if (!node.Value.isPlaying)
            {
                audioSource = node.Value;
                break;
            }
        }

        if (audioSource == null)
        {
            GameObject oneShotAudio = GameObject.Instantiate(oneShotAudioPrefab) as GameObject;

            oneShotAudio.transform.parent = dynamicObjects;

            audioSource = oneShotAudio.GetComponent<AudioSource>();
            audioSource.clip = clip;

            list.AddLast(audioSource);
        }

        audioSource.Play();
    }
}
