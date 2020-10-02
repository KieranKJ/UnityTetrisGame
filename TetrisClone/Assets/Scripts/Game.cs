using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Game : MonoBehaviour
{

    public static int gridWidth = 10;
    public static int gridHeight = 20;

    public static Transform[,] grid = new Transform[gridWidth, gridHeight];

    public static bool startingLevelZero;
    public static int startingLevel;


    public ParticleSystem scoreExplosion;

    public Canvas hud_canvas;
    public Canvas pause_canvas;

    public int scoreOneLine = 40;
    public int scoreTwoLine = 100;
    public int scoreThreeLine = 300;
    public int scoreFourLine = 1200;

    public int currentLevel = 0;
    private int numLinesCleared = 0;

    public static float fallSpeed = 1.0f;
    public static bool isPaused = false;

    public AudioClip clearedLineSound;

    public Text hud_score;
    public Text hud_Level;
    public Text hud_lines;

    private int numberOfRowsThisTurn = 0;

    private AudioSource audioSource;

    public static int currentScore =0;

    private GameObject previewTetromino;
    private GameObject nextTetromino;
    public GameObject savedTetromino;
    private GameObject ghostTetromino;

    private bool gameStarted = false;

    private int startingHighScore;
    private int startingHighScore2;
    private int startingHighScore3;

    private Vector2 previewTetrominoPosition = new Vector2(15f, 9f);
    private Vector2 savedTetrominoPosition = new Vector2(15f, 3f);

    public int maxSwaps = 2;
    private int currentSwaps = 0;



    // Start is called before the first frame update
    void Start()
    {
        currentScore = 0;

        hud_score.text = "0";
        
        currentLevel = startingLevel;

        hud_Level.text = currentLevel.ToString();

        hud_lines.text = "0";

        SpawnNextTetromino();

        audioSource = GetComponent<AudioSource>();

        startingHighScore = PlayerPrefs.GetInt("highscore");
        startingHighScore2 = PlayerPrefs.GetInt("highscore2");
        startingHighScore3 = PlayerPrefs.GetInt("highscore3");
    }

    void Update()
    {
        UpdateScore();
        UpdateUI();
        UpdateLevel();
        UpdateSpeed();
        CheckUserInput();
    }

    void CheckUserInput()
    {
        if (Input.GetKeyUp(KeyCode.P))
        {
            if (Time.timeScale == 1)
                PauseGame();
            else 
                ResumeGame();
           
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            GameObject tempNextTetromino = GameObject.FindGameObjectWithTag("currentActiveTetromino");
            SaveTetromino(tempNextTetromino.transform);
        }
    }
    void PauseGame()
    {
        Time.timeScale = 0;
        audioSource.Pause();
        isPaused = true;
        hud_canvas.enabled = false;
        pause_canvas.enabled = true;
    }

    void ResumeGame()
    {
        Time.timeScale = 1;
        isPaused = false;
        audioSource.Play();
        hud_canvas.enabled = true;
        pause_canvas.enabled = false;
    }

    void UpdateLevel()
    {
        if ((startingLevelZero == true) || (startingLevelZero == false && numLinesCleared / 10 > startingLevel))
        currentLevel = numLinesCleared / 10;
        
    }

    void UpdateSpeed()
    {
        fallSpeed = 1.0f - ((float)currentLevel * 0.1f);
       
    }

    public void UpdateUI()
    {
        hud_score.text = currentScore.ToString();
        hud_Level.text = currentLevel.ToString();
        hud_lines.text = numLinesCleared.ToString();
    }

    public void UpdateScore()
    {
        if (numberOfRowsThisTurn > 0)
        {
            if (numberOfRowsThisTurn == 1)
            {
                ClearedOneLine();

            } else if (numberOfRowsThisTurn == 2)
            {
                ClearedTwoLines();

            }
            else if (numberOfRowsThisTurn == 3)
            {
                ClearedThreeLines();

            }
            else if (numberOfRowsThisTurn == 4)
            {
                ClearedFourLines();

            }

            numberOfRowsThisTurn = 0;

            PlayLineClearedSound();
        }
    }

    public void ClearedOneLine ()
    {
        currentScore += scoreOneLine + (currentLevel * 20);
        numLinesCleared++; 
    }

    public void ClearedTwoLines()
    {
        currentScore += scoreTwoLine + (currentLevel * 25);
        numLinesCleared += 2;
    }
    public void ClearedThreeLines()
    {
        currentScore += scoreThreeLine + (currentLevel * 30);
        numLinesCleared += 3;
    }
    public void ClearedFourLines()
    {
        currentScore += scoreFourLine + (currentLevel * 50);
        numLinesCleared += 4;
    }

    public void PlayLineClearedSound()
    {
        audioSource.PlayOneShot(clearedLineSound);
    }

    public void UpdateHighScore()
    {
        if ( currentScore > startingHighScore){

            PlayerPrefs.SetInt("highscore3", startingHighScore2);
            PlayerPrefs.SetInt("highscore2", startingHighScore);
            PlayerPrefs.SetInt("highscore", currentScore);
        }
        else if (currentScore > startingHighScore2)
        {
            PlayerPrefs.SetInt("highscore3", startingHighScore2);
            PlayerPrefs.SetInt("highscore2", currentScore);
        } 
        else if (currentScore > startingHighScore3)
        {
            PlayerPrefs.SetInt("highscore3", currentScore);
        }

        PlayerPrefs.SetInt("lastScore", currentScore);
    }

    bool CheckIsValidPosition (GameObject tetromino)
    {
        foreach (Transform mino in tetromino.transform)
        {
            Vector2 pos = Round(mino.position);

            if (!CheckIsInsideGrid(pos))
            
            return    false;
            

            if (GetTransformAtGridPosition(pos) != null && GetTransformAtGridPosition(pos).parent != tetromino.transform)
                return false;
        }
        return true;
    }



    public bool CheckIsAboveGrid (Tetris tetris)
    {
        for (int x=0; x<gridWidth; ++x)
        {
            foreach(Transform mino in tetris.transform)
            {
                Vector2 pos = Round(mino.position);
                if (pos.y > gridHeight - 1)
                {
                    return true;
                }
            }
        }
        return false;
    }

   


public bool IsFullRowAt (int y)
    {
        for (int x =0; x < gridWidth; ++x)
        {
            if (grid[x,y] == null)
            {
                return false;
            }
        }
        //-since we found a full row, we increment the full row variable.
        numberOfRowsThisTurn++;
        return true;
    }



    public void  DeleteMinoAt (int y)
    {
        for (int x =0; x < gridWidth; ++x)
        {
            Destroy(grid[x, y].gameObject);
            grid[x, y] = null;
        }
    }

    public void MoveRowDown (int y)
    {
        for (int x=0; x<gridWidth; ++x)
        {
        if (grid[x,y] != null)
            {
                grid[x, y - 1] = grid[x, y];

                grid[x, y] = null;

                grid[x, y - 1].position += new Vector3(0, -1, 0);
            }
        }
    }
    public void MoveAllRowsDown (int y)
    {
        for (int i=y; i < gridHeight; ++i)
        {
            MoveRowDown(i);
        }
    }

    public void DeleteRow ()
    {
        for (int y=0; y< gridHeight; ++y)
        {
            if (IsFullRowAt(y))
            {
                
                
                DeleteMinoAt(y);

                MoveAllRowsDown(y + 1);
                --y;

            }
        }
    }

    public void UpdateGrid(Tetris tetris)
    {
        for (int y=0; y < gridHeight; ++ y)
        {
            for (int x=0; x<gridWidth; ++x)
            {
                if (grid [x, y] != null)
                {
                    if (grid[x,y].parent == tetris.transform)
                    {
                        grid[x, y] = null;
                    }
                }
            }
        }
        foreach (Transform mino in tetris.transform)
        {
            Vector2 pos = Round(mino.position);
            if (pos.y < gridHeight)
            {
                grid[(int)pos.x, (int)pos.y] = mino;
            }
        }
    }

    public Transform GetTransformAtGridPosition (Vector2 pos)
    {
        if (pos.y > gridHeight - 1)
        {
            return null;
        } else
        {
            return grid[(int)pos.x, (int)pos.y];
        }
    }




    public void SpawnNextTetromino ()
    {
        if (!gameStarted)
        {
            gameStarted = true;
            nextTetromino = (GameObject)Instantiate(Resources.Load(GetRandomTetromino(), typeof(GameObject)), new Vector2(5.0f, 20.0f), Quaternion.identity);
            previewTetromino = (GameObject)Instantiate(Resources.Load(GetRandomTetromino(), typeof(GameObject)), previewTetrominoPosition, Quaternion.identity);
            previewTetromino.GetComponent<Tetris>().enabled = false;
            nextTetromino.tag = "currentActiveTetromino";

            SpawnGhostTetromino();
        }
        else
        {
            previewTetromino.transform.localPosition = new Vector2(5.0f, 20.0f);
            nextTetromino = previewTetromino;
            nextTetromino.GetComponent<Tetris>().enabled = true;
            nextTetromino.tag = "currentActiveTetromino";
            previewTetromino = (GameObject)Instantiate(Resources.Load(GetRandomTetromino(), typeof(GameObject)), previewTetrominoPosition, Quaternion.identity);
            previewTetromino.GetComponent<Tetris>().enabled = false;

            SpawnGhostTetromino();
        }
        currentSwaps = 0;
    }

    public void SpawnGhostTetromino()
    {
        if (GameObject.FindGameObjectWithTag ("currentGhostTetromino") != null)
        
        Destroy(GameObject.FindGameObjectWithTag("currentGhostTetromino"));

        ghostTetromino = (GameObject)Instantiate (nextTetromino, nextTetromino.transform.position, Quaternion.identity);

        Destroy(ghostTetromino.GetComponent<Tetris>());
        ghostTetromino.AddComponent<GhostTetromino>();
    }
    public void SaveTetromino (Transform t)
    {
        currentSwaps++;
        if (currentSwaps > maxSwaps)
            return;

        if (savedTetromino != null)
        {
            //-there is currently a tetromino being held
            GameObject tempSavedTetromino = GameObject.FindGameObjectWithTag("currentSavedTetromino");
            tempSavedTetromino.transform.localPosition = new Vector2 (gridWidth / 2, gridHeight);

            if(!CheckIsValidPosition(tempSavedTetromino))
            {
                tempSavedTetromino.transform.localPosition = savedTetrominoPosition;
                return;
            }

            savedTetromino = (GameObject)Instantiate(t.gameObject);
            savedTetromino.GetComponent<Tetris>().enabled = false;
            savedTetromino.transform.localPosition = savedTetrominoPosition;
            savedTetromino.tag = "currentSavedTetromino";

            nextTetromino = (GameObject)Instantiate(tempSavedTetromino);
            nextTetromino.GetComponent<Tetris>().enabled = true;
            nextTetromino.transform.localPosition = new Vector2(gridWidth / 2, gridHeight);
            nextTetromino.tag = "currentActiveTetromino";

            DestroyImmediate(t.gameObject);
            DestroyImmediate(tempSavedTetromino);

            SpawnGhostTetromino();

        } else
        {
            //- there is currently no tetromino being held
            savedTetromino = (GameObject)Instantiate(GameObject.FindGameObjectWithTag("currentActiveTetromino"));
            savedTetromino.GetComponent<Tetris>().enabled = false;
            savedTetromino.transform.localPosition = savedTetrominoPosition;
            savedTetromino.tag = "currentSavedTetromino";

            DestroyImmediate(GameObject.FindGameObjectWithTag("currentActiveTetromino"));

            SpawnNextTetromino();

        }
    }
    public bool CheckIsInsideGrid (Vector2 pos)
    {
        return ((int)pos.x >= 0 && (int)pos.x < gridWidth && (int)pos.y >= 0);
    }

    public Vector2 Round (Vector2 pos)
    {
        return new Vector2 (Mathf.Round(pos.x), Mathf.Round(pos.y));
    }

    string GetRandomTetromino()
    {
        int randomTetromino = Random.Range(1, 8);
        string randomTetrominoName = "tetromino_T";

        switch (randomTetromino)
        {
            case 1:
                randomTetrominoName = "Prefabs/TetrisBlock_T";
                    break;

            case 2:
                randomTetrominoName = "Prefabs/TetrisBlock_L";
                    break;
            case 3:
                randomTetrominoName = "Prefabs/TetrisBlock_Long";
                    break;

            case 4:
                randomTetrominoName = "Prefabs/TetrisBlock_S";
                    break;
            case 5:
                randomTetrominoName = "Prefabs/TetrisBlock_Square";
                    break;

            case 6:
                randomTetrominoName = "Prefabs/TetrisBlock_Z";
                    break;
            case 7:
                randomTetrominoName = "Prefabs/TetrisJBlock";
                    break;
        }

        return randomTetrominoName;
    }

    public void GameOver()
    {
        UpdateHighScore();
        SceneManager.LoadScene("GameOver");
    }
}
