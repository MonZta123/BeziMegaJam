using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class InputDeviceManager : MonoBehaviour
{
    public static InputDeviceManager Instance { get; set; }

    [SerializeField]
    private PlayerInputManager playerInputManager;
    
    public PlayerInput PlayerInput { get; set; }

    public UnityEvent playerJoined;

    private void Awake()
    {
        if (Instance && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void OnPlayerJoined(PlayerInput playerInput)
    {
        PlayerInput = playerInput;

        playerJoined?.Invoke();
        
        playerInput.transform.SetParent(transform);
    }
}
