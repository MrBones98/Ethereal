using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    [SerializeField] private float _timer;
    [SerializeField] private float _movingSpeed;
    [SerializeField] private bool _breakable;
    [SerializeField] private bool _movingPlatform;
    [SerializeField] Animator _animator;
    [SerializeField] private List<Vector3> _movingPlatformLevelPos = new List<Vector3>();

    private bool _disabled = false;
    private bool _canReset;
    private int _index;
    private Vector3 _position;
    private List<Vector3> _goalPositions = new List<Vector3>();
    private void Start()
    {
        _index = 0;
        //_renderer = GetComponent<SpriteRenderer>();
        //_originalColor = _renderer.color
        _position = transform.position;
        if (_movingPlatform)
        {
            //_goalPositions.Add(_position);
            for (int i = 0; i < _movingPlatformLevelPos.Count-1; i++)
            {
                _goalPositions.Add(_movingPlatformLevelPos[i]);
                Debug.Log(_goalPositions[i]);
            }

        }
        
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
        if (_movingPlatform == true)
        {
            MovePlatform();
        }
    }

    private void MovePlatform()
    {
        //transform.position = Vector2.Lerp(transform.position, _goalPositions[_index], _movingSpeed * Time.deltaTime);
        transform.position = Vector2.MoveTowards(transform.position, _goalPositions[_index], _movingSpeed * Time.deltaTime);
        Debug.Log(_index);
        if(Vector2.Distance(_goalPositions[_index],transform.position ) <= 0)
        {
            _index++;
        }
        if (_index != _goalPositions.Count-1)
            return;
        _goalPositions.Reverse();
        _index = 0;
        //for(int i = 1;i< _goalPositions.Count - 1; i++)
        //{
        //    Vector3 direction =  transform.position - _goalPositions[i];
        //    while(transform.position  != _goalPositions[i])
        //    {
        //        transform.position = Vector2.MoveTowards(transform.position,_goalPositions[i], _movingSpeed*Time.deltaTime);
        //    }
        //}
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
        if(_movingPlatform && playerMarker != null)
        {
            collision.gameObject.transform.SetParent(gameObject.transform, true);
        }
        if (_breakable && playerMarker != null)
        {
            _disabled = true;
            _animator.SetBool("Triggered", true);
        }
    }
    public bool GetMovingState()
    {
        return _movingPlatform;
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        PlayerMarker playerMarker = collision.gameObject.GetComponent<PlayerMarker>();
        if (_movingPlatform && playerMarker!= null)
        {
            collision.gameObject.transform.parent = null;
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
