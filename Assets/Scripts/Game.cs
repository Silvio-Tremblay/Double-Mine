using System;
using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
   public int width = 8;
   public int height = 8;
   public int mineCount = 16;

   public String[] colors = {"blue", "red", "purple"};

public enum FlagColor {
           Blue,
           Red
}

public FlagColor flagColor = FlagColor.Blue;
public float mouseScrollY;
private Board board;
private Cell[,] grid;

private bool generated;

private int firstTileX;

private int firstTileY;
private bool gameOver;

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

        StopAllCoroutines();

        Camera.main.transform.position = new Vector3(width / 2f, height / 2f, -10);
        gameOver = false;
        generated = false;

        grid = new Cell[width, height];   
        GenerateCells();     
        board.Draw(grid);
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
               grid[x, y] = cell;
          }
        }
   }

   private void GenerateOtherTiles(Cell startingCell)
   {
     if (generated) return;

     GenerateMines(startingCell);
     GenerateNumbers();

     generated = true;
   }

   private void GenerateMines(Cell startingCell)
   {
          for (int i = 0; i < mineCount; i++)
          {
               int x = UnityEngine.Random.Range(0, width);
               int y = UnityEngine.Random.Range(0, height);
               String color = colors[UnityEngine.Random.Range(0, 2)];

               Cell cell = grid[x, y];

               while (cell.type == Cell.Type.Mine || IsAdjacent(startingCell, cell))
               {
                    x++;

                    if (x >= width)
                    {
                         x = 0;
                         y++;

                         if (y >= height) {
                         y = 0;
                         }
                    }

                    cell = grid[x, y];
               }


               cell.type = Cell.Type.Mine;
               cell.color = color;
          }
   }

   public void GenerateNumbers() 
   {
     for (int x = 0; x < width; x++)
        {
               for (int y = 0; y < height; y++)
               {
                    Cell cell = grid[x, y];
                    if (cell.type == Cell.Type.Mine)
                    {
                    continue;
                    }

                    cell.number = CountMines(x, y);

                    if (cell.number > 0)
                    {
                         cell.type = Cell.Type.Number;
                         cell.color = NumberColor(x, y);
                         grid[x, y] = cell;
                         
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

               if (grid[x, y].type == Cell.Type.Mine)
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
     if (Input.GetKeyDown(KeyCode.R)) {
          NewGame();
     }
     else if (!gameOver) {
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
   }

   private void Flag()
   {
     if (IsMouseOnCell(out Cell cell)) return;
     if (cell.revealed) return;

     if (flagColor == FlagColor.Blue) {
          cell.redFlagged = false;
          cell.blueFlagged = !cell.blueFlagged;
     } else {
          cell.blueFlagged = false;
          cell.redFlagged = !cell.redFlagged;
     }
     board.Draw(grid);
   }

   private void Reveal()
   {
     if (IsMouseOnCell(out Cell cell)) {
          if (!generated) {
               GenerateOtherTiles(cell);
          }
          Reveal(cell);
     }
   }

   private void Reveal(Cell cell)
   {
     if (cell.revealed || cell.blueFlagged || cell.redFlagged) {
          return;
     }

     switch (cell.type)
     {
          case Cell.Type.Mine: 
               Explode(cell);
               break;
          case Cell.Type.Empty:
               Flood(cell);
               CheckWinCondition();
               break;
          default:
               cell.revealed = true;
               CheckWinCondition();
               break;

     }

     board.Draw(grid);
   }

   private void Flood (Cell cell) 
   {
     if (cell.revealed || cell.type == Cell.Type.Mine) {
          return;
     }

     cell.revealed = true;
     grid[cell.position.x, cell.position.y] = cell;

     if (cell.type == Cell.Type.Empty) {

          Cell adjacentCell = GetCell(cell.position.x - 1, cell.position.y);
          if (adjacentCell != null) {
               Flood(adjacentCell);
          }

          adjacentCell = GetCell(cell.position.x + 1, cell.position.y);
          if (adjacentCell != null) {
               Flood(adjacentCell);
          }

          adjacentCell = GetCell(cell.position.x, cell.position.y - 1);
          if (adjacentCell != null) {
               Flood(adjacentCell);
          }

          adjacentCell = GetCell(cell.position.x, cell.position.y + 1);
          if (adjacentCell != null) {
               Flood(adjacentCell);
          }

          adjacentCell = GetCell(cell.position.x - 1, cell.position.y - 1);
          if (adjacentCell != null) {
               Flood(adjacentCell);
          }

          adjacentCell = GetCell(cell.position.x - 1, cell.position.y + 1);
          if (adjacentCell != null) {
               Flood(adjacentCell);
          }

          adjacentCell = GetCell(cell.position.x + 1, cell.position.y - 1);
          if (adjacentCell != null) {
               Flood(adjacentCell);
          }

          adjacentCell = GetCell(cell.position.x + 1, cell.position.y + 1);
          if (adjacentCell != null) {
               Flood(adjacentCell);
          }
     }
   }

   private void Explode(Cell cell)
   {
     gameOver = true;
     Debug.Log("You Lost! Press \"R\" to play again!");
     cell.revealed = true;
     cell.exploded = true;
     grid[cell.position.x, cell.position.y] = cell;

    for (int x = 0; x < width; x++)
    {
        for (int y = 0; y < height; y++)
        {
            Cell currentCell = grid[x, y];

            if (currentCell.blueFlagged || currentCell.redFlagged)
            {
                continue;
            }

            if (currentCell.type == Cell.Type.Mine)
            {
                currentCell.revealed = true;
                grid[x, y] = currentCell;
            }
        }
    }

    board.Draw(grid);
   }

   private void CheckWinCondition()
   {
     int score = 0;
     for (int x = 0; x < width; x++)
    {
        for (int y = 0; y < height; y++)
        {
            Cell cell = grid[x, y];
            if (cell.type != Cell.Type.Mine && !cell.revealed) {
               return;
            }
            if (cell.type == Cell.Type.Mine && cell.color.Equals("blue") && cell.blueFlagged) {
               score++;
               continue;
            }
            if (cell.type == Cell.Type.Mine && cell.color.Equals("red") && cell.redFlagged) {
               score++;
            }
        }
    }

    gameOver = true;
    Debug.Log("You Won! Total Score: " + score + ". Press \"R\" to play again!");
    

   }

   private Cell GetCell(int x, int y)
   {
     if (IsValid(x, y)) {
          return grid[x, y];
     } else {
          return null;
     }     
   }

   private bool IsCellValid(int x, int y, out Cell cell)
   {
     cell = GetCell(x, y);
     return cell != null;
   }

   private bool IsValid(int x, int y)
   {
     return x >= 0 && x < width && y >= 0 && y < height; 
     
   }

   private bool IsMouseOnCell(out Cell cell)
    {
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cellPosition = board.tilemap.WorldToCell(worldPosition);
        return IsCellValid(cellPosition.x, cellPosition.y, out cell);
    }

    private bool IsAdjacent(Cell a, Cell b)
    {
          return Mathf.Abs(a.position.x - b.position.x) <= 1 &&
               Mathf.Abs(a.position.y - b.position.y) <= 1;
    }
}
