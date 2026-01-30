using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public float timeLimit = 60f;

    public UIManager ui;
    public PlayerHealth playerHealth;

    float timeLeft;
    bool ended;

    void Start()
    {
        timeLeft = timeLimit;

        if (playerHealth != null) playerHealth.OnDied += Lose;
        if (ui != null) ui.BindPlayer(playerHealth);

    }

    void Update()
    {
        // 終了後もリスタートだけは受け付ける
        if (ended)
        {
            if (Input.GetKeyDown(KeyCode.R))
                Restart();
            return;
        }

        timeLeft -= Time.deltaTime;
        if (ui != null) ui.SetTime(timeLeft);

        if (timeLeft <= 0f)
            Win();

        if (Input.GetKeyDown(KeyCode.R))
            Restart();
    }


    void Win()
    {
        ended = true;
        if (ui != null) ui.ShowResult(true);
        Time.timeScale = 0f;
    }

    void Lose()
    {
        ended = true;
        if (ui != null) ui.ShowResult(false);
        Time.timeScale = 0f;
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
