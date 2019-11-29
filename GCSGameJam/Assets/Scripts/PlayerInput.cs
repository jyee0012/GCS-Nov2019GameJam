using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerInput : MonoBehaviour
{
    [SerializeField]
    AudioSource bgMusic = null;
    [SerializeField]
    GameObject trail = null;
    [SerializeField]
    Text scoreText = null;
    
    public int Score = 0;
    public int MaxEnemiesAllowed = 50;
    [SerializeField]
    bool gameOver = false;
    [SerializeField]
    EnemySpawner spawner = null;
    [SerializeField]
    GameObject MainGameplayPanel = null;
    [SerializeField]
    GameObject GameOverPanel = null;
    [SerializeField]
    TextMeshProUGUI GameOverScore = null;

    [SerializeField]
    AudioClip tapSound = null;
    [SerializeField]
    float fadeTime = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        if (spawner == null)
            spawner = FindObjectOfType<EnemySpawner>();

        if (spawner != null)
            spawner.enemyCountSlider.maxValue = MaxEnemiesAllowed;
    }

    // Update is called once per frame
    void Update()
    {
        //if (trail != null) trail.transform.position = Input.mousePosition;

        if (!gameOver)
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            if (Input.GetKeyDown(KeyCode.Mouse0))
                ClickEnemy(Input.mousePosition);
#endif

#if UNITY_ANDROID
            foreach (Touch touch in Input.touches)
            {
                if (touch.phase == TouchPhase.Began)
                    ClickEnemy(touch.position);
            }
#endif
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
#if UNITY_ANDROID
            SceneManager.LoadSceneAsync("MenuScene");
#endif
#if UNITY_EDITOR || UNITY_STANDALONE
            SceneManager.LoadSceneAsync("MenuScenePC");
#endif
        }

        if (spawner != null && spawner.enemyCount >= MaxEnemiesAllowed)
            LoseGame();
    }

    public EnemyAI[] FindAllEnemies()
    {
        EnemyAI[] enemies = FindObjectsOfType<EnemyAI>();
        return enemies;
    }

    public void DestroyAllEnemy(EnemyAI[] enemies)
    {
        foreach (EnemyAI enemy in enemies)
            Destroy(enemy.gameObject);
    }

    public void ClickEnemy(Vector2 input)
    {
        Vector2 point = Camera.main.ScreenToWorldPoint(input);
        RaycastHit2D hitInfo = Physics2D.Raycast(point, Vector2.zero, 30.0f, LayerMask.GetMask("EnemyLayer"));

        if (hitInfo.collider != null)
        {
            EnemyAI enemy = hitInfo.collider.GetComponent<EnemyAI>();

            if (enemy != null && !enemy.isSpawning)
            {
                enemy.DamageEnemy();
                AddScore(1 * enemy.scoreMultiplier);
            }
        }
    }

    public void AddScore(int amount)
    {
        Score += amount;
        scoreText.text = "" + Score;
    }

    public void LoseGame()
    {
        //Stop spawning enemies
        if (spawner != null)
            spawner.gamePlaying = false;

        StartCoroutine(FadeAudio());

        //Deactivate Main Gameplay panel
        if (MainGameplayPanel != null)
            MainGameplayPanel.SetActive(false);

        //Display the GameOver Screen
        if (GameOverPanel != null)
            GameOverPanel.SetActive(true);

        //Set the final score on the Game Over Screen
        if (GameOverScore != null)
            GameOverScore.SetText("Total Score: " + Score);

        gameOver = true;
        DestroyAllEnemy(FindAllEnemies());
    }

    IEnumerator FadeAudio()
    {
        float endFade = Time.time + fadeTime;

        while (bgMusic.volume > 0)
        {
            float lerpAmount = (endFade - Time.time) / fadeTime;
            bgMusic.volume = lerpAmount;

            yield return new WaitForEndOfFrame();
        }

        yield break;
    }
}
