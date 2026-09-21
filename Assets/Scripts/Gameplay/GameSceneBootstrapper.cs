using System.Diagnostics;
using UnityEngine;


public class GameSceneBootstrapper : MonoBehaviour
{
    
    private Player player;

    [SerializeField]
    private GameObject splashScreen;
    
    private void Start()
    {
        var instance = InputDeviceManager.Instance;
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
        if (InputDeviceManager.Instance.PlayerInput)
        {
            player.SetPlayerInput(instance.PlayerInput);
        }
        else
        {
            Time.timeScale = 0;
            splashScreen.SetActive(true);
            instance.playerJoined.AddListener(OnPlayerJoined);
        }

        if (!Settings.Instance)
        {
            new GameObject("Settings").AddComponent<Settings>();
        }
    }
    
    private void OnPlayerJoined()
    {
        UnityEngine.Debug.Log(player);
        Time.timeScale = 1;
        splashScreen.SetActive(false);
        player.SetPlayerInput(InputDeviceManager.Instance.PlayerInput);
        InputDeviceManager.Instance.playerJoined.RemoveListener(OnPlayerJoined);
    }
}
