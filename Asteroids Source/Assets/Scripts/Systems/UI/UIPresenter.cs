using System;
using TMPro;
using UnityEngine;

public class UIPresenter : MonoBehaviour
{
    [SerializeField] private RectTransform _menuContainer;
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private HealthUI _healthUI;
    [SerializeField] private InputUIButton _inputUIButton;
    [SerializeField] private EventButton _newGameButton;
    [SerializeField] private ContinueButton _continueButton;
    [SerializeField] private EventButton _exitButton;

    private void Start()
    {
        _healthUI.SetMaxHealth(PlayerBody.Instance.LivesCount);

        GameManager.Instance.OnPause += ActivateMenu;
        GameManager.Instance.OnContinue += DeactivateMenu;
        ScoreCounter.Instance.OnScoreChange += ChangeScoreText;
        PlayerBody.Instance.OnHit += RemoveHealth;
        InputEvents.OnPauseKeyDown += ChangePause;
        _continueButton.OnClick += GameManager.Instance.ContinueGame;
        _inputUIButton.OnInputChange += ChangeInput;
        _newGameButton.OnClick += StartGameByButton;
        _exitButton.OnClick += GameManager.Instance.ExitGame;

        SetMenuActive(GameManager.Instance.IsPaused);
    }

    private void DeactivateMenu() => SetMenuActive(false);

    private void ActivateMenu() => SetMenuActive(true);

    private void SetMenuActive(bool isActive)
    {
        _menuContainer.gameObject.SetActive(isActive);
        if (isActive)
        {
            _continueButton.SetInteractable(GameManager.Instance.IsGameStarted);
        }
    }

    private void StartGameByButton()
    {
        GameManager.Instance.StartNewGame();
    }

    private void ChangeInput(InputBehaviourBase input)
    {
        InputReciever.Instance.ChangeInput(input);
    }

    private void RemoveHealth(int healthValue)
    {
        _healthUI.RemoveHealthByValue(healthValue);
    }

    private void ChangeScoreText(int score)
    {
        _scoreText.text = score.ToString();
    }

    private void ChangePause()
    {
        if (!GameManager.Instance.IsGameStarted) return;
        if (GameManager.Instance.IsPaused) GameManager.Instance.ContinueGame();
        else GameManager.Instance.PauseGame();
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnPause -= ActivateMenu;
        GameManager.Instance.OnContinue -= DeactivateMenu;
        ScoreCounter.Instance.OnScoreChange -= ChangeScoreText;
        PlayerBody.Instance.OnHit -= RemoveHealth;
        InputEvents.OnPauseKeyDown -= ChangePause;
        _continueButton.OnClick -= GameManager.Instance.ContinueGame;
        _inputUIButton.OnInputChange -= ChangeInput;
        _newGameButton.OnClick -= StartGameByButton;
        _exitButton.OnClick -= GameManager.Instance.ExitGame;
    }
}

