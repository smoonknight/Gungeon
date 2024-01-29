using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    public GameObject player;
    public LayerMask playerMask;
    public LayerMask enemyMask;
    public RectTransform operateUI;
    public Transform PlayerTransform => player.transform;

    private void Start()
    {
        AudioManager.Instance.Play("Music");
    }

    LTDescr lTDescr(CanvasGroup canvas)
    {
        return LeanTween.alphaCanvas(canvas, 1, 1);
    }

    public void Win()
    {
        operateUI.gameObject.SetActive(true);
        CanvasGroup canvasGroup = operateUI.GetComponent<CanvasGroup>();
        lTDescr(canvasGroup);

        operateUI.Find("Condition Text").GetComponent<TextMeshProUGUI>().text = "Menang";
        
    }
    public void Lose()
    {
        operateUI.gameObject.SetActive(true);
        CanvasGroup canvasGroup = operateUI.GetComponent<CanvasGroup>();
        lTDescr(canvasGroup);

        operateUI.Find("Condition Text").GetComponent<TextMeshProUGUI>().text = "Kalah";
    }

    public void Restart() => SceneManager.LoadScene("SampleScene");
}