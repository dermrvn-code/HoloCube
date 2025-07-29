using System.Collections.Generic;
using UnityEngine;

public class GameBoard : MonoBehaviour
{
    public GameObject tilePrefab;
    public float spacing = 0.2f;

    public Dictionary<FaceController.FaceType, FaceController> faceMap = new Dictionary<FaceController.FaceType, FaceController>();

    Player player;
    GameManager gameManager;

    void Awake()
    {
        gameManager = FindFirstObjectByType<GameManager>();

        CreateFaces();
        player = FindFirstObjectByType<Player>();
        player.currentFace = faceMap[gameManager.startFace];
        player.x = gameManager.startPosition.x;
        player.y = gameManager.startPosition.y;
        player.gameBoard = this;
        GetComponent<Renderer>().enabled = false;

        for (int i = 0; i < gameManager.startPoints; i++)
        {
            GeneratePoints();
        }
    }

    void CreateFaces()
    {
        FaceController.FaceType[] faceTypes = { FaceController.FaceType.Front, FaceController.FaceType.Back, FaceController.FaceType.Right, FaceController.FaceType.Left, FaceController.FaceType.Top, FaceController.FaceType.Bottom };
        Vector3[] facePositions = {
            new Vector3(0, 0, 0.5f),   // Front
            new Vector3(0, 0, -0.5f),  // Back
            new Vector3(-0.5f, 0, 0),  // Right
            new Vector3(0.5f, 0, 0),   // Left
            new Vector3(0, 0.5f, 0),   // Top
            new Vector3(0, -0.5f, 0)   // Bottom
        };

        Vector3[] faceRotations = {
            Vector3.zero,              // Front
            new Vector3(0, 180, 0),    // Back
            new Vector3(0, -90, 0),    // Right
            new Vector3(0, 90, 0),     // Left
            new Vector3(-90, 0, 0),    // Top
            new Vector3(90, 0, 0)      // Bottom
        };

        for (int i = 0; i < 6; i++)
        {
            GameObject faceGO = new GameObject(faceTypes[i].ToString());
            faceGO.transform.parent = transform;
            faceGO.transform.localPosition = facePositions[i];
            faceGO.transform.localEulerAngles = faceRotations[i];
            faceGO.transform.localScale = Vector3.one;


            FaceController face = faceGO.AddComponent<FaceController>();
            face.tilePrefab = tilePrefab;
            face.gridSize = gameManager.gridSize;
            face.faceType = faceTypes[i];
            face.Initialize(gameManager.spacing);

            faceMap[faceTypes[i]] = face;
        }
    }

    void Update()
    {
        if (gameManager.isRunning || gameManager.isGameOver)
            transform.Rotate(Vector3.left, Time.deltaTime * gameManager.cubeSpeed, Space.World);

        if (!gameManager.isRunning)
            return;

        SpawnNewPoints();
        SpawnNewObstacles();
    }

    float pointTimer = 0f;
    void SpawnNewPoints()
    {
        pointTimer += Time.deltaTime;
        if (pointTimer >= gameManager.pointSpawnInterval)
        {
            pointTimer = 0f;
            GeneratePoints();
        }
    }

    float obstacleTimer = 0f;
    void SpawnNewObstacles()
    {
        obstacleTimer += Time.deltaTime;
        if (obstacleTimer >= gameManager.obstacleSpawnInterval)
        {
            obstacleTimer = 0f;
            GenerateObstacles();
        }
    }

    void GeneratePoints()
    {
        foreach (var face in faceMap.Values)
        {
            face.GenerateSpecialTiles();
        }
    }

    void GenerateObstacles()
    {
        foreach (var face in faceMap.Values)
        {
            face.GenerateSpecialTiles(Tile.TileType.Obstacle, 1 / 10f);
        }
    }
}
