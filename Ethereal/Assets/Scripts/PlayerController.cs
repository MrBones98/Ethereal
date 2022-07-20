using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Walking Speed")]
    [SerializeField] private int _walkingSpeed;

    [Header("Running Speed")]
    [SerializeField] private int _runningSpeed;

    [Header("Jump Power")]
    //[Range(3000,4000)] //this is for AddForceOnly big boy number
    [SerializeField] private float _jumpPower;
    [SerializeField] private float _horizontalWallForce;
    [SerializeField] private float _verticalWallForce;
    [SerializeField] private float _wallJumpDuration;

    [Header("Gravity while falling")]
    [SerializeField] private float _fallingGravity;
    
    [Header("Collision Check")]
    [SerializeField] private Transform _groundCheckPos;
    [SerializeField] private Transform _frontCheckPos;
    [SerializeField] private float _groundCheckRadius;
    [SerializeField] private float _wallCheckRadius;
    [SerializeField] private Transform _grappleHookDirection;

    [SerializeField] private GameObject _sprite; //to access player sprite for flipping
    [SerializeField] private float _jumpTime;
    [SerializeField] private int _dashLenght; //leave at 4
    [SerializeField] private int _jumpCountValue; //to set jumpcount to original value

    [SerializeField] [Range(0.2f, 1.0f)] private float _dashDuration;
    [SerializeField] [Range(0.2f, 1.0f)] private float _dashCooldown;
    [SerializeField] [Range(0.5f, 0.9f)] private float _controllerAnalogRunningValue;

    [Header("Pause Menu Canvas")]
    [SerializeField] private GameObject _pauseMenu;

    [Header("Miscellaneous UI Elements")]
    [SerializeField] private GameObject _prompts;

    [Header("Dash Particles")]
    [SerializeField] private GameObject _blinkSmoke;
    [SerializeField] private Transform _blinkSmokePos;

    private Rigidbody2D _rigidbody;
    private Animator _animator;
    private Vector3 _playerScale; //to flipping the sprite
    private Vector2 _direction;
    private Vector2 _input;
    private Vector2 _grappleDirection;
    private float _wallJumpCount;
    private float _timeSinceDash;
    private float _originalGravity;
    private float _jumpTimeCounter;
    private float _grappleDistance;
    private int _speed;
    private int _jumpCount; //implement
    private bool _isInputEnabled = true;
    private bool _isTryingToJump = false;
    private bool _isTryingToDash = false;
    private bool _isTryingToGrapple = false;
    private bool _holdingGrapple = false;
    private bool _canMove = true;
    //private bool _isDashing = false;
    private bool _isJumping = false;
    private bool _isGrounded;
    private bool _wallCollision;
    private bool _hangingFromWall = false;
    private bool _isWallJumping = false;
    private bool _isGroundedHazard; //for damaging platforms
    //private bool _canDoubleJump; //implement
    private PlayerControls _playerControls;
    //Can rename to ground we can jump from or something of the sort as we add more surfaces
    public LayerMask Ground;
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _originalGravity = _rigidbody.gravityScale;
        _jumpCount = _jumpCountValue; //setting jump count back to n jumps
        _playerScale = _sprite.transform.localScale;
        _animator = GetComponent<Animator>();
        _speed = _walkingSpeed;
        _playerControls = new PlayerControls();
    }
    private void Start()
    {
        _timeSinceDash = 0f;
        _wallJumpCount = 0f;
        _grappleDistance = Vector2.Distance(transform.position, _grappleHookDirection.position);
    }
    private void OnEnable()
    {
        _playerControls.Enable();
    }
    private void Update()
    {
        UpdatePlayerInput();

        //Checking for pause menu interaction
        if (_playerControls.Base.OpenMenu.triggered)
        {
            if (_pauseMenu.activeSelf == false)
            {
                PauseGame();
            }
            else
            {
                ResumeGame();
            }
        }
        //Yeah fucking add layers for wall collision and might as well for grappling ones
        _isGrounded = Physics2D.OverlapCircle(_groundCheckPos.position, _groundCheckRadius, Ground);
        _wallCollision = Physics2D.OverlapCircle(_frontCheckPos.position, _wallCheckRadius, Ground);

        //Debug.Log(_isTryingToGrapple);

        if (_playerControls.Base.Grapple.triggered)
        {
            _isTryingToGrapple = true;
            //Debug.Log(CanGrapple());
        }
        else
        {
            _isTryingToGrapple = false;
        }
        if(_playerControls.Base.Grapple.ReadValue<float>() == 1)
        {
            _holdingGrapple = true;
        }
        else if (_playerControls.Base.Grapple.WasReleasedThisFrame())
        {
            _holdingGrapple = false;
        }
        
        //Might be unnecessary check it outover the weekend now that I'm goiung over grappling agaaaain CHAO MAMAHUEVO ESTO NONO AH EHHH
        //if (_isTryingToGrapple)
        //{
        //    _animator.SetBool("GrapplingStarted", true);
        //}
        //else
        //{
        //    _animator.SetBool("GrapplingStarted", false);
        //}
        //for idle/walk/running animations. Do the same for jumping with _direction.y?/_rigidbody.velocity.y maybe
        _animator.SetFloat("Speed", Mathf.Abs(_direction.x));
        _animator.SetBool("Landed", _isGrounded);
        _animator.SetBool("IsBlinking", IsDashing());
        _animator.SetBool("IsHangingFromWall", _hangingFromWall);

        //Same, check if it's redundaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaant
        _animator.SetBool("GrapplingInput", _isTryingToGrapple);
        _animator.SetBool("HoldingGrapple", _holdingGrapple);


        //checking for Jump Input
        if (_playerControls.Base.Jump.triggered)
        {
            _isTryingToJump = true;
        }
        else
        {
            _isTryingToJump = false;
        }
        if (_rigidbody.velocity.y > 0 && _isGrounded == false)
        {
            _animator.SetBool("IsJumping", true);
            _animator.SetBool("IsFalling", false);
        }
        else if (_rigidbody.velocity.y < 0 && !_hangingFromWall)
        {
            _animator.SetBool("IsFalling", true);
            _animator.SetBool("IsJumping", false);
        }
        else
        {
            _animator.SetBool("IsJumping", false);
            _animator.SetBool("IsFalling", false);
        }

        if (_playerControls.Base.Blink.triggered && CanDash())
        {
            _isTryingToDash = true;
        }

        _timeSinceDash += Time.deltaTime;

        if (_input.x > _controllerAnalogRunningValue || _input.x < -_controllerAnalogRunningValue)
        {
            _speed = _runningSpeed;
            _animator.SetBool("Sprint", true);
        }
        else
        {
            _speed = _walkingSpeed;
            _animator.SetBool("Sprint", false);
        }

        // Setting according gravity scale
        if (_isGrounded)
        {
            _rigidbody.gravityScale = _originalGravity;
            _jumpCount = _jumpCountValue; //setting jump count back to n jumps
            _hangingFromWall = false;
        }
        else if (IsDashing())
        {
            _rigidbody.gravityScale = 0;
        }
        else
        {
            _rigidbody.gravityScale = _fallingGravity;
        }

        //Doing Wall Jump check
        //if(!_isGrounded && _wallCollision && _input.x !=0 && !_isWallJumping)
        if( _wallCollision && _input.x !=0 && !_isWallJumping)
        {
            _hangingFromWall = true;
            //Debug.Log($"Wall collision is: {_wallCollision}");
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, 0);
            //_rigidbody.gravityScale = 0;
        }
        else
        {
            _hangingFromWall = false;
        }
        //Debug.Log($"The player is hanging from wall: {_hangingFromWall}");
        
        //Checking for Jump
        JumpInputCheck();
        //BRRRRRRRRRROOOOOOOOOOOOOOOOOO FIX THIS WTFFFFF RIGIDBODY MAAAAAAN COÑOOOOOOOOOO
        if (_isTryingToGrapple)
        {
            Grapple();
        }
        FacingDirection();
        //Debug.Log(_hangingFromWall);
        //Debug.Log($"The player is trying to dash: {_isTryingToDash}");
        //Debug.Log($"The player can dash: {CanDash()}");
        //Debug.Log($"Is the player dashing? {IsDashing()}");
        //Debug.Log(_rigidbody.velocity);
    }

    private void JumpInputCheck()
    {
        // && isGrounded later to avoid dumb bugs and to reduce _jumpcount if players drops off a platform
        if (_isTryingToJump && _jumpCount > 0 && !_hangingFromWall)
        {
            Debug.Log("The dude is normal jumping");
            Jump();
        }
        //if (Input.GetKey(KeyCode.Space) && _isJumping)
        if (_playerControls.Base.Jump.ReadValue<float>() == 1 && _isJumping && !_hangingFromWall)
        {
            ExtendedJump();
        }
        //if (Input.GetKeyUp(KeyCode.Space))
        if (_playerControls.Base.Jump.WasReleasedThisFrame() && !_hangingFromWall)
        {
            _isJumping = false;
            _jumpCount--;
        }
        if(_rigidbody.velocity.y < 0 && _jumpCount == _jumpCountValue)
        {
            _jumpCount--;
        }

        if(_isTryingToJump && _hangingFromWall)
        {
            _isWallJumping = true;
            _jumpCount++;
        }
        if (_isTryingToJump && _isWallJumping)
        {
            _isInputEnabled = false;
            WallJump();
            _isWallJumping=false;
            _isInputEnabled = true;
        }
    }

    //private void WallJumpSetter()
    //{
    //    _isWallJumping = false;
    //}

    private void WallJump()
    {
        Debug.Log("Going into the WallJump function, AHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHH");
        //float direction = _input.x;
        //_rigidbody.velocity = new Vector2(_horizontalWallForce * -direction, _verticalWallForce);
        //Debug.Log(_rigidbody.velocity);
        _rigidbody.AddForce(new Vector2(_horizontalWallForce * -transform.localScale.x, _verticalWallForce),ForceMode2D.Impulse);
        Debug.Log($"The wall jump velocity is {_rigidbody.velocity}");
        if(_jumpCount > 0)
        {
            _jumpCount += 1;
        }
        //Vector2 wallJumpForce;

        //wallJumpForce.x = _horizontalWallForce * transform.localScale.x;
        //wallJumpForce.y = _verticalWallForce;

        //_rigidbody.AddForce(wallJumpForce, ForceMode2D.Impulse);
    }

    private IEnumerator SetLanding()
    {
        _animator.SetTrigger("Land");
        yield return new WaitForSeconds(1.0f);
       
    }

    private void FixedUpdate()
    {
        //_rigidbody.velocity = new Vector2(_direction.x *_speed* Time.deltaTime,_rigidbody.velocity.y);
        //if(_isTryingToGrapple && CanGrapple())
        //{
        //    _rigidbody.gravityScale = 0f;
        //    _rigidbody.velocity = Vector2.MoveTowards(transform.position, _grappleDirection, _grappleDistance);
        //    //_rigidbody.MovePosition(_grappleDirection);
        //    ////_grappleDirection = new Vector2(0f, 0f);
        //    _rigidbody.gravityScale = _originalGravity;
        //}
        //if (!IsDashing() && _isInputEnabled && !_isWallJumping && !_isTryingToGrapple)
        //if (_isTryingToGrapple)
        //{
        //    Grapple();
        //}
        if (!IsDashing() && _isInputEnabled && !_isWallJumping && !_holdingGrapple)
        {
            _rigidbody.velocity = new Vector2(_direction.x *_speed* Time.deltaTime,_rigidbody.velocity.y);
                if (_isTryingToDash && CanDash() && !_hangingFromWall)
                {
                    Instantiate(_blinkSmoke, _blinkSmokePos);
                    StartDash();
                    //_animator.SetBool("IsBlinking", false);
                }
        }
    }

    //private Vector2 CanGrapple()
    //{
    //    float distance = Vector2.Distance(transform.position, _grappleHookDirection.position);
    //    RaycastHit2D hit = Physics2D.Raycast(transform.position, _grappleHookDirection.position, distance, Ground);
    //    if (hit.collider != null)
    //    {
    //        return _grappleDirection = transform.position - hit.collider.transform.position;
            
    //    }
    //    else { return; }
    //}
    private void Grapple()
    {
        //Add a different Layer for the grapplingHook allowed platforms
        float distance = Vector2.Distance(transform.position, _grappleHookDirection.position);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, _grappleHookDirection.position, distance, Ground);
        Debug.Log("Grappling");
        if (hit && _holdingGrapple)
        {
            Debug.Log(hit);
            Debug.Log(hit.collider.gameObject.name);
            _rigidbody.velocity = Vector2.Lerp(transform.position, hit.point, 0.5f);
        }
    }
    private void StartDash()
    {
        Vector2 dashPosition = ((Vector2)transform.position + new Vector2(_sprite.transform.localScale.x,0) * _dashLenght);
        Vector2 dashVelocity = (dashPosition - (Vector2)transform.position) / _dashDuration;

        _rigidbody.velocity = dashVelocity;
        _timeSinceDash = 0;
        _isTryingToDash = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(_groundCheckPos.position, _groundCheckRadius);
        Gizmos.DrawSphere(_frontCheckPos.position, _wallCheckRadius);
        Gizmos.DrawLine(transform.position, _grappleHookDirection.position);
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
        if (!_holdingGrapple)
        {
            if (_direction.x < 0)
            {
                _sprite.transform.localScale = new Vector3(-_playerScale.x, _playerScale.y, _playerScale.z);
            }
            else if(_direction.x > 0)
            {
                _sprite.transform.localScale = new Vector3(_playerScale.x, _playerScale.y, _playerScale.z);
            }
        }
        //Here later will most likely all be replaces with setBools for animaiton states from the animator, this is just a showcase of easy flipping
        
    }

    private void Jump()
    {
        _isJumping = true;
        _jumpTimeCounter = _jumpTime;
        _rigidbody.velocity = Vector2.up * _jumpPower;
        _rigidbody.gravityScale = _fallingGravity;
    }
    public void PauseGame()
    {
        _prompts?.SetActive(false);
        _pauseMenu?.SetActive(true);
        Time.timeScale = 0;
    }
    public void ResumeGame()
    {

        _pauseMenu?.SetActive(false);
        _prompts?.SetActive(true);
        Time.timeScale = 1;
    }
    private void UpdatePlayerInput()
    {
        _input = _playerControls.Base.Navigation.ReadValue<Vector2>();
        float moveX = 0f;
        float moveY = 0f;
        if(_playerControls.Base.Jump.WasPressedThisFrame())
        {
            _isTryingToJump = true;
        }
        else
        {
            _isTryingToJump = false;
        }
        if (_isInputEnabled)
        {
            //Movement direction check for sprite flip
            if(_input.x > 0)
            {
                moveX += 1f;
            }
            if (_input.x < 0)
            {
                moveX -= 1f;
            }
        }

        _direction = new Vector2(moveX, moveY).normalized;
    }
    private bool CanDash()
    {
        return _timeSinceDash >= _dashCooldown;
    }
    private bool IsDashing()
    {
        return _timeSinceDash <= _dashDuration;
    }
    private void OnDisable()
    {
        _playerControls.Disable();
    }
}
