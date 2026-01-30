using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Timer")]
    public TMP_Text timerText;

    [Header("Hearts")]
    public Image[] hearts; // 3つ入れる

    [Header("Result")]
    public GameObject resultPanel;
    public TMP_Text resultText;

    public void BindPlayer(PlayerHealth health)
    {
        if (health == null) return;
        health.OnHpChanged += (cur, max) => SetHearts(cur);
        SetHearts(health.CurrentHp);
    }

    public void SetTime(float timeLeft)
    {
        if (timerText == null) return;

        timeLeft = Mathf.Max(0f, timeLeft);

        // 残り1秒が見える方が自然なので切り上げ
        int totalSeconds = Mathf.CeilToInt(timeLeft);

        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;

        timerText.text = $"TIME {minutes:00}:{seconds:00}";
    }


    public void SetHearts(int current)
    {
        if (hearts == null) return;
        for (int i = 0; i < hearts.Length; i++)
            hearts[i].enabled = (i < current);
    }

    public void ShowResult(bool win)
    {
        if (resultPanel != null) resultPanel.SetActive(true);
        if (resultText != null) resultText.text = win ? "CLEAR!" : "GAME OVER";
    }
}
