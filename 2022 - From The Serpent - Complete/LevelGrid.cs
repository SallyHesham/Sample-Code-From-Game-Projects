using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGrid
{
    private Vector2Int foodPos;
    private int halfWidth, halfHeight;
    private GameObject foodGO;
    private Snake snake;
    private List<Vector2Int> obstaclePosList;
    private List<GameObject> obstacleList;
    private List<Material> blockColors;
    private GameObject obstacleContainer;
    public GameObject block1;
    public GameObject block2;

    public LevelGrid(int w, int h)
    {
        halfWidth = w;
        halfHeight = h;
    }

    public void Setup(Snake s)
    {
        snake = s;
        block1 = GameAssets.Instance.block1;
        block2 = GameAssets.Instance.block2;
        SpawnFood();
        SetupLevel();
    }

    private void SpawnFood()
    {
        do
        {
            foodPos = new Vector2Int(Random.Range(-halfWidth + 1, halfWidth), Random.Range(-halfHeight + 1, halfHeight));
        } while (snake.GetAllPos().IndexOf(foodPos) != -1 );
        
        
        foodGO = GameObject.Instantiate(GameAssets.Instance.foodPrefab);
        foodGO.transform.position = new Vector3(foodPos.x, foodPos.y);
    }

    public bool TrySnakeEatFood(Vector2Int snakePos)
    {
        if (snakePos == foodPos)
        {
            Object.Destroy(foodGO);
            SpawnFood();
            return true;
        }
        else
        {
            return false;
        }
    }

    public Vector2Int WrapCoordinates(Vector2Int pos)
    {
        if (pos.x >= halfWidth)
            pos.x = -halfWidth + 1;
        if (pos.x <= -halfWidth)
            pos.x = halfWidth - 1;
        if (pos.y >= halfHeight)
            pos.y = -halfHeight + 1;
        if (pos.y <= -halfHeight)
            pos.y = halfHeight - 1;
        return pos;
    }

    public void SetupLevel()
    {
        obstaclePosList = new List<Vector2Int>();
        obstacleList = new List<GameObject>();
        blockColors = GameAssets.Instance.colors;
        int obsCount = Random.Range(GameAssets.Instance.minObs, GameAssets.Instance.maxObs);
        Vector2Int obsPos;
        int colorIndex;
        obstacleContainer = new GameObject("Obstacle Container");

        for (int i = 0; i < obsCount; i++)
        {
            do
            {
                obsPos = new Vector2Int(Random.Range(-halfWidth + 1, halfWidth), Random.Range(-halfHeight + 1, halfHeight));
            } while (obstaclePosList.IndexOf(obsPos) != -1 || obsPos == new Vector2Int(0, 0));
            colorIndex = Random.Range(0, blockColors.Count);
            CreateObstacle(obsPos, colorIndex);
        }
    }

    private void CreateObstacle(Vector2Int pos, int color)
    {
        GameObject obs;
        if (color == 0)
        {
            obs = GameObject.Instantiate(block1);
        }
        else
        {
            obs = GameObject.Instantiate(block2);
        }
        obs.GetComponent<SpriteRenderer>().sortingLayerName = "OverBack";
        obs.transform.position = new Vector3(pos.x, pos.y);
        obstaclePosList.Add(pos);
        obstacleList.Add(obs);
        obs.transform.parent = obstacleContainer.transform;
        obs.name = "Block";
    }

    public void ResetLevel()
    {
        foreach (var obs in obstacleList)
            GameObject.Destroy(obs.gameObject);
        GameObject.Destroy(obstacleContainer);
        GameObject.Destroy(foodGO);
        SpawnFood();
        SetupLevel();
    }
}
