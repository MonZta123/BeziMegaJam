using UnityEngine;

public class Initialization : MonoBehaviour
{

    private void Awake()
    {
        // Disable V-Sync to allow custom target framerates
        //QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 120; // Or -1 for uncapped
    }
}
