using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public enum TileType { Normal, Point, Player, Obstacle }

    public TileType tileType = TileType.Normal;

    Renderer rend;

    Dictionary<TileType, Color> tileColors = new Dictionary<TileType, Color>
    {
        { TileType.Normal, Color.white },
        { TileType.Point, Color.green },
        { TileType.Player, Color.blue },
        { TileType.Obstacle, Color.red }
    };

    void Awake()
    {
        rend = GetComponent<Renderer>();
    }

    void Update()
    {
        rend.material.color = tileColors[tileType];
    }
}
