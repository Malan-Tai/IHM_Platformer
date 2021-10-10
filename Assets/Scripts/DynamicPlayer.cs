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
    [SerializeField]
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

    private SpriteRenderer _spriteRenderer;
    private Vector2 _baseSpriteSize;
    private Vector2 _squishedSize;

    private Vector3 _prevVelocity;

    private void Start()
    {
        _collider = GetComponent<BoxCollider2D>();
        _baseGravity = this._gravity;

        _spriteRenderer = GetComponent<SpriteRenderer>();
        _baseSpriteSize = _spriteRenderer.size;
    }

    private new void Update()
    {
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

        if (_isDashing)
        {
            _spriteRenderer.size = _baseSpriteSize + new Vector2(Mathf.Abs(_velocity.x) * 0.02f, -Mathf.Abs(_velocity.x) * 0.005f);
        }
        else if (Mathf.Abs(this._velocity.y) > 1f)
        {
            _spriteRenderer.size = _baseSpriteSize + new Vector2(-Mathf.Abs(_velocity.y) * 0.01f, Mathf.Abs(_velocity.y) * 0.02f);
        }
        else if (Mathf.Abs(_prevVelocity.y) > 4f)
        {
            _spriteRenderer.size = _baseSpriteSize + new Vector2(Mathf.Abs(_prevVelocity.y) * 0.05f, -Mathf.Abs(_prevVelocity.y) * 0.02f);
            _spriteRenderer.size = new Vector2(_spriteRenderer.size.x, Mathf.Abs(_spriteRenderer.size.y));
            _squishedSize = _spriteRenderer.size;
        }
        else
        {
            _squishedSize.x = Mathf.Max(_squishedSize.x * 0.99f, _baseSpriteSize.x);
            _squishedSize.y = Mathf.Min(_squishedSize.y * 1.1f, _baseSpriteSize.y);
            _spriteRenderer.size = _squishedSize;
        }

        _prevVelocity = this._velocity;
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

    public void TryGettingDownThinPlatform()
    {
        float deltaY = _gravity * Time.deltaTime / 10f;

        List<Collider2D> colliders = new List<Collider2D>();

        _collider.offset = new Vector2(0, deltaY);
        if (_collider.OverlapCollider(new ContactFilter2D(), colliders) > 0)
        {
            bool thin = true;

            foreach (Collider2D col in colliders)
            {
                if (col.tag != "ThinPlatform")
                {
                    thin = false;
                    break;
                }
            }

            if (thin)
            {
                this.transform.position += new Vector3(0, deltaY, 0);
                _inThinPlatformLastFrame = true;
            }
        }
        _collider.offset = Vector2.zero;
    }
}
