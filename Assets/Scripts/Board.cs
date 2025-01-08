using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Board : MonoBehaviour
{
    public Tilemap tilemap { get; private set; }

    public Tile tileUnkown;
    public Tile tileEmpty;
    public Tile tileExploded;

    public Dictionary<string, Tile[]> coloredTiles;
    public Tile tileBlueMine;
    public Tile tileBlueFlag;
    public Tile tileBlueNum1;
    public Tile tileBlueNum2;
    public Tile tileBlueNum3;
    public Tile tileBlueNum4;
    public Tile tileBlueNum5;
    public Tile tileBlueNum6;
    public Tile tileBlueNum7;
    public Tile tileBlueNum8;
    public Tile tileRedMine;
    public Tile tileRedFlag;
    public Tile tileRedNum1;
    public Tile tileRedNum2;
    public Tile tileRedNum3;
    public Tile tileRedNum4;
    public Tile tileRedNum5;
    public Tile tileRedNum6;
    public Tile tileRedNum7;
    public Tile tileRedNum8;
    public Tile tilePurpleMine;
    public Tile tilePurpleFlag;
    public Tile tilePurpleNum1;
    public Tile tilePurpleNum2;
    public Tile tilePurpleNum3;
    public Tile tilePurpleNum4;
    public Tile tilePurpleNum5;
    public Tile tilePurpleNum6;
    public Tile tilePurpleNum7;
    public Tile tilePurpleNum8;


    public void Awake()
    {
        tilemap = GetComponent<Tilemap>();
    }

    public void Draw(Cell[,] state) 
    {
        int width = state.GetLength(0);
        int height = state.GetLength(1);
        coloredTiles = new Dictionary<string, Tile[]>
        {
            {"blue", new Tile[] {tileBlueMine, tileBlueFlag, tileBlueNum1, tileBlueNum2, tileBlueNum3, tileBlueNum4, tileBlueNum5, tileBlueNum6, tileBlueNum7, tileBlueNum8}},
            {"red", new Tile[] {tileRedMine, tileRedFlag, tileRedNum1, tileRedNum2, tileRedNum3, tileRedNum4, tileRedNum5, tileRedNum6, tileRedNum7, tileRedNum8}},
            {"purple", new Tile[] {tilePurpleMine, tilePurpleFlag, tilePurpleNum1, tilePurpleNum2, tilePurpleNum3, tilePurpleNum4, tilePurpleNum5, tilePurpleNum6, tilePurpleNum7, tilePurpleNum8}}
        };

        for (int x = 0; x < width; x++) 
        {
            for (int y = 0; y < height; y++)
            {
                Cell cell = state[x, y];
                tilemap.SetTile(cell.position, GetTile(cell));
            }
        }
    }

    private Tile GetTile (Cell cell)
    {
        if (cell.revealed) {
            return GetRevealedTile(cell);
        } else if (cell.blueFlagged) {
            return tileBlueFlag;
        } else if (cell.redFlagged) {
            return tileRedFlag;
        } else if (cell.purpleFlagged) {
            return tilePurpleFlag;
        } else {
            return tileUnkown;
        }
    }

    private Tile GetRevealedTile (Cell cell)
    {
        switch (cell.type)
        {  
            case Cell.Type.Empty: return tileEmpty;
            case Cell.Type.Mine: return cell.exploded? tileExploded : coloredTiles[cell.color][0];
            case Cell.Type.Number: return coloredTiles[cell.color][cell.number+1];
            default: return null;
        }
    }
}

