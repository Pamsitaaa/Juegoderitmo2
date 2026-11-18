using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

[System.Serializable]
public class NoteTiming
{
    public float timeInSeconds;
    public int laneIndex;

    public NoteTiming(float time, int lane)
    {
        timeInSeconds = time;
        laneIndex = lane;
    }
}

public class NoteSpawner : MonoBehaviour
{
    [Header("Calibración de Ritmo (Offset)")]
    [Tooltip("Súbele o bájale aquí para adelantar o atrasar las notas sin tocar la grabación.")]
    public float globalOffset = 0.0f; // Si van tarde, ponle números negativos (ej. -0.3f). Si van temprano, positivos.

    [Header("Referencias")]
    public AudioSource musicSource;
    public GameObject[] notePrefabs;
    public Transform[] spawnPoints;

    [Header("Referencias de Audio (SFX)")]
    public AudioSource sfxSource;   // AudioSource exclusivo para efectos (como el Fail)
    public AudioClip failSound;     // Tu archivo de audio "Fail"

    [Header("Modo Editor / Grabación")]
    public bool isRecording = true;

    [Header("Patrón de la Canción (Chart)")]
    public List<NoteTiming> songNotes = new List<NoteTiming>();

    private int nextIndex = 0;
    private bool songStarted = false;

    void Update()
    {
        if (isRecording)
        {
            if (!songStarted && musicSource.isPlaying)
            {
                songStarted = true;
                Debug.Log("--- ¡GRABACIÓN INICIADA AL RITMO DE LA MÚSICA! ---");
            }

            if (musicSource.isPlaying)
            {
                DetectRecordingInputs();
            }
        }
        else
        {
            if (musicSource.isPlaying && nextIndex < songNotes.Count)
            {
                float currentSongTime = musicSource.time;
                float approachTime = 2.0f;

                // APLICAMOS EL OFFSET AQUÍ: 
                // Esto engaña al sistema para que spawnee la nota antes o después sin alterar sus datos puros.
                float targetTimeWithOffset = songNotes[nextIndex].timeInSeconds + globalOffset;

                if (currentSongTime >= (targetTimeWithOffset - approachTime))
                {
                    SpawnSpecificNote(songNotes[nextIndex].laneIndex);
                    nextIndex++;
                }
            }
        }
    }

    void DetectRecordingInputs()
    {
        if (Keyboard.current == null) return;

        int recordedLane = -1;

        if (Keyboard.current.qKey.wasPressedThisFrame) recordedLane = 0;
        else if (Keyboard.current.eKey.wasPressedThisFrame) recordedLane = 1;
        else if (Keyboard.current.spaceKey.wasPressedThisFrame) recordedLane = 2;
        else if (Keyboard.current.aKey.wasPressedThisFrame) recordedLane = 3;
        else if (Keyboard.current.dKey.wasPressedThisFrame) recordedLane = 4;

        if (recordedLane != -1)
        {
            float hitTime = musicSource.time;
            songNotes.Add(new NoteTiming(hitTime, recordedLane));

            SpawnSpecificNote(recordedLane);

            Debug.Log($"Nota grabada -> Tiempo: {hitTime:F2}s | Carril: {recordedLane}");
        }
    }

    void SpawnSpecificNote(int lane)
    {
        if (spawnPoints == null || notePrefabs == null) return;
        if (lane < 0 || lane >= spawnPoints.Length || lane >= notePrefabs.Length) return;

        Transform targetSpawn = spawnPoints[lane];
        GameObject selectedNotePrefab = notePrefabs[lane];

        // Pedimos la nota al NotePooler usando el índice exacto del carril
        GameObject newNote = NotePooler.Instance.GetPooledNote(lane);

        if (newNote != null && targetSpawn != null && selectedNotePrefab != null)
        {
            newNote.transform.position = targetSpawn.position;

            // Respetamos la rotación original exacta del prefab
            newNote.transform.rotation = selectedNotePrefab.transform.rotation;

            newNote.SetActive(true);
        }
    }

    // Método para reproducir el sonido de fallo cuando lo necesites
    // Método para reproducir el sonido de fallo de forma obligatoria y sin bloqueos
    // Método blindado para que el sonido de fallo nunca falle ni se sature
    // Método para reproducir el sonido de fallo encimándose (polifonía de errores)
    public void PlayFailSound()
    {
        if (sfxSource != null && failSound != null)
        {
            // ¡Ojo aquí! Usamos PlayOneShot SIN hacer .Stop() previo.
            // Esto permite que si te equivocas 200 veces seguidas, cada error suene a la par encima del anterior.
            sfxSource.PlayOneShot(failSound);
        }
    }
}