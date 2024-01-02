using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class volumeslider : MonoBehaviour
{
    public Slider volumeSlider;
    void Start()
    {

        if (!PlayerPrefs.HasKey("Music"))
        {
            PlayerPrefs.SetFloat("Music", 1);
            Load();
        }
        else
        {
            Load();
        }
    }

    void Update()
    {
       
    }
    public void ChangeVolume()
    {
        AudioListener.volume = volumeSlider.value;
        Save();
    }
    public void Save()
    {
        PlayerPrefs.SetFloat("Music",volumeSlider.value);
    }
    public void Load()
    {
        volumeSlider.value = PlayerPrefs.GetFloat("Music");
        AudioListener.volume = volumeSlider.value;
    }
}