using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxScrolling : MonoBehaviour
{
    //both of these are for independent scrolling form the character
    //I'm testing shit out
    [SerializeField] private float _speed = 1.0f;
    [SerializeField] private Vector2 _direction = Vector2.left;

    private Vector3 _movement = Vector3.zero;
    private List<SpriteRenderer> _spriteRenderers = new List<SpriteRenderer>();
    private void Start()
    {
        //Add multiple backgrounds to list
        for (int i = 0; i < transform.childCount; i++)
        {
            SpriteRenderer renderer = transform.GetChild(i).GetComponent<SpriteRenderer>();
            if (renderer == null) continue;
            _spriteRenderers.Add(renderer);
            Debug.Log(_spriteRenderers[i].name);
        }
        Debug.Log(_spriteRenderers.Count);
    }
    private void Update()
    {
        _movement = (Vector3)_direction * (_speed * Time.deltaTime);
        //This autoscroll is good for the clouds
        for (int i = 0; i < _spriteRenderers.Count; i++)
        {
            _spriteRenderers[i].transform.position += _movement;
        }
    }
}
