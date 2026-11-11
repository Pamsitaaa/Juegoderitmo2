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
    [Header("Referencias")]
    public AudioSource musicSource;
    public GameObject[] notePrefabs;
    public Transform[] spawnPoints;

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

                if (currentSongTime >= (songNotes[nextIndex].timeInSeconds - approachTime))
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

            // ¡Forzamos que use la rotación local exacta del prefab original!
            newNote.transform.rotation = selectedNotePrefab.transform.rotation;

            newNote.SetActive(true);
        }
    }



}