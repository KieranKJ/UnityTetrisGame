using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameMenu : MonoBehaviour
{

    public AudioClip StartGameClip;

    public Text highScoreText;
    public Text highScoreText2;
    public Text highScoreText3;
    public Text levelText;

    public Text lastScore;

    private AudioSource audioSource;


    
    // Start is called before the first frame update
    void Start()
    {

        if (levelText != null)
        levelText.text = "0";
        
        if (highScoreText != null)
        highScoreText.text = PlayerPrefs.GetInt("highscore").ToString();

        if (highScoreText2 != null)
        highScoreText2.text = PlayerPrefs.GetInt("highscore2").ToString();

        if (highScoreText3 != null)
         highScoreText3.text = PlayerPrefs.GetInt("highscore3").ToString();

        if (lastScore != null)
            lastScore.text = PlayerPrefs.GetInt("lastScore").ToString();

        audioSource = GetComponent<AudioSource>();

    }




    public void PlayGame()
    {
        if (Game.startingLevel == 0)
            Game.startingLevelZero = true;
        else
            Game.startingLevelZero = false;
        SceneManager.LoadScene("Game");
        //PlayStartSound();
    }
    //public void PlayStartSound()
    //{
    //    audioSource.PlayOneShot(StartGameClip);

    //}


    public void QuitGame()
    {
        Application.Quit();
    }
        public void openHighScores()
    {
        
    }

    public void ChangedValue (float value)
    {
        Game.startingLevel = (int)value;
        levelText.text = value.ToString();
    }

    public void LaunchGameMenu()
    {
        SceneManager.LoadScene("GameMenu");
    }


}
