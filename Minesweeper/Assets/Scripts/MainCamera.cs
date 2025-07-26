using UnityEngine;
using UnityEngine.Tilemaps;

public class MainCamera : MonoBehaviour
{
    void Start()
    {
        Game gameSet = GameObject.Find("Grid").GetComponent<Game>();
        this.transform.position = new Vector3Int(gameSet.width/2, gameSet.height/2, -10);
        this.GetComponent<Camera>().orthographicSize = 
            ((gameSet.height / 2)> (gameSet.width / 4) ? gameSet.height / 2 : gameSet.width / 4) + 1;
    }
}
