using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicPlayer : DynamicObject
{
    [SerializeField]
    private float _jumpImpulse;

    [SerializeField]
    private int _airJumpNumber;
    private int _currentAirJumps = 0;

    [SerializeField]
    private int _dashNumber;
    private int _currentDash;

    private float _baseGravity;
    [SerializeField]
    private float _loweredGravity;

    private bool _isDashing = false;
    public bool IsDashing
    {
        get { return _isDashing; }
        private set { _isDashing = value; }
    }
    private float _dashDurationLimit = 0.1f;
    private float _dashDuration = 0;

    private bool _canJump = true;
    public bool CanJump
    {
        get { return _canJump; }
        private set { _canJump = value; }
    }

    private bool _isRunning = false;
    public bool IsRunning
    {
        get { return _isRunning; }
        private set { _isRunning = value; }
    }
    private float _runningTimerMax = 1;
    private float _runningTimer;

    [SerializeField]
    private float _wallGrabStrength;
    private float _hitWallDirection;
    private float _prevHitWallDirection;

    [SerializeField]
    private float _maxCoyoteTime;
    private float _coyoteTimer;

    //public float _speedPreviousFrame;
    //public float _speedPreviousPreviousFrame;
    //public bool isTurningAround = false;
    //private float _turnAroundTimerMax = 0.2f;
    //private float _turnAroundTimer;

    private void Start()
    {
        _collider = GetComponent<BoxCollider2D>();
        _baseGravity = this._gravity;
    }

    private new void Update()
    {
        //_speedPreviousPreviousFrame = _speedPreviousFrame;
        //_speedPreviousFrame = _velocity.x;

        base.Update();
        if (Velocity.y <= 0)
        {
            RestoreGravity();
        }

        _coyoteTimer += Time.deltaTime;

        if (_isDashing)
        {
            if (_dashDuration < _dashDurationLimit)
            {
                _dashDuration += Time.deltaTime;
            }
            else
            {
                _isDashing = false;
                _affectedByGravity = true;
                _canJump = true;
                _dashDuration = 0;
            }
        }

        //if (isTurningAround)
        //{
        //    if (_turnAroundTimer < _turnAroundTimerMax)
        //    {
        //        _turnAroundTimer += Time.deltaTime;
        //    }
        //    else
        //    {
        //        isTurningAround = false;
        //        _turnAroundTimer = 0;
        //    }
        //}

        if (_isRunning)
        {
            if (_runningTimer < _runningTimerMax)
            {
                _runningTimer += Time.deltaTime;
            }
            else
            {
                this.StopRunning();
            }
        }
    }

    public void Jump()
    {
        bool doJump = false;
        if ((this._velocity.y != 0 && _coyoteTimer > _maxCoyoteTime) && _currentAirJumps < _airJumpNumber && _hitWallDirection == 0f)
        {
            _currentAirJumps++;
            doJump = true;
        }
        else if (this._velocity.y == 0 || _hitWallDirection != 0f || _coyoteTimer <= _maxCoyoteTime) doJump = true;

        if (doJump)
        {
            this._gravity = _loweredGravity;
            this._velocity.y = _jumpImpulse;
            if (_hitWallDirection != 0f)
            {
                this._velocity.x = -_hitWallDirection / Mathf.Abs(_hitWallDirection) * _jumpImpulse;
            }
        }
    }

    public void RestoreGravity()
    {
        _gravity = _baseGravity;
    }

    public void Dash(float direction)
    {
        if ((direction == 0) || (_currentDash >= _dashNumber)) return;

        List<Collider2D> colliders = new List<Collider2D>();
        if (_collider.OverlapCollider(new ContactFilter2D(), colliders) > 0)
        {
            foreach (Collider2D col in colliders)
            {
                if (col.tag != "WindZone") return;
            }
        }

        float directionNormalized = Mathf.Sign(direction);
        _isDashing = true;
        _canJump = false;
        _affectedByGravity = false;
        this._velocity.y = 0;
        SetHorizontalSpeed(40 * directionNormalized);
        _currentDash++;
    }

    public void StartRunning()
    {
        _isRunning = true;
    }

    public void StopRunning()
    {
        _isRunning = false;
        _runningTimer = 0;
    }

    public override void Land(Vector3 delta)
    {
        if (delta.y < 0)
        {
            _currentAirJumps = 0;
            _currentDash = 0;
            _prevHitWallDirection = 0f;
            _hitWallDirection = 0f;
            _coyoteTimer = 0;
        }
        base.Land(delta);
    }

    public override void HitWall(ref Vector3 delta)
    {
        if (_wallGrabStrength != 0f && _prevHitWallDirection * this._velocity.x <= 0f)
        {
            _currentAirJumps = 0;
            _currentDash = 0;
            _hitWallDirection = this._velocity.x;

            delta.y /= _wallGrabStrength;
            this._velocity.y /= _wallGrabStrength;
        }
    }

    public override void StopHittingWall()
    {
        if (_hitWallDirection == 0f) return;

        _prevHitWallDirection = _hitWallDirection;
        _hitWallDirection = 0f;
    }
}
