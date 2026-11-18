using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

[System.Serializable]
public class SongData
{
    public string songName;
    public Sprite coverArt;
    public AudioClip fullSong;
    public float previewStartTime;
    public float previewDuration = 15f;
    public float songBPM = 120f;
}

public class SongSelector : MonoBehaviour
{
    [Header("Lista de Canciones (Tu Biblioteca)")]
    public SongData[] songs;

    [Header("Referencias de la UI")]
    public Image coverImage;
    public Text songTitleText;

    [Header("Audio Preview")]
    public AudioSource previewSource;

    [Header("Configuración de Volumen (Para no sordo-mudos)")]
    [Range(0f, 1f)] public float maxPreviewVolume = 0.35f; // <--- AQUÍ MANDAS TÚ. Ponlo bajito (ej. 0.3 o 0.4) para que no te queme los oídos.
    public float fadeInDuration = 1.5f; // <--- Qué tan lento quieres que suba (1.5 segundos es una delicia de suave).

    [Header("Escena de Juego")]
    public string gameplaySceneName = "GameplayScene";

    private int currentIndex = 0;
    private float previewTimer = 0f;
    private Coroutine fadeCoroutine;

    public static SongData selectedSong;

    void Start()
    {
        if (songs.Length > 0)
        {
            UpdateSongDisplayInitial();
        }
    }

    void Update()
    {
        if (previewSource != null && previewSource.isPlaying)
        {
            previewTimer += Time.deltaTime;
            if (previewTimer >= songs[currentIndex].previewDuration)
            {
                PlaySongPreview();
            }
        }
    }

    public void NextSong()
    {
        if (songs.Length == 0) return;

        currentIndex++;
        if (currentIndex >= songs.Length)
        {
            currentIndex = 0;
        }

        UpdateSongDisplay();
    }

    public void PreviousSong()
    {
        if (songs.Length == 0) return;

        currentIndex--;
        if (currentIndex < 0)
        {
            currentIndex = songs.Length - 1;
        }

        UpdateSongDisplay();
    }

    void UpdateSongDisplay()
    {
        SongData currentSong = songs[currentIndex];

        if (coverImage != null) coverImage.sprite = currentSong.coverArt;
        if (songTitleText != null) songTitleText.text = currentSong.songName;

        selectedSong = currentSong;

        PlaySongPreview();
    }

    void UpdateSongDisplayInitial()
    {
        SongData currentSong = songs[currentIndex];

        if (coverImage != null) coverImage.sprite = currentSong.coverArt;
        if (songTitleText != null) songTitleText.text = currentSong.songName;

        selectedSong = currentSong;

        if (previewSource == null || currentSong.fullSong == null) return;

        previewTimer = 0f;
        previewSource.Stop();
        previewSource.clip = currentSong.fullSong;

        if (currentSong.previewStartTime < currentSong.fullSong.length)
        {
            previewSource.time = currentSong.previewStartTime;
        }
        else
        {
            previewSource.time = 0f;
        }

        previewSource.volume = 0f;
        previewSource.Play();

        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);

        // Entra lento y llega al volumen seguro que pusiste arriba
        fadeCoroutine = StartCoroutine(FadeInOnly(fadeInDuration));
    }

    IEnumerator FadeInOnly(float fadeTime)
    {
        float timer = 0f;
        while (timer < fadeTime)
        {
            timer += Time.unscaledDeltaTime;
            // Sube progresivamente desde 0 hasta el 'maxPreviewVolume' (ej. 0.35)
            previewSource.volume = Mathf.Lerp(0f, maxPreviewVolume, timer / fadeTime);
            yield return null;
        }
        previewSource.volume = maxPreviewVolume;
    }

    void PlaySongPreview()
    {
        if (previewSource == null || songs[currentIndex].fullSong == null) return;

        SongData currentSong = songs[currentIndex];
        previewTimer = 0f;

        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        fadeCoroutine = StartCoroutine(FadeAndSwitchSong(currentSong));
    }

    IEnumerator FadeAndSwitchSong(SongData newSong)
    {
        float fadeTime = 0.6f; // Tiempo de transición al cambiar de canción
        float startVolume = previewSource.volume;

        // 1. FADE OUT de salida
        if (previewSource.isPlaying)
        {
            float timer = 0f;
            while (timer < fadeTime)
            {
                timer += Time.unscaledDeltaTime;
                previewSource.volume = Mathf.Lerp(startVolume, 0f, timer / fadeTime);
                yield return null;
            }
            previewSource.Stop();
        }

        previewSource.clip = newSong.fullSong;

        if (newSong.previewStartTime < newSong.fullSong.length)
        {
            previewSource.time = newSong.previewStartTime;
        }
        else
        {
            previewSource.time = 0f;
        }

        previewSource.volume = 0f;
        previewSource.Play();

        // 2. FADE IN de entrada hacia el nuevo tema de forma muy progresiva
        float fadeInTimer = 0f;
        while (fadeInTimer < fadeInDuration)
        {
            fadeInTimer += Time.unscaledDeltaTime;
            previewSource.volume = Mathf.Lerp(0f, maxPreviewVolume, fadeInTimer / fadeInDuration);
            yield return null;
        }
        previewSource.volume = maxPreviewVolume;
    }

    public void PlaySelectedSong()
    {
        selectedSong = songs[currentIndex];

        if (previewSource != null) previewSource.Stop();

        SceneManager.LoadScene(gameplaySceneName);
    }
}