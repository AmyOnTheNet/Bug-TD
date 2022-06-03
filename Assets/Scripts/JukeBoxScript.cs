using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Michsky.UI.ModernUIPack;

public class JukeBoxScript : MonoBehaviour
{
	public AudioClip[] MusicLibrary;

	public GameObject Notification;

	private AudioClip currentClip;
	private AudioSource _AudioSource;
	private int lastPlayedIndex;

	void Awake(){
		GameObject[] objs = GameObject.FindGameObjectsWithTag("Jukebox");

		if (objs.Length > 1)
		{
			Destroy(this.gameObject);
		}
		
		DontDestroyOnLoad(this.gameObject);
	}

	void Start()
	{
		_AudioSource = GetComponent<AudioSource>();

		PlayClipNotify(MusicLibrary[0]);
	}

	// Update is called once per frame
	private float nextSongTimer;

	void Update()
	{
		if (PlayerPrefs.GetInt("MusicEnabled") == 0){
			_AudioSource.mute = true;
			return;
		}else{
			_AudioSource.mute = false;
		}

		nextSongTimer -= Time.deltaTime;

		if (nextSongTimer <= 0) 
		{
			PlayRandom();
		}
	}

	public void PlayClipNotify(AudioClip _clipToPlay){

		if (PlayerPrefs.GetInt("MusicEnabled", 1) == 0)
			return;

		NotificationManager _manager = Notification.GetComponent<NotificationManager>();

		_manager.description = _clipToPlay.name;

		_AudioSource.clip = _clipToPlay;

		nextSongTimer = _clipToPlay.length;

		_AudioSource.Play();

		_manager.UpdateUI();
		_manager.OpenNotification();
	}

	public void PlayRandom(){
		int _i = GetNewRandomIndex();
		lastPlayedIndex = _i;
		PlayClipNotify(MusicLibrary[_i]);
	}

	private int GetNewRandomIndex(){
		int _index = Random.Range(0,MusicLibrary.Length);

		if (_index == lastPlayedIndex) 
		{
			return GetNewRandomIndex();
		}else {
			return _index;
		}
	}
}
