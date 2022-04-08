using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Music Site(s):
// https://pernyblom.github.io/abundant-music/index.html

public class Jukebox : MonoBehaviour
{
    public PlayType musicSelectionStyle;
    public List<BGSong> backgroundMusic;
    public AudioSource musicSpeakers;

    public int initialSongChoice;

    // Start is called before the first frame update
    void Start()
    {
        if (musicSpeakers == null)
        {
            musicSpeakers = gameObject.AddComponent<AudioSource>();
        }
        musicSpeakers.loop = false; // To better control the music
        initialSongChoice--;
        PlayNext();
    }

    // Update is called once per frame
    void Update()
    {
        if(musicSelectionStyle == PlayType.Silence)
        {
            if(musicSpeakers.isPlaying) // If we select silence, stop the music.
                musicSpeakers.Stop();
        }
        else if(!musicSpeakers.isPlaying)
        {
            PlayNext();
        }
    }

    public void PlayNext()
    {
        if (backgroundMusic == null || backgroundMusic.Count < 1)
        {
            Debug.Log("No music added to Jukebox!");
            return;
        }

        // In case somehow the current song disabled, or all the songs or the shuffle is super unlucky,
        // we don't want to create an infinite loop. This prevents that.
        int attemptSongs = 50;

        do
        {
            attemptSongs--;
            switch (musicSelectionStyle)
            {
                case PlayType.Silence:
                    // Basically, do nothing, since the player wants the music off
                    return;
                case PlayType.Normal:
                    initialSongChoice++;
                    if (initialSongChoice >= backgroundMusic.Count) initialSongChoice = 0;
                    break;
                case PlayType.Shuffle:
                    initialSongChoice = Random.Range(0, backgroundMusic.Count);
                    break;
                case PlayType.RepeatSingle:
                    // Clamp values, just in case. IE: in the title, we want to keep the same song playing.
                    if (initialSongChoice < 0) initialSongChoice = 0;
                    if (initialSongChoice >= backgroundMusic.Count) initialSongChoice = backgroundMusic.Count - 1;
                    break;
            }
        } while (backgroundMusic[initialSongChoice].skipSong && attemptSongs > 0);

        if (attemptSongs <= 0)
        {
            Debug.Log("Seems most or all the songs are disabled.");
            return;
        }

        musicSpeakers.clip = backgroundMusic[initialSongChoice].songClip;
        musicSpeakers.Play();
    }

    public enum PlayType
    {
        Silence,
        Normal,
        Shuffle,
        RepeatSingle        
    }

    private void OnValidate()
    {
        if (backgroundMusic != null && initialSongChoice >= backgroundMusic.Count) initialSongChoice = backgroundMusic.Count - 1;
        if (initialSongChoice < 0) initialSongChoice = 0;
    }
}

[System.Serializable]
public class BGSong
{
    public string uiName;
    public AudioClip songClip;
    public bool skipSong;
}