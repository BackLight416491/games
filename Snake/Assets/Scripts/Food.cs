using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
    public BoxCollider2D gridArea;
    private float _timeCount;
    public float sparkTime = 0.2f;

    private void Start()
    {
        RandomizePos();
    }

    private void FixedUpdate()
    {
        if (Time.time - _timeCount >= sparkTime)
        {
            _timeCount = Time.time;
            GetComponent<Renderer>().enabled = !GetComponent<Renderer>().enabled;
        }
    }

    private void RandomizePos()
    {
        Bounds bounds = this.gridArea.bounds;

        int x = Random.Range((int)bounds.min.x, (int)bounds.max.x);
        int y = Random.Range((int)bounds.min.y, (int)bounds.max.y);

        this.transform.position = new Vector3(x, y, 0.0f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player" || collision.tag == "Obstacle")
        {
            RandomizePos();
        }
    }
}
