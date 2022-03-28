using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Transform _groundCheckPos;
    [SerializeField] private GameObject _sprite;
    [SerializeField] private int _speed;
    [SerializeField] private float _groundCheckRadius;
    [SerializeField] private float _jumpPower;

    private Rigidbody2D _rigidbody;
    private Vector3 _playerScale;
    private Vector2 _direction;
    private bool _isGrounded;
    private bool _isTryingToJump = false;

    //Can rename to ground we can jump from or something of the sort as we add more surfaces
    public LayerMask Ground;
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        //_playerScale = _sprite.transform.localScale;
        
    }
    private void Update()
    {
        UpdatePlayerInput();
        
        _isGrounded = Physics2D.OverlapCircle(_groundCheckPos.position, _groundCheckRadius,Ground);
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _isTryingToJump = true;
        }
        else
        {
            _isTryingToJump = false;
        }
        Debug.Log($"Is grounded: {_isGrounded}");
        Debug.Log($"Is trying to Jump {_isTryingToJump}");
        //if (_isGrounded && _isTryingToJump)
        //if (_isGrounded && Input.GetKeyDown(KeyCode.Space)) 
        //{
        //    _rigidbody.AddForce(Vector2.up*_jumpPower,ForceMode2D.Impulse);
        //    Debug.Log("Jumpy Jump");
        //}

        //FacingDirection();
    }

    //private void FacingDirection()
    //{
    //    if (_direction.x < 0)
    //    {
    //        _sprite.transform.localScale = new Vector3(-_playerScale.x, _playerScale.y, _playerScale.z);
    //    }
    //    else
    //    {
    //        _sprite.transform.localScale = new Vector3(_playerScale.x, _playerScale.y, _playerScale.z);
    //    }
    //}
    private void FixedUpdate()
    {
        _rigidbody.velocity = _direction *_speed* Time.deltaTime;

        //For later add clear method call for groundcheck, check neon runner perhaps and do callculations through parameters
        if (_isGrounded && _isTryingToJump)
        {
            _rigidbody.AddForce(new Vector2(0f,_jumpPower));
            Debug.Log("Jumpy Jump");
            //_rigidbody.velocity += Vector2.up * _jumpPower;
        }
    }

    private void UpdatePlayerInput()
    {
        float moveX = 0f;
        float moveY = 0f;


        if (Input.GetKeyDown(KeyCode.Space))
        {
            _isTryingToJump = true;
        }
        else
        {
            _isTryingToJump = false;
        }
        //if (Input.GetKey(KeyCode.W))
        //{
        //    moveY += 1f;
        //}
        //if (Input.GetKey(KeyCode.S))
        //{
        //    moveY -= 1f;
        //}

        //Introduce jump somewhere along here as well,
        //decide whether we want physics as well
        //Remember to later switch to either Input.GetAxisRaw and/or
        //use unity's new input system for easier controller integration
        if (Input.GetKey(KeyCode.D))
        {
            moveX += 1f;
        }
        if (Input.GetKey(KeyCode.A))
        {
            moveX -= 1f;
        }
        //Debug.Log(moveX);
        _direction = new Vector2(moveX, moveY).normalized;
    }
}
