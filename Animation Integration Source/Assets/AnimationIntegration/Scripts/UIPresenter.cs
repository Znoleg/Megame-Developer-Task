using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIPresenter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _finishText;

    private void Start()
    {
        FinishingController.Instance.OnEnemyEnter += ShowFinishText;
        FinishingController.Instance.OnEnemiesLeft += HideFinishText;
    }

    private void HideFinishText()
    {
        _finishText.gameObject.SetActive(false);
    }

    private void ShowFinishText()
    {
        _finishText.gameObject.SetActive(true);
    }

    private void OnDestroy()
    {
        FinishingController.Instance.OnEnemyEnter -= ShowFinishText;
        FinishingController.Instance.OnEnemiesLeft -= HideFinishText;
    }
}
