using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TimerManager : MonoBehaviour
{
    public static TimerManager instance;

    [Header("Time Thresholds")]
    public float startingTime;
    public float bronzeScoreTime;
    public float silverScoreTime;
    public float goldScoreTime;

    private float currentTime;
    private bool isTimerPaused;
    private string recievedMedal;

    [Header("UI")]
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI completionTimeText;
    public TextMeshProUGUI medalText;
    public GameObject completionText;
    public TextMeshProUGUI continueText;

    public TextMeshProUGUI bronzeTimeText;
    public TextMeshProUGUI silverTimeText;
    public TextMeshProUGUI goldTimeText;

    [Header("Colors")]
    public Color goldColor;
    public Color silverColor;
    public Color bronzeColor;
    public Color noMedalColor;

    public string finalLevelName = "Level3";

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        completionText.SetActive(false);
        StartTimer();

        bronzeTimeText.text = "Bronze Time: " + bronzeScoreTime;
        bronzeTimeText.color = bronzeColor;

        silverTimeText.text = "Silver Time: " + silverScoreTime;
        silverTimeText.color = silverColor;

        goldTimeText.text = "Gold Time: " + goldScoreTime;
        goldTimeText.color = goldColor;
    }

    void Update()
    {
        if (!isTimerPaused)
        {
            currentTime -= Time.deltaTime;
            //ToString("F1") rounds the time to the first decimal place
            timerText.text = "Time: " + currentTime.ToString("F1");
        }

        if (currentTime <= 0)
        {
            PauseTimer();
            SceneManagment.instance.ReloadCurrentScene();
        }
    }

    public void StartTimer()
    {
        isTimerPaused = false;
        currentTime = startingTime;
    }

    public void PauseTimer()
    {
        isTimerPaused = !isTimerPaused;
    }

    public void CompleteLevel()
    {
        isTimerPaused = true;

        if (currentTime >= goldScoreTime)
        {
            Debug.Log("Completed level: Gold medal recieved");
            recievedMedal = "Gold";
            medalText.color = goldColor;
            completionTimeText.color = goldColor;
        } else if (currentTime >= silverScoreTime)
        {
            Debug.Log("Completed level: Silver medal recieved");
            recievedMedal = "Silver";
            medalText.color = silverColor;
            completionTimeText.color = silverColor;

        } else if (currentTime >= bronzeScoreTime)
        {
            Debug.Log("Completed level: Bronze medal recieved");
            recievedMedal = "Bronze";
            medalText.color = bronzeColor;
            completionTimeText.color = bronzeColor;
        } else
        {
            Debug.Log("Completed level: No medal recieved");
            medalText.color = noMedalColor;
            completionTimeText.color = noMedalColor;
            recievedMedal = "None";
        }

        if (SceneManager.GetActiveScene().name == finalLevelName)
        {
            continueText.text = "All levels completed! Press Escape To Return To Title";
        } else
        {
            continueText.text = "Press Enter To Continue";
        }

        completionText.SetActive(true);
        completionTimeText.text = "Final Time: " + currentTime.ToString("F1");
        medalText.text = " Medal Recieved: " + recievedMedal;

    }
}
