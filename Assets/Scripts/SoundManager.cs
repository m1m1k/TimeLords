using UnityEngine;
using System.Collections;

namespace TimeLords
{
	public class SoundManager : MonoBehaviour 
	{
        public static SoundManager Instance = null;

        public AudioSource efxSource;					//Drag a reference to the audio source which will play the sound effects.
		public AudioSource musicSource;					//Drag a reference to the audio source which will play the music.
		public float lowPitchRange = .95f;				//The lowest a sound effect will be randomly pitched.
		public float highPitchRange = 1.05f;			//The highest a sound effect will be randomly pitched.
		
		
		void Awake ()
        {
            Extensions.AssureSingletonAndDestroyExtras(ref Instance, this);
            //InsureOnlyOneInstance(ref Instance, this);

            //Set SoundManager to DontDestroyOnLoad so that it won't be destroyed when reloading our scene.
            DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// Works fine if in same class. mgr must be byref.
        /// </summary>
        public void InsureOnlyOneInstance(ref SoundManager mgr, MonoBehaviour obj)
        {
            //Check if instance already exists
            if (mgr == null)
            {
                //if not, set instance to this
                mgr = this;
            }
            //If instance already exists and it's not this:
            else if (mgr != this)
            {
                //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
                Destroy(obj);
            }
        }


        //Used to play single sound clips.
        public void PlaySingle(AudioClip clip)
		{
			//Set the clip of our efxSource audio source to the clip passed in as a parameter.
			efxSource.clip = clip;
			
			//Play the clip.
			efxSource.Play ();
		}
		
		
		//RandomizeSfx chooses randomly between various audio clips and slightly changes their pitch.
		public void RandomizeSfx (params AudioClip[] clips)
		{
			//Generate a random number between 0 and the length of our array of clips passed in.
			int randomIndex = Random.Range(0, clips.Length);
			
			//Choose a random pitch to play back our clip at between our high and low pitch ranges.
			float randomPitch = Random.Range(lowPitchRange, highPitchRange);
			
			//Set the pitch of the audio source to the randomly chosen pitch.
			efxSource.pitch = randomPitch;
			
			//Set the clip to the clip at our randomly chosen index.
			efxSource.clip = clips[randomIndex];
			
			//Play the clip.
			efxSource.Play();
		}
	}
}
