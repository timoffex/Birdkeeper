using UnityEngine;
using System.Collections;


/// <summary>
/// Will locate an AudioSource and allow the playing of sounds. Mainly
/// a utility for UI elements that can't be linked directly to an AudioSource.
/// </summary>
public class SoundPlayer : MonoBehaviour {

	private AudioSource audioSource;

	// Use this for initialization
	void Awake () {
		audioSource = FindObjectOfType<AudioSource> ();	

		if (audioSource == null)
			Debug.LogFormat ("No AudioSource found in scene. Sounds from {0} will not play.", name);
	}
	

	public void PlayOneShot (AudioClip clip) {
		if (audioSource != null)
			audioSource.PlayOneShot (clip);
	}
}
