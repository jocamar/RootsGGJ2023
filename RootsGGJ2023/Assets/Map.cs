using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    public enum TileType
    {
        Empty,
        Root,
        Obstacle,
        End,
        Start
    }

    public class Tile
    {
        public TileType type;
    }

    private int width;
    private int height;
    private Tile[,] tiles;
    public int myStartX;
    public int myStartY;
    private int myEndX;
    private int myEndY;
    private MonoBehaviour owner;

    private GameObject[,] objects;

    public Map(MonoBehaviour owner, int width, int height)
    {
        this.width = width;
        this.height = height;
        tiles = new Tile[width, height];
        objects = new GameObject[width, height];
        this.owner = owner;
    }

    public int GetWidth()
    {
        return width;
    }

    public int GetHeight()
    {
        return height;
    }

    public Tile GetTile(int x, int y)
    {
        if (x >= 0 && x < width && y >= 0 && y < height)
        {
            return tiles[x, y];
        }
        return null;
    }

    public void SetTile(int x, int y, Tile tile)
    {
        if (x >= 0 && x < width && y >= 0 && y < height)
        {
            tiles[x, y] = tile;
        }
    }

    public void OnDestroy()
    {
        for (int x = 0; x < objects.GetLength(0); x++)
        {
            for (int y = 0; y < objects.GetLength(1); y++)
            {
                Destroy(objects[x, y]);
            }
        }
    }

    public void SetObject(int x, int y, GameObject tile)
    {
        if (x >= 0 && x < width && y >= 0 && y < height)
        {
            if (objects[x,y])
                Destroy(objects[x, y]);

            objects[x, y] = tile;
        }
    }

    public void GenerateMap(int endDistance)
    {
        // Initialize the map with the default tile type
        for (int x = 0; x < tiles.GetLength(0); x++)
        {
            for (int y = 0; y < tiles.GetLength(1); y++)
            {
                tiles[x, y] = new Tile { type = TileType.Empty };
            }
        }

        // Determine the start tile position randomly along one of the borders
        myStartX = 0;
        myStartY = 0;
        int border = Random.Range(0, 4);
        if (border == 0)
        {
            myStartX = 0;
            myStartY = Random.Range(0, tiles.GetLength(1));
        }
        else if (border == 1)
        {
            myStartX = tiles.GetLength(0) - 1;
            myStartY = Random.Range(0, tiles.GetLength(1));
        }
        else if (border == 2)
        {
            myStartX = Random.Range(0, tiles.GetLength(0));
            myStartY = 0;
        }
        else if (border == 3)
        {
            myStartX = Random.Range(0, tiles.GetLength(0));
            myStartY = tiles.GetLength(1) - 1;
        }

        // Set the start tile
        tiles[myStartX, myStartY] = new Tile { type = TileType.Start };

        // Determine the end tile position based on the distance from the start tile
        myEndX = 0;
        myEndY = 0;
        if (myStartY == 0)
        {
            myEndX = tiles.GetLength(0) - 1;
            myEndY = myStartY + endDistance;
        }
        else if (myStartY == tiles.GetLength(0) - 1)
        {
            myEndX = 0;
            myEndY = myStartY - endDistance;
        }
        else if (myStartX == 0)
        {
            myEndX = myStartX + endDistance;
            myEndY = tiles.GetLength(1) - 1;
        }
        else if (myStartX == tiles.GetLength(1) - 1)
        {
            myEndX = myStartX - endDistance;
            myEndY = 0;
        }

        // Set the end tile
        tiles[myEndX, myEndY] = new Tile { type = TileType.End };

        while (true)
        {
            GenerateObstacles(15);
            if (IsReachable(myStartX, myStartY, myEndX, myEndY))
               break;

            ClearObstacles();
        }
    }

    public void ClearObstacles()
    {
        for (int x = 0; x < tiles.GetLength(0); x++)
        {
            for (int y = 0; y < tiles.GetLength(1); y++)
            {
                if (tiles[x, y].type == TileType.Obstacle)
                {
                    tiles[x, y].type = TileType.Empty;
                }
            }
        }
    }

    private bool CheckOverlap(int[,] obstacle, int x, int y)
    {
        for (int i = 0; i < obstacle.GetLength(0); i++)
        {
            for (int j = 0; j < obstacle.GetLength(1); j++)
            {
                int mapX = x + i;
                int mapY = y + j;
                if (mapX < 0 || mapX >= tiles.GetLength(0) || mapY < 0 || mapY >= tiles.GetLength(1) || (obstacle[i, j] == 1 && tiles[mapX, mapY].type != TileType.Empty))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private bool IsReachable(int startX, int startY, int endX, int endY)
    {
        Queue<(int, int)> queue = new Queue<(int, int)>();
        queue.Enqueue((startX, startY));
        bool[,] visited = new bool[tiles.GetLength(0), tiles.GetLength(1)];
        visited[startX, startY] = true;

        while (queue.Count > 0)
        {
            (int x, int y) = queue.Dequeue();
            if (x == endX && y == endY)
            {
                return true;
            }

            if (x > 0 && !visited[x - 1, y] && tiles[x - 1, y].type != TileType.Obstacle)
            {
                queue.Enqueue((x - 1, y));
                visited[x - 1, y] = true;
            }
            if (x < tiles.GetLength(0) - 1 && !visited[x + 1, y] && tiles[x + 1, y].type != TileType.Obstacle)
            {
                queue.Enqueue((x + 1, y));
                visited[x + 1, y] = true;
            }
            if (y > 0 && !visited[x, y - 1] && tiles[x, y - 1].type != TileType.Obstacle)
            {
                queue.Enqueue((x, y - 1));
                visited[x, y - 1] = true;
            }
            if (y < tiles.GetLength(1) - 1 && !visited[x, y + 1] && tiles[x, y + 1].type != TileType.Obstacle)
            {
                queue.Enqueue((x, y + 1));
                visited[x, y + 1] = true;
            }
        }

        return false;
    }

    public void GenerateObstacles(int numObstacles)
    {
        int mapWidth = tiles.GetLength(0);
        int mapHeight = tiles.GetLength(1);
        List<int[,]> tetrominoes = new List<int[,]>();
        tetrominoes.Add(new int[,] { { 1, 1, 1, 1 } });
        tetrominoes.Add(new int[,] { { 1, 1 }, { 1, 1 } });
        tetrominoes.Add(new int[,] { { 1, 1, 1 }, { 0, 1, 0 } });
        tetrominoes.Add(new int[,] { { 1, 1, 1 }, { 1, 0, 0 } });
        tetrominoes.Add(new int[,] { { 0, 1, 1 }, { 1, 1, 0 } });
        tetrominoes.Add(new int[,] { { 1, 1, 0 }, { 0, 1, 1 } });
        tetrominoes.Add(new int[,] { { 1, 1, 1 }, { 0, 0, 1 } });

        for (int i = 0; i < numObstacles; i++)
        {
            int[,] tetromino = tetrominoes[Random.Range(0, tetrominoes.Count)];
            int rotation = Random.Range(0, 4);
            tetromino = RotateMatrix(tetromino, rotation);
            int tetrominoWidth = tetromino.GetLength(0);
            int tetrominoHeight = tetromino.GetLength(1);

            int x = Random.Range(0, mapWidth - tetrominoWidth + 1);
            int y = Random.Range(0, mapHeight - tetrominoHeight + 1);

            bool isOverlapping = CheckOverlap(tetromino, x, y);
            
            while (isOverlapping)
            {
                x = Random.Range(0, mapWidth - tetrominoWidth + 1);
                y = Random.Range(0, mapHeight - tetrominoHeight + 1);
                isOverlapping = CheckOverlap(tetromino, x, y);
            }

            for (int m = 0; m < tetrominoWidth; m++)
            {
                for (int n = 0; n < tetrominoHeight; n++)
                {
                    if (tetromino[m, n] == 1)
                    {
                        tiles[x + m, y + n].type = TileType.Obstacle;
                    }
                }
            }
        }
    }

    private int[,] RotateMatrix(int[,] matrix, int rotation)
    {
        int[,] rotatedMatrix = new int[matrix.GetLength(0), matrix.GetLength(1)];

        for (int i = 0; i < rotation; i++)
        {
            rotatedMatrix = RotateMatrix90Degrees(matrix);
            matrix = rotatedMatrix;
        }

        return rotatedMatrix;
    }

    private int[,] RotateMatrix90Degrees(int[,] matrix)
    {
        int[,] rotatedMatrix = new int[matrix.GetLength(1), matrix.GetLength(0)];

        for (int i = 0; i < matrix.GetLength(0); i++)
        {
            for (int j = 0; j < matrix.GetLength(1); j++)
            {
                rotatedMatrix[j, matrix.GetLength(0) - i - 1] = matrix[i, j];
            }
        }

        return rotatedMatrix;
    }
}
