using UnityEngine;
using UnityEngine.Video;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(VideoPlayer))]
public class AdSound : MonoBehaviour
{
    [Header("Distance Settings")]
    public Transform playerTransform;
    public float maxDistance = 20f;
    public float minDistance = 3f;

    private AudioSource audioSource;
    private VideoPlayer videoPlayer;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        videoPlayer = GetComponent<VideoPlayer>();

        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) playerTransform = player.transform;
        }
    }

    void Update()
    {
        if (playerTransform == null || !videoPlayer.isPlaying) return;

        float distance = Vector3.Distance(transform.position, playerTransform.position);

        // Smoothly scale volume 0 to 1
        float targetVolume = 1f - Mathf.Clamp01((distance - minDistance) / (maxDistance - minDistance));
        
        // Apply the volume to the Audio Source (which is piping the Video's audio)
        audioSource.volume = targetVolume;

        // WebGL Optimization: Mute if too far to save processing
        audioSource.mute = (distance > maxDistance);
    }
}