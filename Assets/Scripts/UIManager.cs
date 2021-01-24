using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Text scoreText;
    [SerializeField] private Text ammoCountText;
    [SerializeField] private Sprite[] livesSprites;
    [SerializeField] private Image livesImage;
    [SerializeField] private Text gameOverText;
    [SerializeField] private Text restartText;
    [SerializeField] private GameManager gameManager;


    // Start is called before the first frame update
    void Start()
    {
        scoreText.text = "Score: 0";
        gameOverText.gameObject.SetActive(false);
        restartText.gameObject.SetActive(false);

        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        if (gameManager == null)
        {
            Debug.Log("Game Manager is null!");
        }
    }

    public void UpdateScoreText(int currentScore)
    {
        scoreText.text = "Score: " + currentScore;
    }
    
    public void UpdateLivesImage(int currentLives)
    {
        if(currentLives < 0)
        {
            currentLives = 0;
        }
        livesImage.sprite = livesSprites[currentLives];

        if(currentLives == 0)
        {
            GameOverSequence();
        }
    }

    public void UpdateAmmoCount(int currentAmmoCount)
    {
        string ammoText = "Ammo: ";

        if (currentAmmoCount == 0)
        {
            ammoText += " EMPTY ";
        }
        else
        {            
            for (int i = 0; i < currentAmmoCount; i++)
            {
                ammoText += "I";
            }
        }

        ammoCountText.text = ammoText;
    }

    IEnumerator GameOverFlickerRoutine()
    {
        while (true)
        {
            gameOverText.text = "GAME OVER";
            yield return new WaitForSeconds(0.5f);
            gameOverText.text = "";
            yield return new WaitForSeconds(0.5f);
        }
    }

    void GameOverSequence()
    {
        gameManager.GameOver();
        gameOverText.gameObject.SetActive(true);
        restartText.gameObject.SetActive(true);
        StartCoroutine(GameOverFlickerRoutine());
    }
    
}
