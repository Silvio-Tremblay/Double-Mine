using System;
using UnityEngine;

public class Game : MonoBehaviour
{
   public int width = 16;
   public int height = 16;
   public int mineCount = 32;

   public String[] colors = {"blue", "red", "purple"};

   private Board board;
   private Cell[,] state;

   private void Awake()
   {
        board = GetComponentInChildren<Board>();
   }

   private void Start()
   {
        NewGame();
   }

   private void NewGame()
   {
        state = new Cell[width, height];

        GenerateCells();
        GenerateMines();

        Camera.main.transform.position = new Vector3(width / 2f, height / 2f, -10);
        board.Draw(state);
   }

   private void GenerateCells()
   {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Cell cell = new Cell();
                cell.position = new Vector3Int(x, y, 0);
                cell.type = Cell.Type.Empty;
                state[x, y] = cell;
            }
        }
   }

   private void GenerateMines()
   {
     for (int i = 0; i < mineCount; i++)
        {
            int x = UnityEngine.Random.Range(0, width);
            int y = UnityEngine.Random.Range(0, height);
            String color = colors[UnityEngine.Random.Range(0, 3)];

            while(state[x, y].type == Cell.Type.Mine)
            {
                x = UnityEngine.Random.Range(0, width);
                y = UnityEngine.Random.Range(0, height);
            }

            state[x, y].type = Cell.Type.Mine;
            state[x, y].color = color;
        }
   }
}
