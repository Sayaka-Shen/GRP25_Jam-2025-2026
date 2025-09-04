using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private Button _startButton;
    [SerializeField] private Button _quitButton;

    private void Start()
    {
        StartMenuAnimation();
    }

    public void StartMenuAnimation()
    {
        // Position de base
        Vector3 startPos = _startButton.GetComponent<RectTransform>().localPosition;
        Vector3 quitPos = _quitButton.GetComponent<RectTransform>().localPosition;

        // Animation haut/bas Start
        _startButton.GetComponent<RectTransform>()
            .DOLocalMoveY(startPos.y + 20f, 1f)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);

        // Animation haut/bas Quit
        _quitButton.GetComponent<RectTransform>()
            .DOLocalMoveY(quitPos.y + 20f, 1f)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }
    public void OnStartButton()
    {
        // Charger la scène de jeu (à adapter selon votre gestion de scènes)
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
    }
    public void OnQuitButton()
    {
        Application.Quit();
    }
}