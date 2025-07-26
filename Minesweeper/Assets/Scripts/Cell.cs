
using UnityEngine;

public struct Cell
{
    public enum Type
    {
        EMPTY,
        MINE,
        NUM
    }

    public Type type;
    public Vector3Int pos;
    public int number;
    public bool cellRevealed;
    public bool flagged;
    public bool exploded;
}
