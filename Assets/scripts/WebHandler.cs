using UnityEngine;
using System.Runtime.InteropServices;

public class WebHandler : MonoBehaviour
{
    // Link to the .jslib function
    [DllImport("__Internal")]
    private static extern void OpenNewTab(string url);

    public void OpenLink(string url)
    {
        #if !UNITY_EDITOR && UNITY_WEBGL
            OpenNewTab(url);
        #else
            // Fallback for the Unity Editor or other platforms
            Application.OpenURL(url);
        #endif
    }
}