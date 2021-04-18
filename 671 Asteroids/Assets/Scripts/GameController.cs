using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{

    public GameObject asteroid;

    private int score;
    private int hiscore;
    private int asteroidsRemaining;
    private int lives;
    private int wave;
    private int increaseEachWave = 4;

    private float timeLeft = 5.0f;

    public Text scoreText;
    public Text livesText;
    public Text waveText;
    public Text hiscoreText;



    // Use this for initialization
    void Start()
    {

        hiscore = PlayerPrefs.GetInt("hiscore", 0);
        BeginGame();
    }

    // Update is called once per frame
    void Update()
    {

        // Quit if player presses escape
        if (Input.GetKey("escape"))
            Application.Quit();

        timeLeft -= Time.deltaTime;

        if(timeLeft < 0)
        {
            //Play Ambience
            FMODUnity.RuntimeManager.PlayOneShot("event:/Ambience/Ambience");
            Debug.Log("Ambience Playing");
            timeLeft = 30.0f;
            

        }
    }

    void BeginGame()
    {

        score = 0;
        lives = 3;
        wave = 1;

        // Prepare the HUD
        scoreText.text = "SCORE:" + score;
        hiscoreText.text = "HISCORE: " + hiscore;
        livesText.text = "LIVES: " + lives;
        waveText.text = "WAVE: " + wave;

        FMODUnity.RuntimeManager.PlayOneShot("event:/VO/StartVO");


        SpawnAsteroids();
    }

    void SpawnAsteroids()
    {

        DestroyExistingAsteroids();

        // Decide how many asteroids to spawn
        // If any asteroids left over from previous game, subtract them
        asteroidsRemaining = (wave * increaseEachWave);

        for (int i = 0; i < asteroidsRemaining; i++)
        {

            // Spawn an asteroid
            Instantiate(asteroid,
                new Vector3(Random.Range(-9.0f, 9.0f),
                    Random.Range(-6.0f, 6.0f), 0),
                Quaternion.Euler(0, 0, Random.Range(-0.0f, 359.0f)));

        }

        waveText.text = "WAVE: " + wave;
    }

    public void IncrementScore()
    {
        score++;
        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/ScoreGain");
        scoreText.text = "SCORE:" + score;

        if (score > hiscore)
        {
            hiscore = score;
            hiscoreText.text = "HISCORE: " + hiscore;

            // Save the new hiscore
            PlayerPrefs.SetInt("hiscore", hiscore);
        }

        // Has player destroyed all asteroids?
        if (asteroidsRemaining < 1)
        {

            // Start next wave
            wave++;
            SpawnAsteroids();

        }
    }

    public void DecrementLives()
    {
        lives--;
        livesText.text = "LIVES: " + lives;

        // Has player run out of lives?
        if (lives < 1)
        {
            // Restart the game
            BeginGame();
        }
    }

    public void DecrementAsteroids()
    {
        asteroidsRemaining--;
    }

    public void SplitAsteroid()
    {
        // Two extra asteroids
        // - big one
        // + 3 little ones
        // = 2
        asteroidsRemaining += 2;
        //FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/LgAsteroidBreak");

    }

    void DestroyExistingAsteroids()
    {
        GameObject[] asteroids =
            GameObject.FindGameObjectsWithTag("Large Asteroid");

        foreach (GameObject current in asteroids)
        {
            GameObject.Destroy(current);
        }

        GameObject[] asteroids2 =
            GameObject.FindGameObjectsWithTag("Small Asteroid");

        foreach (GameObject current in asteroids2)
        {
            GameObject.Destroy(current);
        }
    }

}
