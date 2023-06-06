using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    public int enemiesAlive = 0;
    public float countdownTime = 5f; // Adjust the countdown time as desired
    private bool isCountdownActive = false;
    public int round = 0;
    public float baseHealth = 50f;
    public float healthIncreasePerRound = 5f;
    public GameObject[] spawnPoints;

    public GameObject enemyPrefab;
    public GameObject bossEnemyPrefab;
    //public GameObject pauseMenu;

    public TextMeshProUGUI roundNum;
    public TextMeshProUGUI roundsSurvived;
    //public GameObject endScreen;

    //public Animator blackScreenAnimator;

    // Start is called before the first frame update
    void Start() {
    }

    // Update is called once per frame
    void Update()
    {
        if (enemiesAlive == 0)
        {
            if (!isCountdownActive)
            {
                isCountdownActive = true;
                StartCoroutine(StartCountdown());
            }
        }
    }

    IEnumerator StartCountdown()
    {
        while (countdownTime > 0)
        {
            roundNum.text = "Next Round in: " + countdownTime.ToString("0");
            yield return new WaitForSeconds(1f);
            countdownTime--;
        }

        round++;
        NextWave(round);
        roundNum.text = "Round: " + round.ToString();
        isCountdownActive = false;
        countdownTime = 5f; // Reset the countdown time for the next round
    }
    public void NextWave(int round)
    {
        int enemyCount = Mathf.RoundToInt(Mathf.Pow(round, 1.5f)); // Exponential growth formula

        if (round == 2)
        {
            // Spawn the boss enemy
            GameObject bossSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            GameObject bossSpawned = Instantiate(bossEnemyPrefab, bossSpawnPoint.transform.position, Quaternion.identity);
            bossSpawned.GetComponent<EnemyManager>().gameManager = GetComponent<GameManager>();
            enemiesAlive++;
        }
        else
        {
            for (int i = 0; i < enemyCount; i++)
            {
                GameObject spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

                GameObject enemySpawned = Instantiate(enemyPrefab, spawnPoint.transform.position, Quaternion.identity);
                enemySpawned.GetComponent<EnemyManager>().gameManager = GetComponent<GameManager>();
                enemiesAlive++;
            }
        }
    }

    //public void EndGame() {
    //    Time.timeScale = 0;
    //    Cursor.lockState = CursorLockMode.None;
    //    endScreen.SetActive(true);
    //    roundsSurvived.text = round.ToString();
    //}

    //public void ReplayGame() {
    //    SceneManager.LoadScene(1);
    //    Time.timeScale = 1;
    //    round = 0;
    //}

    //public void MainMenu() {
    //    Time.timeScale = 1; 
    //    AudioListener.volume = 1;
    //    blackScreenAnimator.SetTrigger("FadeIn");
    //    Invoke("LoadMainMenuScene", .4f);
    //}

    //void LoadMainMenuScene() {
    //    SceneManager.LoadScene(0);
    //}

    //public void Pause() {
    //    pauseMenu.SetActive(true);
    //    Time.timeScale = 0;
    //    Cursor.lockState = CursorLockMode.None;
    //    AudioListener.volume = 0;
    //}

    //public void UnPause() {
    //    pauseMenu.SetActive(false);
    //    Time.timeScale = 1;
    //    Cursor.lockState = CursorLockMode.Locked;
    //    AudioListener.volume = 1;
    //}
    public float GetMaxHealth()
    {
        int currentRound = round;
        return baseHealth + (healthIncreasePerRound * (currentRound-1));
    }
}
