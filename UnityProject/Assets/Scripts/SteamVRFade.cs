using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteamVRFade : MonoBehaviour {

    // Fade the screen to black
    public static void fadeOut()
    {
        // SteamVR_Fade is too fast in builds.  We compenstate for this here.
#if UNITY_EDITOR
        SteamVR_Fade.View(Color.black, .15f);
#else
				SteamVR_Fade.View(Color.black, .15f * .666f);
#endif
    }

    // Fade the screen back to clear
    public static void fadeIn()
    {
        // SteamVR_Fade is too fast in builds.  We compenstate for this here.
#if UNITY_EDITOR
        SteamVR_Fade.View(Color.clear, .35f);
#else
				SteamVR_Fade.View(Color.clear, .35f * .666f);
#endif
    }

    // Fade the screen to black
    public static void fadeOut(float FadeOutSec)
    {
        // SteamVR_Fade is too fast in builds.  We compenstate for this here.
#if UNITY_EDITOR
        SteamVR_Fade.View(Color.black, FadeOutSec);
#else
				SteamVR_Fade.View(Color.black, FadeOutSec * .666f);
#endif
    }

    // Fade the screen back to clear
    public static void fadeIn(float FadeInSec)
    {
        // SteamVR_Fade is too fast in builds.  We compenstate for this here.
#if UNITY_EDITOR
        SteamVR_Fade.View(Color.clear, FadeInSec);
#else
				SteamVR_Fade.View(Color.clear, FadeInSec * .666f);
#endif
    }
}
