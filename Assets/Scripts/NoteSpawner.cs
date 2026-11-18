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
    public float globalOffset = 0.0f;

    [Header("Referencias")]
    public AudioSource musicSource;
    public GameObject[] notePrefabs;
    public Transform[] spawnPoints;

    [Header("Referencias de Audio (SFX)")]
    public AudioSource sfxSource;
    public AudioClip failSound;

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

        GameObject newNote = NotePooler.Instance.GetPooledNote(lane);

        if (newNote != null && targetSpawn != null && selectedNotePrefab != null)
        {
            newNote.transform.position = targetSpawn.position;
            newNote.transform.rotation = selectedNotePrefab.transform.rotation;
            newNote.SetActive(true);
        }
    }

    public void PlayFailSound()
    {
        if (sfxSource != null && failSound != null)
        {
            sfxSource.PlayOneShot(failSound);
        }
    }

    // ==========================================
    // GENERADOR AUTOMÁTICO PARA PRAY 2.0 (4:54 min / 294 segundos)
    // ==========================================
    [Header("Generador Automático por BPM (Pray 2.0)")]
    public float songStartTime = 0f;
    public float songEndTime = 294f;  // 4 minutos con 54 segundos exactos

    [ContextMenu("Generar Patrón Base 135-180 BPM")]
    public void GenerateBpmTestPattern()
    {
        songNotes.Clear();
        float currentTime = songStartTime;
        float currentBpm = 135f;
        int laneCycle = 0;

        while (currentTime < songEndTime)
        {
            float beatInterval = 60f / currentBpm;
            songNotes.Add(new NoteTiming(currentTime, laneCycle));

            laneCycle = (laneCycle + 1) % 5;
            currentTime += beatInterval;

            // Calculamos el progreso porcentual a lo largo de toda la canción de 4:54 min
            float progress = currentTime / songEndTime;

            // Cambios de ritmo escalados proporcionalmente para que cubran toda la duración:
            if (progress > 0.15f && currentBpm < 155f) currentBpm = 155f;
            if (progress > 0.35f && currentBpm < 170f) currentBpm = 170f;
            if (progress > 0.55f && currentBpm < 180f) currentBpm = 180f;
            if (progress > 0.70f && currentBpm > 170f) currentBpm = 170f; // Bajón de pre-clímax
            if (progress > 0.85f && currentBpm < 180f) currentBpm = 180f; // Cierre insano a tope
        }

        Debug.Log($"¡Patrón épico para Pray 2.0 generado con éxito! Total de notas: {songNotes.Count}");
    }
}