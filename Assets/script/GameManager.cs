using System;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;

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

    public void OnGameEnd()
    {
        PlayerMouvement[] players = FindObjectsByType<PlayerMouvement>(FindObjectsSortMode.None);
        foreach (PlayerMouvement player in players)
        {
            player._canMove = false;
        }
        Slider slider = FindAnyObjectByType<CampFire>().m_sliderAllumettes;
        if (slider.value == 50)
        { 
            _startText.text = "That is a perfect draw !!";
        }
        else if (slider.value > 50)
        {
            _startText.text = "Player 1 Win !!";
        }
        else if (slider.value < 50)
        {
            _startText.text = "Player 2 Win !!";
        }
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
            PlayerMouvement[] players = FindObjectsByType<PlayerMouvement>(FindObjectsSortMode.None);
            foreach (PlayerMouvement player in players)
            {
                player._canMove = true;
            }
        });
    }
}