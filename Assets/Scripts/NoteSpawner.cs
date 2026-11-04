using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

[System.Serializable]
public class NoteTiming
{
    public float timeInSeconds;
    public int laneIndex;

    // Constructor para facilitar la creación
    public NoteTiming(float time, int lane)
    {
        timeInSeconds = time;
        laneIndex = lane;
    }
}

public class NoteSpawner : MonoBehaviour
{
    [Header("Referencias")]
    public AudioSource musicSource;
    public GameObject[] notePrefabs;
    public Transform[] spawnPoints;

    [Header("Modo Editor / Grabación")]
    public bool isRecording = true; // Actívalo en el Inspector para grabar los taps al ritmo

    [Header("Patrón de la Canción (Chart)")]
    public List<NoteTiming> songNotes = new List<NoteTiming>();

    private int nextIndex = 0;
    private bool songStarted = false;

    void Update()
    {
        // 1. Si estamos grabando el nivel con la música en vivo
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
        // 2. Si estamos jugando normalmente leyendo el chart
        else
        {
            if (musicSource.isPlaying && nextIndex < songNotes.Count)
            {
                float currentSongTime = musicSource.time;
                float approachTime = 2.0f; // Tiempo que tarda la nota en caer desde arriba

                if (currentSongTime >= (songNotes[nextIndex].timeInSeconds - approachTime))
                {
                    SpawnSpecificNote(songNotes[nextIndex].laneIndex);
                    nextIndex++;
                }
            }
        }
    }

    // Detecta tus teclas mientras suena la música y anota el segundo exacto
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

            // Instancia visualmente la nota al momento para que veas que sí la registró
            SpawnSpecificNote(recordedLane);

            Debug.Log($"Nota gravada -> Tiempo: {hitTime:F2}s | Carril: {recordedLane}");
        }
    }

    void SpawnSpecificNote(int lane)
    {
        if (spawnPoints.Length <= lane || notePrefabs.Length <= lane) return;

        Transform targetSpawn = spawnPoints[lane];
        GameObject selectedNotePrefab = notePrefabs[lane];

        Instantiate(selectedNotePrefab, targetSpawn.position, selectedNotePrefab.transform.rotation);
    }
}