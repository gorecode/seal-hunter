using UnityEngine;
using System.Collections;

public class AudioClips {
    public static void PlayRandomClipAtMainCamera(AudioClip[] clips)
    {
        if (clips == null || clips.Length == 0) return;
        AudioClip clip = clips[Random.Range(0, clips.Length - 1)];
        if (clip != null) AudioSource.PlayClipAtPoint(clip, Camera.main.transform.position);
    }

    public static void PlayClipAtMainCamera(AudioClip clip)
    {
        if (clip != null) AudioSource.PlayClipAtPoint(clip, Camera.main.transform.position);
    }

    private AudioClips() { }
}
