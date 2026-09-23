using UnityEngine;

public class GlobalAudioManager : MonoBehaviour
{
    [SerializeField]
    private AudioSource musicAudioSource;
    
    public static GlobalAudioManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }
}
