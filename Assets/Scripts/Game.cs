using System;
using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
   public int width = 16;
   public int height = 16;
   public int mineCount = 32;

   public String[] colors = {"blue", "red", "purple"};

   public enum FlagColor {
          Blue,
          Red
   }

   public FlagColor flagColor = FlagColor.Blue;
   public float mouseScrollY;
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
        GenerateNumbers();

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
               String color = colors[UnityEngine.Random.Range(0, 2)];

               while(state[x, y].type == Cell.Type.Mine)
               {
                    x = UnityEngine.Random.Range(0, width);
                    y = UnityEngine.Random.Range(0, height);
               }

               state[x, y].type = Cell.Type.Mine;
               state[x, y].color = color;
          }
   }

   public void GenerateNumbers() 
   {
     for (int x = 0; x < width; x++)
        {
               for (int y = 0; y < height; y++)
               {
                    Cell cell = state[x, y];
                    if (cell.type == Cell.Type.Mine)
                    {
                    continue;
                    }

                    cell.number = CountMines(x, y);

                    if (cell.number > 0)
                    {
                         cell.type = Cell.Type.Number;
                         cell.color = NumberColor(x, y);
                         state[x, y] = cell;
                         
                    }
                    
               }
        }
   }

   public int CountMines(int cellX, int cellY)
   {
     int count = 0;

     for (int adjacentX = -1; adjacentX <= 1; adjacentX++)
     {
          for (int adjacentY = -1; adjacentY <= 1; adjacentY++)
          {
               if (adjacentX == 0 && adjacentY == 0)
               {
                    continue;
               }

               int x = cellX + adjacentX;
               int y = cellY + adjacentY;

               if (x < 0 || x >= width || y < 0 || y >= height) 
               {
                    continue;
               }

               if (state[x, y].type == Cell.Type.Mine)
               {
                    count++;

               }
          }
     }
     return count;
   }

   public String NumberColor(int cellX, int cellY)
   {
     String color = "none";

     for (int adjacentX = -1; adjacentX <= 1; adjacentX++)
     {
          for (int adjacentY = -1; adjacentY <= 1; adjacentY++)
          {
               if (adjacentX == 0 && adjacentY == 0)
               {
                    continue;
               }

               int x = cellX + adjacentX;
               int y = cellY + adjacentY;

               if (x < 0 || x >= width || y < 0 || y >= height) 
               {
                    continue;
               }

               if (GetCell(x, y).type == Cell.Type.Mine)
               {
                    color = ColorSwap(color, GetCell(x, y).color);
               }
          }
     }
     return color;
   }

   public String ColorSwap(String currentColor, String mineColor) 
   {
          switch (currentColor)
          {
               case "none": return mineColor;
               case "red": 
                    if (mineColor.Equals("red"))
                    {
                         return "red";
                    } else {
                         return "purple";
                    }
               case "blue":     
                    if (mineColor.Equals("blue"))
                    {
                         return "blue";
                    } else {
                         return "purple";
                    }
               case "purple": return "purple";
               default: return mineColor;     
          }
   }

   private void Update()
   {
     if (Input.GetKeyDown(KeyCode.Alpha1)) {
          flagColor = FlagColor.Blue;
     }
     if (Input.GetKeyDown(KeyCode.Alpha2)) {
          flagColor = FlagColor.Red;
     }
     if (Input.GetMouseButtonDown(1))
     {
          Flag();
     } else if (Input.GetMouseButtonDown(0)) {
          Reveal();
     }
   }

   private void Flag()
   {
     Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
     Vector3Int cellPosition = board.tilemap.WorldToCell(worldPosition);
     Cell cell = GetCell(cellPosition.x, cellPosition.y);

     if (cell.type == Cell.Type.Invalid || cell.revealed) {
          return;
     }

     if (flagColor == FlagColor.Blue) {
          cell.redFlagged = false;
          cell.blueFlagged = !cell.blueFlagged;
     } else {
          cell.blueFlagged = false;
          cell.redFlagged = !cell.redFlagged;
     }
     state[cellPosition.x, cellPosition.y] = cell;
     board.Draw(state);
   }

   private void Reveal()
   {
     Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
     Vector3Int cellPosition = board.tilemap.WorldToCell(worldPosition);
     Cell cell = GetCell(cellPosition.x, cellPosition.y);

     if (cell.type == Cell.Type.Invalid || cell.revealed || cell.blueFlagged || cell.redFlagged) {
          return;
     }

     if (cell.type == Cell.Type.Empty) {
          Flood(cell);
     }

     cell.revealed = true;
     state[cellPosition.x, cellPosition.y] = cell;
     board.Draw(state);
   }

   private void Flood (Cell cell) 
   {
     if (cell.revealed || cell.type == Cell.Type.Mine || cell.type == Cell.Type.Invalid) {
          return;
     }

     cell.revealed = true;
     state[cell.position.x, cell.position.y] = cell;

     if (cell.type == Cell.Type.Empty) {
          
          Flood(GetCell(cell.position.x - 1, cell.position.y));
          Flood(GetCell(cell.position.x + 1, cell.position.y));
          Flood(GetCell(cell.position.x, cell.position.y - 1));
          Flood(GetCell(cell.position.x, cell.position.y + 1));
          Flood(GetCell(cell.position.x - 1, cell.position.y - 1));
          Flood(GetCell(cell.position.x - 1, cell.position.y + 1));
          Flood(GetCell(cell.position.x + 1, cell.position.y - 1));
          Flood(GetCell(cell.position.x + 1, cell.position.y + 1));
     }
   }

   private Cell GetCell(int x, int y)
   {
     if (IsValid(x, y)) {
          return state[x, y];
     } else {
          return new Cell();
     }     
   }

   private bool IsValid(int x, int y)
   {
     return x >= 0 && x < width && y >= 0 && y < height; 
     
   }


}
