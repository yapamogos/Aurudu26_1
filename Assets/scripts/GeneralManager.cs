using UnityEngine;
using System.Collections;
using System.Threading;

public class GeneralManager : MonoBehaviour
{
    public static GeneralManager Instance { get; private set; }

    [Header("Frame Settings")]
    int MaxRate = 120;
    public float TargetFrameRate = 120.0f;
    float currentFrameTime;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = MaxRate;
        currentFrameTime = Time.realtimeSinceStartup;
        StartCoroutine("WaitForNextFrame");
    }

    IEnumerator WaitForNextFrame()
    {
        while (true)
        {
            yield return new WaitForEndOfFrame();
            currentFrameTime += 1.0f / TargetFrameRate;
            var t = Time.realtimeSinceStartup;
            var sleepTime = currentFrameTime - t - 0.01f;
            if (sleepTime > 0)
                Thread.Sleep((int)(sleepTime * 1000));
            while (t < currentFrameTime)
                t = Time.realtimeSinceStartup;
        }
    }

    public int currentCharacterIndex = 0;
    public int currentColorIndex = 0;

    public void SetCurrentCharacterIndex(int index)
    {
        currentCharacterIndex = index;
    }

    public void SetCurrentColorIndex(int index)
    {
        currentColorIndex = index;
    }
}
