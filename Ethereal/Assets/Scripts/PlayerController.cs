using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Walking Speed")]
    [SerializeField] private int _speed;

    [Header("Jump Power")]
    //[Range(3000,4000)] //this is for AddForceOnly big boy number
    [SerializeField] private float _jumpPower;

    [Header("Gravity while falling")]
    [SerializeField] private float _fallingGravity;
    
    [SerializeField] private Transform _groundCheckPos;
    [SerializeField] private GameObject _sprite; //to access player sprite for flipping
    [SerializeField] private float _groundCheckRadius;
    [SerializeField] private float _jumpTime;
    [SerializeField] private int _jumpCountValue; //to set jumpcount to original value

    private Rigidbody2D _rigidbody;
    private Animator _animator;
    private Vector3 _playerScale; //to flipping the sprite
    private Vector2 _direction;
    private float _originalGravity;
    private float _jumpTimeCounter;
    private int _jumpCount; //implement
    private bool _isGrounded;
    private bool _isGroundedHazard; //for damaging platforms
    private bool _isTryingToJump = false;
    private bool _isJumping = false;
    private bool _canDoubleJump; //implement

    //Can rename to ground we can jump from or something of the sort as we add more surfaces
    public LayerMask Ground;
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _originalGravity = _rigidbody.gravityScale;
        _jumpCount = _jumpCountValue; //setting jump count back to n jumps
        _playerScale = _sprite.transform.localScale;
        _animator = GetComponent<Animator>();
    }
    private void Update()
    {
        UpdatePlayerInput();
        
        _isGrounded = Physics2D.OverlapCircle(_groundCheckPos.position, _groundCheckRadius,Ground);

        _animator.SetFloat("Speed", Mathf.Abs(_direction.x));
        
        //checking for Jump Input
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _isTryingToJump = true;
        }
        else
        {
            _isTryingToJump = false;
        }
        //if (Input.GetKeyDown(KeyCode.W))
        //{
        //    _animator.SetBool("TransitionTest", true);
        //}
        //if (Input.GetKeyDown(KeyCode.S))
        //{
        //    _animator.SetBool("TransitionTest", false);
        //}

        Debug.Log($"Is grounded: {_isGrounded}");
        Debug.Log($"Is trying to Jump: {_isTryingToJump}");

        // Setting according gravity scale
        if (_isGrounded)
        {
            _rigidbody.gravityScale = _originalGravity;
            _jumpCount = _jumpCountValue; //setting jump count back to n jumps
        }
        else
        {
            _rigidbody.gravityScale = _fallingGravity;
        }
        

        //Checking for Jump, extract method
        //_isTryingToJump + _isJumping and
        //then the same holding key or not logic for DOUBLE JUMP
        if (_isGrounded && _isTryingToJump)
        {
            Jump();
        }
        if (Input.GetKey(KeyCode.Space) && _isJumping)
        {
            ExtendedJump();
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            _isJumping = false;
        }
        //{
        //    _rigidbody.gravityScale *= _multiplier;
        //}
        FacingDirection();
    }

    private void ExtendedJump()
    {
        if (_jumpTimeCounter > 0)
        {
            _rigidbody.velocity = Vector2.up * _jumpPower;
            _jumpTimeCounter -= Time.deltaTime;
        }
        else
        {
            _isJumping = false;
        }
    }

    private void FacingDirection()
    {
        if (_direction.x < 0)
        {
            _sprite.transform.localScale = new Vector3(-_playerScale.x, _playerScale.y, _playerScale.z);
        }
        else if(_direction.x > 0)
        {
            _sprite.transform.localScale = new Vector3(_playerScale.x, _playerScale.y, _playerScale.z);
        }
        //Here later will most likely all be replaces with setBools for animaiton states from the animator, this is just a showcase of easy flipping
        
    }
    private void FixedUpdate()
    {
        _rigidbody.velocity = new Vector2(_direction.x *_speed* Time.deltaTime,_rigidbody.velocity.y);
        //For later add clear method call for groundcheck, check neon runner perhaps and do callculations through parameters
    }

    private void Jump()
    {
        //Vector2 newVelocity = new Vector2();
        //newVelocity.x = _rigidbody.velocity.x;
        //newVelocity.y = _jumpPower;
        //_rigidbody.AddForce(new Vector2(0f,_jumpPower));

        //_rigidbody.velocity = newVelocity;
        _isJumping = true;
        _jumpTimeCounter = _jumpTime;
        _rigidbody.velocity = Vector2.up * _jumpPower;
        _rigidbody.gravityScale = _fallingGravity;
        Debug.Log("Jumpy Jump");
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(_groundCheckPos.position, _groundCheckRadius);
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

        _direction = new Vector2(moveX, moveY).normalized;
    }
}
