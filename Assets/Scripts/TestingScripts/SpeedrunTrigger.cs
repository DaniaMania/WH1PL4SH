using TMPro;
using UnityEngine;

public class SpeedrunTrigger : MonoBehaviour
{
    public static SpeedrunTrigger Instance { get; private set; }

    [Header("Timer Reference")]
    [SerializeField] private TMP_Text speedrunTimer;

    [Header("Result Panel")]
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private TMP_Text finalTime;
    [SerializeField] private GameObject newBestGroup;
    [SerializeField] private GameObject bestTimeGroup;
    [SerializeField] private TMP_Text bestTimeText;

    private const string BestTimeKey = "SpeedrunBestTime";

    private bool _hasStarted;
    private bool _hasFinished;
    private float _startTime;
    private float _finalTime;

    private void Awake()
    {
        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    private void Start()
    {
        HideResults();
        UpdateDisplay(0f);
    }

    private void Update()
    {
        float elapsed = !_hasStarted ? 0f : _hasFinished ? _finalTime : Time.time - _startTime;
        UpdateDisplay(elapsed);
    }

    public static void ResetTimer()
    {
        if (Instance != null) Instance.ResetRun();
    }

    private void ResetRun()
    {
        _hasStarted = false;
        _hasFinished = false;
        _startTime = 0f;
        _finalTime = 0f;
        HideResults();
        UpdateDisplay(0f);
    }

    public void StartRun()
    {
        if (_hasStarted) return;
        _hasStarted = true;
        _startTime = Time.time;
        Debug.Log("Race Started!");
    }

    public void FinishRun()
    {
        if (!_hasStarted || _hasFinished) return;
        _hasFinished = true;
        _finalTime = Time.time - _startTime;
        UpdateDisplay(_finalTime);

        float best = PlayerPrefs.GetFloat(BestTimeKey, float.MaxValue);
        bool isNewBest = _finalTime < best;
        if (isNewBest)
        {
            PlayerPrefs.SetFloat(BestTimeKey, _finalTime);
            PlayerPrefs.Save();
        }

        Debug.Log($"Race Finished! Time: {FormatTime(_finalTime)}{(isNewBest ? " (New Best)" : "")}");

        if (finalTime != null) finalTime.text = FormatTime(_finalTime);

        if (newBestGroup != null) newBestGroup.SetActive(isNewBest);
        if (bestTimeGroup != null) bestTimeGroup.SetActive(!isNewBest);
        if (!isNewBest && bestTimeText != null) bestTimeText.text = FormatTime(best);

        if (resultPanel != null) resultPanel.SetActive(true);
    }

    private void HideResults()
    {
        if (resultPanel != null) resultPanel.SetActive(false);
        if (newBestGroup != null) newBestGroup.SetActive(false);
        if (bestTimeGroup != null) bestTimeGroup.SetActive(false);
    }

    private void UpdateDisplay(float time)
    {
        if (speedrunTimer != null) speedrunTimer.text = FormatTime(time);
    }

    private static string FormatTime(float time)
    {
        int minutes = (int)(time / 60f);
        int seconds = (int)(time % 60f);
        int centiseconds = (int)((time * 100f) % 100f);
        return $"{minutes:00}:{seconds:00}:{centiseconds:00}";
    }
}