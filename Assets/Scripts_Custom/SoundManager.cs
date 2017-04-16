using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

    public AudioSource efxSource;
    public AudioSource musicSource;
    public static SoundManager instance = null;

    /// <summary>
    /// Adds random variation to our sound effects
    /// Plus minus 5% of pitch should be noticeable but not distracting.
    /// </summary>
    public float lowPitchRange = .95f;
    public float highPitchRange = 1.05f;

    // Use this for initialization
    void Awake () {
        GameManager.AssureSingletonAndDestroyExtras(instance, this);
        DontDestroyOnLoad(gameObject);
    }
	
	// Update is called once per frame
	public void PlaySingle (AudioClip clip) {
        efxSource.clip = clip;
        efxSource.Play();
	}

    public void RandomizeSfx(params AudioClip[] clips)
    {
        // choose a random clip to play
        int randomIndex = Random.Range(0, clips.Length);
        float randomPitch = Random.Range(lowPitchRange, highPitchRange);

        efxSource.pitch = randomPitch;
        efxSource.clip = clips[randomIndex];
        efxSource.Play();
    }
}
