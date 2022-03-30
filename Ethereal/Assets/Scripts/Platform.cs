using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    [SerializeField] private float _timer;

    private SpriteRenderer _renderer;
    private Vector4 _originalColor;
    private void Start()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _originalColor = _renderer.color;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        PlayerController playerController = collision.collider.gameObject.GetComponent<PlayerController>();
        if (playerController != null)
        {
            StartCoroutine(PlatformBreakDown());
            Destroy(gameObject);
        }
    }

    private IEnumerator PlatformBreakDown()
    {
        float timer = _timer;
        while (timer > 0)
        {
            _renderer.color = new Color(_originalColor.x, _originalColor.y, _originalColor.z, 1 - Time.deltaTime);
            timer-=Time.deltaTime;
            //Debug.Log($"Remaining time is {timer}");
            yield return new WaitForSeconds(timer);
        }
        //Destroy(gameObject);
        //yield return null;
    }
}
