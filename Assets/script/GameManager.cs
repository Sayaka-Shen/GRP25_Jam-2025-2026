using System;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] private TextMeshProUGUI _startText; // attention, prends bien TextMeshProUGUI si c'est dans un Canvas UI

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        StartCountDown();
    }

    public void StartCountDown()
    {
        // Reset du texte et alpha
        _startText.text = "";
        _startText.alpha = 1f;
        _startText.transform.localScale = Vector3.one;

        // Créer une séquence DOTween
        Sequence seq = DOTween.Sequence();

        // Valeurs du compte à rebours
        string[] countdown = { "3", "2", "1", "GO!" };

        foreach (string value in countdown)
        {
            seq.AppendCallback(() =>
            {
                _startText.text = value;
                _startText.alpha = 0f;
                _startText.transform.localScale = Vector3.zero;
            });

            // Fade + scale pour faire apparaître
            seq.Append(_startText.DOFade(1f, 0.3f));
            seq.Join(_startText.transform.DOScale(1.2f, 0.3f).SetEase(Ease.OutBack));

            // Attente affichage
            seq.AppendInterval(0.4f);

            // Disparition
            seq.Append(_startText.DOFade(0f, 0.2f));
            seq.Join(_startText.transform.DOScale(0f, 0.2f).SetEase(Ease.InBack));
        }

        // Optionnel : après le "GO!", tu veux peut-être désactiver le texte
        seq.OnComplete(() =>
        {
            _startText.text = "";
            FindAnyObjectByType<Timer>().StartTimer();
        });
    }
}