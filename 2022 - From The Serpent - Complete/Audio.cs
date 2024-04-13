using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Audio : MonoBehaviour
{
    private AudioSource AS;
    public AudioMixer audioMixer;
    public Slider SFXSlider;
    public Slider BGMSlider;
    public AudioClip buttonClip;
    public AudioClip eatSound;
    public AudioClip dieSound;
    public float KhiahVol = 0.2f;
    public List<AudioClip> KhiahSounds;

    private void Awake()
    {
        AS = GetComponent<AudioSource>();
    }

    private void Start()
    {
        Load();
    }

    public void PlaySound(AudioClip audio)
    {
        AS.PlayOneShot(audio);
    }

    public void PlayButtonSound()
    {
        AS.PlayOneShot(buttonClip);
    }

    public void SetBGMVolume(float v)
    {
        audioMixer.SetFloat("BGM", v);
        PlayerPrefs.SetFloat("BGM", v);
        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float v)
    {
        audioMixer.SetFloat("SFX", v);
        PlayerPrefs.SetFloat("SFX", v);
        PlayerPrefs.Save();
    }

    private void Load()
    {
        SFXSlider.value = PlayerPrefs.GetFloat("SFX", 0f);
        audioMixer.SetFloat("SFX", PlayerPrefs.GetFloat("SFX", 0f));
        BGMSlider.value = PlayerPrefs.GetFloat("BGM", 0f);
        audioMixer.SetFloat("BGM", PlayerPrefs.GetFloat("BGM", 0f));
    }

    public void PlayUpKhiahSound()
    {
        AS.PlayOneShot(KhiahSounds[0], KhiahVol);
    }

    public void PlayDownKhiahSound()
    {
        AS.PlayOneShot(KhiahSounds[1], KhiahVol);
    }

    public void PlayLeftKhiahSound()
    {
        AS.PlayOneShot(KhiahSounds[2], KhiahVol);
    }

    public void PlayRightKhiahSound()
    {
        AS.PlayOneShot(KhiahSounds[3], KhiahVol);
    }

    public void PlayStateKhiahSound()
    {
        AS.PlayOneShot(KhiahSounds[4], KhiahVol);
    }

    public void PlayEatSound()
    {
        AS.PlayOneShot(eatSound, 0.25f);
    }

    public void PlayDieSound()
    {
        AS.PlayOneShot(dieSound, 0.2f);
    }
}
