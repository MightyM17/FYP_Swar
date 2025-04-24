using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    private AudioSource audioSource;

    void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    public void PlayPhonemeAudio()
    {
        string phoneme = StaticData.selectedPhoneme;
        string path = $"Audio/{phoneme}"; // Looks inside Resources/Audio/

        AudioClip clip = Resources.Load<AudioClip>(path);
        if (clip != null)
        {
            audioSource.clip = clip;
            audioSource.Play();
            Debug.Log($"Playing phoneme: {phoneme}");
        }
        else
        {
            Debug.LogWarning($"Audio file not found for phoneme: {phoneme} at Resources/Audio/{phoneme}.wav");
        }
    }
}
