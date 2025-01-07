
using UnityEngine;

public struct Cell
{
    public enum Type {
        Invalid,
        Empty,
        Mine,
        Number
    }

    public Vector3Int position;
    public Type type;
    public string color;
    public int number;
    public bool revealed;
    public bool blueFlagged;

    public bool redFlagged;

    public bool purpleFlagged;
    public bool exploded;

    

}
