using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Game : MonoBehaviour
{
    public AudioSource audioPlayer;
    public AudioClip crrectClip;
    public AudioClip loseClip;
    public AudioClip winClip;
    public AudioClip BGM;

    public int bombNum = 12;
    public int width = 16;
    public int height = 16;
    private Cell[,] state;

    private Board board;

    private bool gameIsOver = false;

    private void Awake()
    {
        board = GetComponentInChildren<Board>();
    }

    private void Start()
    {
        if(bombNum > width*height)
        {
            throw new System.Exception("Õ¨µ¯Ì«¶àÁË£¡");
        }
        NewGame();
        audioPlayer.GetComponent<AudioSource>().loop = true;
        audioPlayer.GetComponent<AudioSource>().Play();
    }

    private void NewGame()
    {
        GenerateCells();
        GenerateMines();
        board.Draw(state);
    }

    private void GenerateCells()
    {
        state = new Cell[width, height];
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                state[i, j].pos = new Vector3Int(i, j);
                state[i, j].type = Cell.Type.EMPTY;
                state[i, j].number = 0;
            }
        }
    }

    private void GenerateMines()
    {
        for(int i = 0; i < bombNum;)
        {
            int x = Random.Range(0, width);
            int y = Random.Range(0, height);
            if (state[x, y].type != Cell.Type.MINE)
            {
                state[x, y].type = Cell.Type.MINE;
                SetNearNum(x, y);
                i++;
            }
        }
    }

    private void SetNearNum(int x, int y)
    {
        for(int adjacentx = -1;  adjacentx <= 1;  adjacentx++)
        {
            for(int adjacenty = -1; adjacenty <= 1; adjacenty++)
            {
                if (
                    x + adjacentx >= width ||
                    x + adjacentx < 0 ||
                    y + adjacenty >= height ||
                    y + adjacenty < 0 ||
                    state[x + adjacentx, y + adjacenty].type == Cell.Type.MINE) continue;

                state[x + adjacentx, y + adjacenty].number++;
                state[x + adjacentx, y + adjacenty].type = Cell.Type.NUM;
            }
        }
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Reveal();
        } else if(Input.GetMouseButtonDown(1))
        {
            Flag();
        }

        if(Input.GetKeyDown(KeyCode.Space) && gameIsOver)
        {
            NewGame();
            gameIsOver = false;
            audioPlayer.GetComponent<AudioSource>().Play();
        }
    }

    private void Reveal()
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cellPos = board.tilemap.WorldToCell(worldPos);
        if (cellPos.x >= 0 && cellPos.x < width && cellPos.y >= 0 && cellPos.y < height)
        {
            if (state[cellPos.x, cellPos.y].cellRevealed)
            {
                return;
            } else if (state[cellPos.x, cellPos.y].type == Cell.Type.MINE)
            {
                GameOver();
                audioPlayer.GetComponent<AudioSource>().Stop();
                audioPlayer.GetComponent<AudioSource>().PlayOneShot(loseClip);
            }
            else if (state[cellPos.x, cellPos.y].type == Cell.Type.NUM)
            {
                state[cellPos.x, cellPos.y].cellRevealed = true;
                audioPlayer.GetComponent<AudioSource>().PlayOneShot(crrectClip);
                board.Renew(state[cellPos.x, cellPos.y]);
            }
            else//empty
            {
                audioPlayer.GetComponent<AudioSource>().PlayOneShot(crrectClip);
                Light(cellPos.x, cellPos.y);
            }

            if(IsWon() && !gameIsOver)
            {
                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        state[i, j].cellRevealed = true;
                    }
                }
                board.Draw(state);
                Debug.Log("YOU WIN!");
                audioPlayer.GetComponent<AudioSource>().Stop();
                audioPlayer.GetComponent<AudioSource>().PlayOneShot(winClip);
                gameIsOver = true;
            }
        }
    }

    private void Light(int x, int y)
    {
        Queue<int[]> queue = new Queue<int[]>();
        queue.Enqueue(new int[] { x, y });
        while(queue.Count > 0)
        {
            int[] pos = queue.Dequeue();
            state[pos[0], pos[1]].cellRevealed = true;
            if(state[pos[0], pos[1]].type == Cell.Type.NUM)
            {
                continue;
            }
            if (pos[0] + 1 < width 
                && state[pos[0] + 1, pos[1]].type != Cell.Type.MINE
                && !state[pos[0]+1, pos[1]].cellRevealed)
            {
                queue.Enqueue(new int[] { pos[0] + 1, pos[1] });
            }
            if (pos[1] + 1 < height
                && state[pos[0], pos[1]+1].type != Cell.Type.MINE
                && !state[pos[0], pos[1]+1].cellRevealed)
            {
                queue.Enqueue(new int[] { pos[0], pos[1]+1});
            }
            if (pos[0] - 1 >= 0
                && state[pos[0] - 1, pos[1]].type != Cell.Type.MINE
                && !state[pos[0] - 1, pos[1]].cellRevealed)
            {
                queue.Enqueue(new int[] { pos[0] - 1, pos[1] });
            }
            if (pos[1] - 1 >= 0
                && state[pos[0], pos[1]-1].type != Cell.Type.MINE
                && !state[pos[0], pos[1]-1].cellRevealed)
            {
                queue.Enqueue(new int[] { pos[0], pos[1]-1 });
            }
        }
        board.Draw(state);
    }

    private void Flag()
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cellPos = board.tilemap.WorldToCell(worldPos);
        if(cellPos.x >= 0 && cellPos.x < width && cellPos.y >= 0 && cellPos.y < height)
        {
            if (! state[cellPos.x, cellPos.y].cellRevealed)
            {
                if (state[cellPos.x, cellPos.y].flagged)
                {
                    state[cellPos.x, cellPos.y].flagged = false;
                } else
                {
                    state[cellPos.x, cellPos.y].flagged = true;
                }
                board.Renew(state[cellPos.x, cellPos.y]);
            }
        }
    }

    private void GameOver()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                state[i, j].cellRevealed = true;
                state[i, j].exploded = true;
            }
        }
        gameIsOver = true;
        Debug.Log("YOU LOSE!");
        board.Draw(state);
    }

    private bool IsWon()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (state[i, j].type != Cell.Type.MINE && !state[i, j].cellRevealed)
                {
                    return false;
                }
            }
        }

        return true;
    }
}
