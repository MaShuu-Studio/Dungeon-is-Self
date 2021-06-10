using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    private static SoundController instance;
    public static SoundController Instance
    {
        get
        {
            var obj = FindObjectOfType<SoundController>();
            instance = obj;
            return instance;
        }
    }
    private void Awake()
    {
        if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }

    [SerializeField] private AudioSource currentBGM;
    [SerializeField] private GameObject currentSFXObject;
    private List<AudioSource> currentSFX = new List<AudioSource>();
    private float bgmVolume = 10;
    private float sfxVolume = 10;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        currentBGM.volume = bgmVolume;
        for(int i = 0; i < currentSFX.Count; i++)
        {
            if (currentSFX[i].isPlaying)
                currentSFX[i].volume = sfxVolume;
            else Destroy(currentSFX[i]);
        }
    }

    public void PlayBGM(string clipName)
    {
        clipName = clipName.ToUpper();
        AudioClip clip = Resources.Load<AudioClip>("Sounds/BGM/" + clipName);
        if (clip != null)
        {
            currentBGM.Stop();
            currentBGM.loop = true;
            currentBGM.playOnAwake = true;
            currentBGM.volume = bgmVolume;
            currentBGM.clip = clip;
            currentBGM.Play();
        }
    }

    public void PlaySFX(string clipName)
    {
        clipName = clipName.ToUpper();
        AudioClip clip = Resources.Load<AudioClip>("Sounds/SFX/" + clipName);
        if (clip != null)
        {
            AudioSource soundSource = currentSFXObject.AddComponent<AudioSource>();
            soundSource.loop = false;
            soundSource.playOnAwake = true;
            soundSource.volume = sfxVolume;

            soundSource.clip = clip;

            currentSFX.Add(soundSource);
            soundSource.Play();
        }
    }

    public void SetBGMVolume(float v)
    {
        bgmVolume = v / 10f;
    }
    public void SetSFXVolume(float v)
    {
        sfxVolume = v / 10f;
    }
}
