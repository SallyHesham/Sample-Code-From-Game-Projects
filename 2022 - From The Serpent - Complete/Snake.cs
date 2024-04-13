using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snake : MonoBehaviour
{
    private Vector2Int headPosition;
    private Vector2Int headLastPosition;
    private Vector2Int gridMoveDir;
    private Vector2Int lastGridMoveDir;
    private float gridMoveTimer;
    private float gridMoveTimerMax;
    private int gridMoveUnit;
    private LevelGrid levelGrid;
    private int snakeSize;
    private List<Vector2Int> bodyPartsLocs;
    private List<GameObject> bodyPartsObjs;
    private int colorIndex;
    private int nextColorIndex;
    private bool alive;
    private bool pause;
    private Vector2Int upDir;
    private Vector2Int downDir;
    private Vector2Int rightDir;
    private Vector2Int leftDir;
    private Audio audioPlayer;
    public GameObject bodyPrefab;
    public GameObject tail;

    public void Setup(LevelGrid level)
    {
        levelGrid = level;
        Score.Setup();
    }

    private void Awake()
    {
        gridMoveTimerMax = GameAssets.Instance.moveTimerMax;
        gridMoveUnit = GameAssets.Instance.moveUnit;
        headPosition = new Vector2Int(0, 0);
        headLastPosition = new Vector2Int(0, 0);
        gridMoveTimer = gridMoveTimerMax;
        gridMoveDir = new Vector2Int(0, 0);
        lastGridMoveDir = new Vector2Int(0, 0);
        snakeSize = 0;
        bodyPartsLocs = new List<Vector2Int>();
        bodyPartsObjs = new List<GameObject>();
        colorIndex = 0;
        nextColorIndex = 0;
        alive = true;
        pause = false;
        upDir = new Vector2Int(0, gridMoveUnit);
        downDir = new Vector2Int(0, -gridMoveUnit);
        rightDir = new Vector2Int(gridMoveUnit, 0);
        leftDir = new Vector2Int(-gridMoveUnit, 0);
        audioPlayer = GameAssets.Instance.AudioPlayer;
        transform.position = new Vector3(headPosition.x, headPosition.y);
        transform.eulerAngles = new Vector3(0, 0, GetAngleFromVector(gridMoveDir) - 90);
        tail.transform.position = new Vector3(headPosition.x, headPosition.y);
        tail.transform.eulerAngles = new Vector3(0, 0, GetAngleFromVector(gridMoveDir) - 90);
    }

    // Update is called once per frame
    private void Update()
    {
        if (alive && !pause)
        {
            HandleInput();
            HandleMovement();
        }
    }

    // handles input from user
    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            audioPlayer.PlayUpKhiahSound();
            if (headPosition + upDir != headLastPosition)
            {
                gridMoveDir.x = 0;
                gridMoveDir.y = gridMoveUnit;
            }
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            audioPlayer.PlayDownKhiahSound();
            if (headPosition + downDir != headLastPosition)
            {
                gridMoveDir.x = 0;
                gridMoveDir.y = -gridMoveUnit;
            }
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            audioPlayer.PlayRightKhiahSound();
            if (headPosition + rightDir != headLastPosition)
            {
                gridMoveDir.x = gridMoveUnit;
                gridMoveDir.y = 0;
            }
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            audioPlayer.PlayLeftKhiahSound();
            if (headPosition + leftDir != headLastPosition)
            {
                gridMoveDir.x = -gridMoveUnit;
                gridMoveDir.y = 0;
            }
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            audioPlayer.PlayStateKhiahSound();
            nextColorIndex = (colorIndex + 1) % GameAssets.Instance.colors.Count;
        }
    }

    // handles movement and size
    private void HandleMovement()
    {
        gridMoveTimer += Time.deltaTime;
        if (gridMoveTimer >= gridMoveTimerMax)
        {
            headLastPosition = headPosition;
            headPosition += gridMoveDir;
            gridMoveTimer -= gridMoveTimerMax;
            bodyPartsLocs.Insert(0, headLastPosition);
            headPosition = levelGrid.WrapCoordinates(headPosition);
            colorIndex = nextColorIndex;
            gameObject.GetComponent<SpriteRenderer>().material = GameAssets.Instance.colors[colorIndex];

            bool snakeAte = levelGrid.TrySnakeEatFood(headPosition);
            if (snakeAte)
            {
                Score.CoreScore(snakeSize);
                snakeSize++;
                CreateBodyPart();
                audioPlayer.PlayEatSound();
            }
            else
            {
                //for tail
                tail.transform.position = new Vector3Int(bodyPartsLocs[bodyPartsLocs.Count-1].x,
                    bodyPartsLocs[bodyPartsLocs.Count-1].y, 0);
                tail.transform.eulerAngles = gameObject.transform.eulerAngles;

                bodyPartsLocs.RemoveAt(bodyPartsLocs.Count - 1);
                if (snakeSize > 0)
                {
                    GameObject lastObj = bodyPartsObjs[bodyPartsObjs.Count - 1];
                    bodyPartsObjs.RemoveAt(bodyPartsObjs.Count - 1);
                    bodyPartsObjs.Insert(0, lastObj);

                    //tail
                    tail.transform.eulerAngles = lastObj.transform.eulerAngles;
                }
            }

            transform.position = new Vector3(headPosition.x, headPosition.y);
            transform.eulerAngles = new Vector3(0, 0, GetAngleFromVector(gridMoveDir) - 90);
            

            if (snakeSize > 0)
            {
                bodyPartsObjs[0].transform.position = new Vector3(headLastPosition.x, headLastPosition.y);
                bodyPartsObjs[0].GetComponent<SpriteRenderer>().sortingOrder = -1;
                bodyPartsObjs[0].transform.eulerAngles = new Vector3(0, 0, GetAngleFromVector(gridMoveDir) - 90);

                if (snakeSize > 1)
                {
                    bodyPartsObjs[1].GetComponent<SpriteRenderer>().sortingOrder = -2;
                    bodyPartsObjs[0].transform.eulerAngles = new Vector3(0, 0, GetAngleFromVector(gridMoveDir + lastGridMoveDir) - 90);
                }
            }
            
            lastGridMoveDir = gridMoveDir;
        }
    }

    // for death
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Tail")
        {
            return;
        }
        if (collision.gameObject.GetComponent<ColorState>().state != colorIndex)
        {
            alive = false;
            audioPlayer.PlayDieSound();
            //animation trigger
            GameAssets.Instance.finalScore.text = Score.GetScore().ToString();
            if (Score.TrySetHighScore())
            {
                GameAssets.Instance.newWord.SetActive(true);
            }
            GameAssets.Instance.highscore.text = Score.GetHighScore().ToString();
            Score.UpdateTotal();
            //wait
            GameAssets.Instance.gameOverScreen.SetActive(true);
        }
        else
        {
            Score.BlockScore();
        }
    }

    public void ResetGame()
    {
        foreach (var part in bodyPartsObjs)
        {
            Destroy(part.gameObject);
        }
        Awake();
        levelGrid.ResetLevel();
        Score.ResetScore();
        Score.CheckForChat();
    }

    private void CreateBodyPart()
    {
        GameObject snakeBodyGO = Instantiate(bodyPrefab);
        snakeBodyGO.name = "SnakeBody";
        bodyPartsObjs.Insert(0, snakeBodyGO);
    }

    private float GetAngleFromVector(Vector2Int dir)
    {
        float n = Mathf.Atan2 (dir.y, dir.x) * Mathf.Rad2Deg;
        if (n < 0) n += 360;
        return n;
    }

    public Vector2Int GetGridPos()
    {
        return headPosition;
    }

    public List<Vector2Int> GetAllPos()
    {
        List<Vector2Int> list = new List<Vector2Int>() { headPosition };
        list.AddRange(bodyPartsLocs);
        return list;
    }
    public void KillState()
    {
        alive = false ;
    }

    public void ReviveState()
    {
        alive = true;
    }

    public void PauseSnake()
    {
        pause = true;
    }

    public void UnPauseSnake()
    {
        pause = false;
    }
}
