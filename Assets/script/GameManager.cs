using System;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] private TextMeshProUGUI _startText; // attention, prends bien TextMeshProUGUI si c'est dans un Canvas UI
    public bool HasGameEnd {  get; private set; }

    [SerializeField] public TextMeshProUGUI _player1Score; // attention, prends bien TextMeshProUGUI si c'est dans un Canvas UI
    [SerializeField] public TextMeshProUGUI _player2Score; // attention, prends bien TextMeshProUGUI si c'est dans un Canvas UI
    [SerializeField] public Image _player1PowerUp;
    [SerializeField] public Image _player2PowerUp;
    public Material[] Materials;
    public GameObject[] _imageWin;
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
        HasGameEnd = false;
        Color color = _player1PowerUp.color;
        color.a = 0; 
        color = _player2PowerUp.color;
        color.a = 0; 
        _player1PowerUp.color = color;
        _player2PowerUp.color = color;
        StartCountDown();
    }

    public void OnAlumette(Alumette.AlumetteState alumetteState, bool Player1)
    {
        if (Player1)
        {
            switch (alumetteState)
            {
                case Alumette.AlumetteState.Dash:
                    _player1PowerUp.material = Materials[1];
                    break;
                
                case Alumette.AlumetteState.Bouteille:
                    _player1PowerUp.material = Materials[2];
                    break;
                
                case Alumette.AlumetteState.Savon:
                    _player1PowerUp.material = Materials[3];
                    break;
                
                case Alumette.AlumetteState.FireRing:
                    _player1PowerUp.material = Materials[4];
                    break;
            }
            Color color = _player1PowerUp.color;
            color.a = 255f; 
            _player1PowerUp.color = color;
        }
        else
        {
            switch (alumetteState)
            {
                
                case Alumette.AlumetteState.BaseState:
                    _player2PowerUp.material = Materials[0];
                    break;
                case Alumette.AlumetteState.Dash:
                    _player2PowerUp.material = Materials[1];
                    break;
                case Alumette.AlumetteState.Bouteille:
                    _player2PowerUp.material = Materials[2];
                    break;
                case Alumette.AlumetteState.Savon:
                    _player2PowerUp.material = Materials[3];
                    break;
                
                case Alumette.AlumetteState.FireRing:
                    _player2PowerUp.material = Materials[4];
                    break;
                
                default:
                    _player2PowerUp.material = Materials[0];
                    break;
            }
            Color color = _player2PowerUp.color;
            color.a = 255f; 
            _player2PowerUp.color = color;
            
        }
        
    }

    public void OnAlumetteUse(bool player)
    {
        if (player)
        {
            Color color = _player1PowerUp.color;
            color.a = 0; 
            _player1PowerUp.color = color;
        }
        else
        {
            Color color = _player2PowerUp.color;
            color.a = 0; 
            _player2PowerUp.color = color;
        }
    }

    public void OnGameEnd()
    {
        HasGameEnd = true;

        PlayerMouvement[] players = FindObjectsByType<PlayerMouvement>(FindObjectsSortMode.None);
        foreach (PlayerMouvement player in players)
        {
            player._canMove = false;
        }
        Slider slider = FindAnyObjectByType<CampFire>().m_sliderAllumettes;
        if (slider.value == 50)
        { 
            _imageWin[2].SetActive(true);
        }
        else if (slider.value > 50)
        {
            _imageWin[0].SetActive(true);
        }
        else if (slider.value < 50)
        {
            _imageWin[1].SetActive(true);

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
            _startText.alpha = 1f;
            _startText.transform.localScale = Vector3.one;
            FindAnyObjectByType<Timer>().StartTimer();
            PlayerMouvement[] players = FindObjectsByType<PlayerMouvement>(FindObjectsSortMode.None);
            foreach (PlayerMouvement player in players)
            {
                player._canMove = true;
            }
        });
    }

    public void AddPoints(int points, bool Player1)
    {
        if (Player1)
        {
            int currentScore = int.Parse(_player1Score.text);
            int newScore = currentScore + points;
            _player1Score.text = newScore.ToString();
        }
        else
        {
            int currentScore = int.Parse(_player2Score.text);
            int newScore = currentScore + points;
            _player2Score.text = newScore.ToString();
        }
    }
    public void ShakeCamera()
    {
        Vector3 cam = Camera.main.transform.position;
        Camera.main.transform.DOShakePosition(0.2f, 1f, 2, 90, false, true)
            .OnComplete(() =>
            {
                Camera.main.transform.position = cam;
            });
    }
    public void ResetPoint(bool Player1)
    {
        if (Player1)
        {
            _player1Score.text = "0";
        }
        else
        {
            _player2Score.text = "0";
        }
    }
}