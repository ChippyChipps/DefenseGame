using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI : MonoBehaviour
{
    public static UI instance;

    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI killCounterText;
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private GameObject tutorialUI;
    [SerializeField] private float tutorialDuration = 5f;

    private int killCount;

    private void Awake()
    {
        instance = this;
        Time.timeScale = 1.0f;
    }

    private void Start()
    {
        tutorialUI.SetActive(true);

        StartCoroutine(HideTutorial());
    }

    private void Update()
    {
        timerText.text = Time.time.ToString("F2") + "s";
    }

    public void AddKillCount()
    {
        killCount++;
        killCounterText.text = killCount.ToString();
    }

    public void EnabledGameOverUI()
    {
        Time.timeScale = .5f;
        gameOverUI.SetActive(true);
    }

    public void RestartLevel()
    {
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(sceneIndex);
    }

    private IEnumerator HideTutorial()
    {
        yield return new WaitForSeconds(tutorialDuration);

        tutorialUI.SetActive(false);
    }
}
