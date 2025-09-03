using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _timerText; // Texte UI pour afficher le temps
    [SerializeField] private float _duration = 180f;     // 3 minutes = 180 secondes

    private float _timeRemaining;
    private bool _isRunning = false;

    private void Update()
    {
        if (_isRunning)
        {
            _timeRemaining -= Time.deltaTime;

            if (_timeRemaining <= 0f)
            {
                _timeRemaining = 0f;
                _isRunning = false;
                OnTimerEnd();
            }

            UpdateTimerUI();
        }
    }

    /// <summary>
    /// Démarre le timer
    /// </summary>
    public void StartTimer()
    {
        _timeRemaining = _duration;
        _isRunning = true;
        UpdateTimerUI();
    }

    /// <summary>
    /// Met à jour le texte UI
    /// </summary>
    private void UpdateTimerUI()
    {
        int minutes = Mathf.FloorToInt(_timeRemaining / 60);
        int seconds = Mathf.FloorToInt(_timeRemaining % 60);
        _timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    /// <summary>
    /// Appelé quand le timer est fini
    /// </summary>
    private void OnTimerEnd()
    {
        _timerText.text = "00:00";
        Debug.Log("Timer terminé !");
        // Tu peux déclencher ici une autre action (game over, etc.)
    }
}