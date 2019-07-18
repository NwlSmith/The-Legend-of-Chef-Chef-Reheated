using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Audio;

public class VolumeIndicator : Indicator {

    public static VolumeIndicator instance = null;

    public AudioMixer am;
    public float cur_volume;

    protected override void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
        base.Awake();
    }

    protected override void UpdateValue()
    {
        AudioListener.volume = cur_volume = cur_index / 10f;
    }
}
