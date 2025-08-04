using UnityEngine;

public class Player : MonoBehaviour
{
    public enum Direction { Up, Down, Left, Right }

    public FaceController currentFace;
    public int x = 0, y = 0;
    public GameBoard gameBoard;

    private Tile currentTile;
    private GameManager gameManager;
    private float lastMoveTime = 0f;

    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        if (currentFace == null)
        {
            Debug.LogError("Player: currentFace is not assigned!");
            return;
        }
        SelectCurrentTile();
    }


    public bool allowHolding = true;
    bool inputConsumed = false;
    float timeOffset = 0f;
    void Update()
    {
        if (!gameManager.isRunning && !gameManager.paused)
            return;

        if (timeOffset == 0f)
            timeOffset = Time.time;

        float timeSinceStart = Time.time - timeOffset;

        float moveCooldown = 1f / gameManager.playerSpeed;
        if (timeSinceStart - lastMoveTime < moveCooldown)
            return;

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        if (allowHolding)
        {
            if ((horizontal != 0 || vertical != 0) && !inputConsumed)
            {
                inputConsumed = true;
            }
            else if (horizontal == 0 && vertical == 0)
            {
                inputConsumed = false;
                return;
            }
            else
            {
                return;
            }
        }


        if (horizontal != 0 || vertical != 0)
        {
            lastMoveTime = timeSinceStart;
            if (gameManager.paused)
            {
                gameManager.Restart();
                return;
            }
        }

        if (vertical > 0) TryMove(Direction.Up);
        else if (vertical < 0) TryMove(Direction.Down);
        else if (horizontal < 0) TryMove(Direction.Left);
        else if (horizontal > 0) TryMove(Direction.Right);


        if (timeSinceStart - lastMoveTime > gameManager.idleTimeout)
        {
            gameManager.Pause();
            return;
        }
    }

    void TryMove(Direction dir)
    {
        int newX = x, newY = y;
        Direction newDir = dir;

        switch (dir)
        {
            case Direction.Up:
                HandleVertical(true, out newY, out newDir);
                break;
            case Direction.Down:
                HandleVertical(false, out newY, out newDir);
                break;
            case Direction.Right:
                HandleHorizontal(true, out newX, out newDir);
                break;
            case Direction.Left:
                HandleHorizontal(false, out newX, out newDir);
                break;
        }

        if (currentFace.IsValidPosition(newX, newY))
        {
            x = newX;
            y = newY;
            SelectCurrentTile();
        }
        else
        {
            HandleEdge(newDir);
        }
    }

    void HandleVertical(bool up, out int newY, out Direction newDir)
    {
        if (currentFace.faceType == FaceController.FaceType.Back ||
            currentFace.faceType == FaceController.FaceType.Left)
        {
            newY = up ? y + 1 : y - 1;
            newDir = up ? Direction.Down : Direction.Up;
        }
        else
        {
            newY = up ? y - 1 : y + 1;
            newDir = up ? Direction.Up : Direction.Down;
        }
    }

    void HandleHorizontal(bool right, out int newX, out Direction newDir)
    {
        if (currentFace.faceType == FaceController.FaceType.Back ||
            currentFace.faceType == FaceController.FaceType.Left)
        {
            newX = right ? x + 1 : x - 1;
            newDir = right ? Direction.Left : Direction.Right;
        }
        else
        {
            newX = right ? x - 1 : x + 1;
            newDir = right ? Direction.Right : Direction.Left;
        }
    }

    void HandleEdge(Direction dir)
    {
        FaceController nextFace = dir switch
        {
            Direction.Up => FaceTypeToController(currentFace.top),
            Direction.Down => FaceTypeToController(currentFace.bottom),
            Direction.Left => FaceTypeToController(currentFace.left),
            Direction.Right => FaceTypeToController(currentFace.right),
            _ => null
        };

        if (nextFace != null)
        {
            MakeEdgeTransition(nextFace, dir);
            SelectCurrentTile();
        }
    }

    FaceController FaceTypeToController(FaceController.FaceType faceType)
    {
        gameBoard.faceMap.TryGetValue(faceType, out FaceController face);
        return face;
    }

    void MakeEdgeTransition(FaceController newFace, Direction dir)
    {
        int size = newFace.gridSize;

        if (newFace.top == currentFace.faceType)
        {
            if (y == 0 && (dir == Direction.Left || dir == Direction.Right || dir == Direction.Up))
                x = size - 1 - x;
            else if (y == size - 1 && (dir == Direction.Left || dir == Direction.Right))
                x = size - 1 - x;
            else if (x == size - 1 && dir == Direction.Right)
                x = size - 1 - x;
            else if (x == size - 1 && dir == Direction.Left)
                x = size - 1 - y;
            else if (x == 0 && (dir == Direction.Left || dir == Direction.Right))
                x = y;
            y = 0;
        }
        else if (newFace.bottom == currentFace.faceType)
        {
            if (y == 0 && (dir == Direction.Left || dir == Direction.Right))
                x = size - 1 - x;
            else if (y == size - 1 && (dir == Direction.Up || dir == Direction.Down))
                x = size - 1 - x;
            else if (x == 0 && (dir == Direction.Left || dir == Direction.Up))
                x = y;
            else if (x == 0 && (dir == Direction.Right || dir == Direction.Down))
                x = size - 1 - y;
            else if (x == size - 1 && dir == Direction.Left)
                x = y;
            y = size - 1;
        }
        else if (newFace.left == currentFace.faceType)
        {
            if (y == 0 && dir == Direction.Up)
                y = size - 1 - x;
            else if (y == 0 && dir == Direction.Up)
                y = x;
            else if (y == size - 1 && dir == Direction.Up)
                y = size - 1 - x;
            else if (y == size - 1 && dir == Direction.Down)
                y = x;
            x = size - 1;
        }
        else if (newFace.right == currentFace.faceType)
        {
            if (y == 0 && (dir == Direction.Up || dir == Direction.Down))
                y = x;
            else if (y == size - 1 && (dir == Direction.Up || dir == Direction.Down))
                y = size - 1 - x;
            x = 0;
        }
        currentFace = newFace;
    }

    void SelectCurrentTile()
    {
        if (currentTile != null)
            currentTile.tileType = Tile.TileType.Normal;

        currentTile = currentFace.tiles[x, y];

        switch (currentTile.tileType)
        {
            case Tile.TileType.Point:
                currentFace.CollectPoint(x, y);
                gameManager.IncreaseScore();
                break;
            case Tile.TileType.Obstacle:
                gameManager.GameOver();
                return;
        }

        currentTile.tileType = Tile.TileType.Player;
    }
}
