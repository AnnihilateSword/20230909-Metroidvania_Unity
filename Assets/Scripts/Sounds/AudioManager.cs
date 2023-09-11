using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public AudioMixer musicMixer, effectsMixer;
    public AudioSource backgroundMusic, arrow, hit, flame, gold, skeletonDeath, skeletonDamage,
        playerDeath, playerDamage, playerLevelUp;

    [Range(-80, 20)] public float masterVolume, effectsVolume;
    public Slider masterSlider, effectsSlider;

    public static AudioManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        PlayAudio(backgroundMusic);
        masterSlider.value = masterVolume;
        effectsSlider.value = effectsVolume;

        masterSlider.minValue = -80.0f;
        masterSlider.maxValue = 20.0f;
        effectsSlider.minValue = -80.0f;
        effectsSlider.maxValue = 20.0f;
    }

    private void Update()
    {
        MasterVolume();
        EffectsVolume();
    }

    public void PlayAudio(AudioSource audioSource)
    {
        audioSource.Play();
    }

    public void MasterVolume()
    {
        musicMixer.SetFloat("masterVolume", masterSlider.value);
    }

    public void EffectsVolume()
    {
        effectsMixer.SetFloat("effectsVolume", effectsSlider.value);
    }
}
