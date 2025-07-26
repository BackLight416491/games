using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
public class Board : MonoBehaviour
{
    public Tilemap tilemap {  get; private set; }
    public List<Tile> tiles;

    private void Awake()
    {
        tilemap = GetComponent<Tilemap>();
    }

    public void Draw(Cell [ , ] states)
    {
        int width = states.GetLength(0);
        int height = states.GetLength(1);

        for(int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                tilemap.SetTile(states[i, j].pos, GetTile(states[i, j]));
            }
        }
    }

    private Tile GetTile(Cell cell)
    {
        if(cell.cellRevealed)
        {
            switch(cell.type)
            {
                case Cell.Type.EMPTY:
                    return tiles[1];
                case Cell.Type.MINE:
                    if(cell.exploded)
                    {
                        return tiles[2];
                    } else
                    {
                        return tiles[4];
                    }
                case Cell.Type.NUM:
                    return tiles[4 + cell.number];
                default:
                    return null;
            }
        } else if(cell.flagged)
        {
            return tiles[3];
        } else
        {
            return tiles[0];
        }
    }

    public void Renew(Cell cell)
    {
        tilemap.SetTile(cell.pos, GetTile(cell));
    }
}
