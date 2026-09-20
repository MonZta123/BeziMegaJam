using UnityEngine;

public class GameSceneBootstrapper : MonoBehaviour
{
    [SerializeField]
    private Player player;

    [SerializeField]
    private GameObject splashScreen;
    
    private void Start()
    {
        var instance = InputDeviceManager.Instance;
        
        if (InputDeviceManager.Instance.PlayerInput)
        {
            player.SetPlayerInput(instance.PlayerInput);
        }
        else
        {
            splashScreen.SetActive(true);
            instance.playerJoined.AddListener(OnPlayerJoined);
        }
    }
    
    private void OnPlayerJoined()
    {
        splashScreen.SetActive(false);
        player.SetPlayerInput(InputDeviceManager.Instance.PlayerInput);
        InputDeviceManager.Instance.playerJoined.RemoveListener(OnPlayerJoined);
    }
}
