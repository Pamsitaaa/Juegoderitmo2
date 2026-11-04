using UnityEngine;
using UnityEngine.Video; // O UnityEngine.Video

public class SyncManager : MonoBehaviour
{
    public UnityEngine.Video.VideoPlayer videoPlayer;
    public AudioSource audioSource;

    void Start()
    {
        videoPlayer.Prepare();
        videoPlayer.prepareCompleted += Source => {
            videoPlayer.Play();
            audioSource.Play();
        };
    }
}