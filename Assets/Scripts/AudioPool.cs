using UnityEngine;
using System.Collections.Generic;
using System;

public class AudioPool : MonoBehaviour
{
    [Serializable]
    public class ReservedAudioClip
    {
        public string name;
        public AudioClip clip;
    }
    
    public List<ReservedAudioClip> pool = new List<ReservedAudioClip>();

    public void GetAudioClip(string id)
    {
        foreach (ReservedAudioClip clip in pool)
        {
            if (clip.name.Equals(id))
            {
                AudioManager.instance.PlaySFX(clip.clip);
            }
        }
    }
}
