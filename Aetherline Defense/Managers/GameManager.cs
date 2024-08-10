using Eflatun.SceneReference;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    public readonly static GameEvents Events = new();

    [Header("Scene Settings")]
    [SerializeField] private SceneReference[] gameScenes;

    [Header("Timer Settings")]
    [SerializeField] private float startTime = 10f;
    public CountdownTimer Timer { get; private set; }

    private void Start()
    {
        NetworkManager.Singleton.OnServerStarted += OnServerStart;
    }

    private void OnEnable()
    {
        Timer = new CountdownTimer(startTime);
        Timer.OnTimerStop += OnGameCommence;

        if (PlayerManager.Instance != null)
            PlayerManager.Events.AllPlayersReady += OnPlayersReady;
        else 
            Debug.Log("No player manager found in gameManager");
    }

    private void OnDisable()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.SceneManager.OnLoadComplete -= OnSceneLoaded;
            NetworkManager.Singleton.OnServerStarted -= OnServerStart;
        }
        Timer.OnTimerStop -= OnGameCommence;

        if (PlayerManager.Instance != null)
            PlayerManager.Events.AllPlayersReady -= OnPlayersReady;
    }

    private void Update()
    {
        if (Timer.IsRunning)
        {
            Debug.Log($" Game Timer is at {TimerUtils.FloatToTime(Timer.Progress * startTime)}");
            Timer.Tick(Time.deltaTime);
        }
    }

    private void OnServerStart()
    {
        Debug.Log("Server Started, moving scenes");
        NetworkManager.Singleton.SceneManager.OnLoadComplete += OnSceneLoaded;
        NetworkManager.Singleton.SceneManager.LoadScene(gameScenes[0].Name, LoadSceneMode.Single);
    }

    private void OnSceneLoaded(ulong clientId, string sceneName, LoadSceneMode loadSceneMode)
    {
        // Check if the current scene is in the gameScenes array
        if (SceneManager.GetSceneByName(sceneName).IsReferenced(gameScenes))
        {
            Debug.Log("Game Scene Loaded");
            Events.OnLoadGameScene.Invoke();
            ToggleGameSystems(false);
        }
    }

    private void OnPlayersReady()
    {
        Timer.Start();
        Debug.Log("All players ready, countdown start");
    }

    private void OnGameCommence()
    {
        // Enable All Game and Player Systems
        Debug.Log("Game Commence");
        ToggleGameSystems(true);
        Events.OnGameStart.Invoke();
    }

    private void ToggleGameSystems(bool value)
    {
        Debug.Log($"Game Events Toggled: {value}");
        Events.OnGameSystemsToggled.Invoke(value);
    }

    [ContextMenu("On Players Ready")]
    public void OnPlayersReadyContext()
    {
        OnPlayersReady();
    }
}
