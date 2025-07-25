using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class Snake : MonoBehaviour
{
    private Vector2 _direction;
    public float iniSpeed = 10.0f;
    public float curSpeed;
    private float _timeCount;

    private List<Transform> _segments;
    public Transform segmentPrefab;
    public int InitialSize = 4;

    private void Start()
    {
        _segments = new List<Transform>();
        _segments.Add(this.transform);
        curSpeed = iniSpeed;
        _direction = Vector2.right;
        _timeCount = Time.time;
        for (int i = 0; i < InitialSize -1; i++)
        {
            Grow();
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.W))
        {
            if(_direction != Vector2.down)
            {
                _direction = Vector2.up;
            }
        } else if (Input.GetKeyDown(KeyCode.A))
        {
            if (_direction != Vector2.right)
            {
                _direction = Vector2.left;
            }
        } else if (Input.GetKeyDown(KeyCode.S))
        {
            if (_direction != Vector2.up)
            {
                _direction = Vector2.down;
            }
        } else if (Input.GetKeyDown(KeyCode.D))
        {
            if (_direction != Vector2.left)
            {
                _direction = Vector2.right;
            }
        }
    }
    private void FixedUpdate()
    {
        if(Time.time -  _timeCount >= 1/curSpeed)
        {
            _timeCount = Time.time;

            for(int i = this._segments.Count - 1; i > 0; i--)
            {
                this._segments[i].position = this._segments[i - 1].position;
                this._segments[i].gameObject.tag = "Obstacle";
            }

            this.transform.position = new Vector3(
                Mathf.Round(this.transform.position.x) + _direction.x,
                Mathf.Round(this.transform.position.y) + _direction.y,
                0.0f);
        }
    }

    private void Grow()
    {
        Transform segment = Instantiate(this.segmentPrefab);
        segment.position = this._segments[_segments.Count - 1].position;
        _segments.Add(segment);
    }

    public void ReSetstate()
    {
        for (int i = 1; i < this._segments.Count; i++)
        {
            Destroy(this._segments[i].gameObject);
        }
        _segments.Clear();
        this.transform.position = Vector3.zero;
        _segments.Add(this.transform);
        for (int i = 0; i < InitialSize-1; i++)
        {
            Grow(); 
        }
        curSpeed = iniSpeed;
        _timeCount = Time.time;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Food")
        {
            Grow();
            curSpeed += 0.5f;
        } else if(collision.tag == "Obstacle")
        {
            ReSetstate();
        }
    }
}
