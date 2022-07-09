using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    [SerializeField] private float _timer;
    [SerializeField] private bool _breakable;
    [SerializeField] Animator _animator;

    private bool _disabled = false;
    private bool _canReset;
    private void Start()
    {
        //_renderer = GetComponent<SpriteRenderer>();
        //_originalColor = _renderer.color;
        if(_animator!= null)
        {
            _animator.SetFloat("Timer", 1 / _timer);
        }
    }
    private void Update()
    {
        if (_disabled == true && _breakable)
        {
            StartCoroutine(nameof(PlatformEnabling));
        }
    }
    private IEnumerator PlatformEnabling()
    {
        yield return new WaitForSeconds(2*_timer);
        _disabled = false;
        _animator.SetBool("Triggered", false);

    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //PlayerController playerController = collision.collider.gameObject.GetComponent<PlayerController>();
        //if (playerController != null && _breakable)
        //{
        //    StartCoroutine(PlatformBreakDown());
        //    Destroy(gameObject);
        //}
        PlayerMarker playerMarker = collision.gameObject.GetComponent<PlayerMarker>();
        if (_breakable && playerMarker != null)
        {
            _disabled = true;
            _animator.SetBool("Triggered", true);
        }
    }
    //private void OnCollisionStay2D(Collision2D collision)
    //{
    //    PlayerMarker playerMarker = collision.gameObject.GetComponent<PlayerMarker>();
    //    if(_breakable && playerMarker != null)
    //        _disabled = true;
    //}

    //private IEnumerator PlatformBreakDown()
    //{
    //    float timer = _timer;
    //    while (timer > 0)
    //    {
    //        _renderer.color = new Color(_originalColor.x, _originalColor.y, _originalColor.z, 1 - Time.deltaTime);
    //        timer-=Time.deltaTime;
    //        //Debug.Log($"Remaining time is {timer}");
    //        yield return new WaitForSeconds(timer);
    //    }
    //    //Destroy(gameObject);
    //    //yield return null;
    //}
}
