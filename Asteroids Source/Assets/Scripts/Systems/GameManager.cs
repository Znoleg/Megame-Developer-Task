using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    private bool _pausedOnStart = true;

    public event Action OnGameStart;
    public event Action OnNewLevelStart;
    public event Action OnGameEnd;

    public event Action OnPause;
    public event Action OnContinue;

    public bool IsGameStarted { get; private set; } = false;
    public bool IsPaused { get; private set; }

    public void StartNewGame()
    {
        if (IsGameStarted)
        {
            var waitRoutine = new CoroutineObject(this, WaitForSceneReload);
            waitRoutine.Start();
            SceneManager.LoadScene(SceneManager.GetSceneAt(0).buildIndex);
        }
        else
        {
            StartGameInternal();
        }

        IEnumerator WaitForSceneReload()
        {
            yield return new WaitUntil(() => PlayerMovement.Instance != null
                && AsteroidsSpawner.Instance != null
                && AsteroidsDestroyer.Instance != null
                && UFOSpawner.Instance != null);
            
            StartGameInternal();
        }

        void StartGameInternal()
        {
            IsGameStarted = true;
            OnGameStart?.Invoke();
            ContinueGame();
        }
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void PauseGame()
    {
        if (IsPaused) return;
        IsPaused = true;

        Time.timeScale = 0f;
        OnPause?.Invoke();
    }

    public void ContinueGame()
    {
        if (!IsPaused) return;
        IsPaused = false;

        Time.timeScale = 1f;
        OnContinue?.Invoke();
    }

    protected override void Awake()
    {
        try
        {
            base.Awake();
            IsPaused = _pausedOnStart;
            DontDestroyOnLoad(gameObject);
        }
        catch
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        PlayerBody.Instance.OnDying += InvokeOnGameEnd;
        AsteroidsSpawner.Instance.OnAsteroidsSpawn += InvokeOnLevelStart;
    }

    private void InvokeOnLevelStart()
    {
        OnNewLevelStart?.Invoke();
    }

    private void InvokeOnGameEnd()
    {
        OnGameEnd?.Invoke();
        PlayerBody.Instance.OnDying -= InvokeOnGameEnd;
    }

    private void OnDestroy()
    {
        AsteroidsSpawner.Instance.OnAsteroidsSpawn -= InvokeOnLevelStart;
    }
}

