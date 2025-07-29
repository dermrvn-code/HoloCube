using System.Collections.Generic;
using UnityEngine;

public class FaceController : MonoBehaviour
{
    public enum FaceType
    {
        Front,
        Back,
        Left,
        Right,
        Top,
        Bottom
    }

    public int gridSize = 3;
    public GameObject tilePrefab;
    float spacing = 0.02f;
    public Tile[,] tiles;

    public FaceType top;
    public FaceType bottom;
    public FaceType left;
    public FaceType right;

    public FaceType faceType;

    void GenerateNeighbours()
    {
        switch (faceType)
        {
            case FaceType.Front:
                top = FaceType.Top;
                bottom = FaceType.Bottom;
                left = FaceType.Left;
                right = FaceType.Right;
                break;
            case FaceType.Back:
                top = FaceType.Top;
                bottom = FaceType.Bottom;
                left = FaceType.Right;
                right = FaceType.Left;
                break;
            case FaceType.Left:
                top = FaceType.Top;
                bottom = FaceType.Bottom;
                left = FaceType.Back;
                right = FaceType.Front;
                break;
            case FaceType.Right:
                top = FaceType.Top;
                bottom = FaceType.Bottom;
                left = FaceType.Front;
                right = FaceType.Back;
                break;
            case FaceType.Top:
                top = FaceType.Back;
                bottom = FaceType.Front;
                left = FaceType.Left;
                right = FaceType.Right;
                break;
            case FaceType.Bottom:
                top = FaceType.Front;
                bottom = FaceType.Back;
                left = FaceType.Left;
                right = FaceType.Right;
                break;
        }
    }

    public void Initialize(float spacing)
    {
        this.spacing = spacing;
        GenerateFace();
        GenerateNeighbours();
    }

    int pointsOnFace = 0;
    int obstaclesOnFace = 0;
    public void GenerateSpecialTiles(Tile.TileType type = Tile.TileType.Point, float maxFill = 1 / 8f)
    {
        int maxFillFields = Mathf.FloorToInt(gridSize * gridSize * maxFill);
        if (type == Tile.TileType.Point)
        {
            if (pointsOnFace >= maxFillFields)
                return;
        }
        else if (type == Tile.TileType.Obstacle)
        {
            if (obstaclesOnFace >= maxFillFields)
                return;
        }
        else
        {
            return;
        }

        int randX = Random.Range(0, gridSize);
        int randY = Random.Range(0, gridSize);

        int attempts = 0;
        while (tiles[randX, randY].tileType != Tile.TileType.Normal)
        {
            randX = Random.Range(0, gridSize);
            randY = Random.Range(0, gridSize);
            attempts++;

            if (attempts > 10)
                return;
        }
        tiles[randX, randY].tileType = type;

        if (type == Tile.TileType.Obstacle)
            obstaclesOnFace++;
        else if (type == Tile.TileType.Point)
            pointsOnFace++;
    }

    public void CollectPoint(int x, int y)
    {
        if (tiles[x, y].tileType == Tile.TileType.Point)
        {
            tiles[x, y].tileType = Tile.TileType.Normal;
            pointsOnFace--;
        }
    }



    void GenerateFace()
    {
        tiles = new Tile[gridSize, gridSize];

        float tileSize = 1f / (gridSize + (gridSize - 1) * spacing);
        float actualSpacing = tileSize * spacing;

        Vector3 origin = new Vector3(-0.5f + tileSize / 2f, 0.5f - tileSize / 2f, 0);

        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                Vector3 pos = origin + new Vector3(x * (tileSize + actualSpacing), -y * (tileSize + actualSpacing), 0);
                GameObject tileGO = Instantiate(tilePrefab, transform);
                tileGO.transform.localPosition = pos;
                tileGO.transform.localScale = Vector3.one * tileSize;

                Tile tile = tileGO.GetComponent<Tile>();
                tiles[x, y] = tile;

                tile.tileType = Tile.TileType.Normal;
            }
        }
    }

    public bool IsValidPosition(int x, int y)
    {
        return x >= 0 && y >= 0 && x < gridSize && y < gridSize;
    }
}
