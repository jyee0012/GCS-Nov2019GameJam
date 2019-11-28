using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerInput : MonoBehaviour
{
    [SerializeField]
    GameObject trail = null;
    [SerializeField]
    Text scoreText = null;
    
    public int Score = 0;
    [SerializeField]
    EnemySpawner spawner = null;
    [SerializeField]
    GameObject GameOverPanel = null;
    [SerializeField]
    TextMeshProUGUI GameOverScore = null;

    // Start is called before the first frame update
    void Start()
    {
        if (spawner == null)
            spawner = FindObjectOfType<EnemySpawner>();
    }

    // Update is called once per frame
    void Update()
    {
        if (trail != null) trail.transform.position = Input.mousePosition;
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

        if (Input.GetKeyDown(KeyCode.Escape))
            SceneManager.LoadSceneAsync("MenuScene");

        if (spawner != null && spawner.enemyCount >= 50)
            LoseGame();
    }

    //public int FindAllEnemies()
    //{
    //    return FindObjectsOfType<EnemyAI>().Length;
    //}

    public void ClickEnemy(Vector2 input)
    {
        Vector2 point = Camera.main.ScreenToWorldPoint(input);
        RaycastHit2D hitInfo = Physics2D.Raycast(point, Vector2.zero, 30.0f, LayerMask.GetMask("EnemyLayer"));

        if (hitInfo.collider != null)
        {
            EnemyAI enemy = hitInfo.collider.GetComponent<EnemyAI>();

            if (enemy != null && !enemy.isSpawning)
            {
                enemy.KillEnemy();
                AddScore(1);
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

        //Display the GameOver Screen
        if (GameOverPanel != null)
            GameOverPanel.SetActive(true);

        //Set the final score on the Game Over Screen
        if (GameOverScore != null)
            GameOverScore.SetText("Total Score: " + Score);
    }
}
