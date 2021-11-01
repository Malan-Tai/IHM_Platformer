using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicPlayer : DynamicObject
{
    [SerializeField]
    private ParticleSystem _landingParticle;

    [SerializeField]
    private ParticleSystem _wallParticleRight;
    [SerializeField]
    private ParticleSystem _wallParticleLeft;

    [SerializeField]
    private ParticleSystem _deathParticle;

    [SerializeField]
    private ShakeBehavior _camera;

    [SerializeField]
    private float _jumpImpulse;
    [SerializeField]
    private float _wallJumpImpulse;

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
    [SerializeField]
    private float _maxSquish;
    [SerializeField]
    private float _minSquish;

    [SerializeField]
    private float _persistentImageSpawnTime;
    private float _currentImageSpawnTime = 0f;

    [SerializeField]
    private Color _dashColor;
    private Color _baseColor;
    [SerializeField]
    private float _maxColorTimer;
    private float _currentColorTimer = 0f;

    private Vector3 _prevVelocity;

    [SerializeField]
    private float _maxWallJumpTimer;
    private float _currentWallJumpTimer = 0f;
    public bool IsWallJumping
    {
        get => _currentWallJumpTimer < _maxWallJumpTimer;
    }

    [SerializeField]
    private AudioSource _jumpSound;
    [SerializeField]
    private AudioSource _deathSound;
    [SerializeField]
    private AudioSource _dashSound;

    private void Start()
    {
        _collider = GetComponent<BoxCollider2D>();
        _baseGravity = this._gravity;

        _spriteRenderer = GetComponent<SpriteRenderer>();
        _baseSpriteSize = _spriteRenderer.size;
        _baseColor = _spriteRenderer.color;
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

        float prevAbsSpeed = Mathf.Abs(_prevVelocity.y);
        float curAbsSpeed = Mathf.Abs(this._velocity.y);

        Vector2 newSpriteSize;
        if (_isDashing || (_isRunning && curAbsSpeed < 1f)) // horizontal speed
        {
            newSpriteSize = _baseSpriteSize + new Vector2(Mathf.Abs(_velocity.x) * 0.005f, -Mathf.Abs(_velocity.x) * 0.005f);
        }
        else if (curAbsSpeed > 2f && prevAbsSpeed > 3f) // vertical speed
        {
            newSpriteSize = _baseSpriteSize + new Vector2(-curAbsSpeed * 0.01f, curAbsSpeed * 0.02f);
        }
        else if (prevAbsSpeed > 4f) // landing
        {
            newSpriteSize = _baseSpriteSize + new Vector2(prevAbsSpeed * 0.05f, -prevAbsSpeed * 0.02f);
            newSpriteSize = new Vector2(newSpriteSize.x, Mathf.Abs(newSpriteSize.y));
        }
        else
        {
            newSpriteSize = _squishedSize + (_baseSpriteSize - _squishedSize) * Time.deltaTime * 10f;
        }

        newSpriteSize.x = Mathf.Clamp(newSpriteSize.x, _minSquish, _maxSquish);
        newSpriteSize.y = Mathf.Clamp(newSpriteSize.y, _minSquish, _maxSquish);

        _spriteRenderer.size = newSpriteSize;
        _squishedSize = newSpriteSize;

        //if (this._velocity.magnitude > 10)
        _currentImageSpawnTime += Time.deltaTime;

        if (_currentImageSpawnTime >= _persistentImageSpawnTime)
        {
            _currentImageSpawnTime = 0f;
            PersistentImageFactory.Instance.CreateImage(this.transform.position, _squishedSize);
        }

        if (_currentWallJumpTimer < _maxWallJumpTimer) _currentWallJumpTimer += Time.deltaTime;

        _prevVelocity = this._velocity;

        if (_spriteRenderer.color != _baseColor)
        {
            _currentColorTimer += Time.deltaTime;
            if (_currentColorTimer >= _maxColorTimer)
            {
                _currentColorTimer = 0f;
                _spriteRenderer.color = _baseColor;
            }
        }
    }

    public void Jump()
    {
        bool doJump = false;
        if ((this._velocity.y != 0 && _coyoteTimer > _maxCoyoteTime) && _currentAirJumps < _airJumpNumber && _hitWallDirection == 0f)
        {
            _currentAirJumps++;
            _spriteRenderer.color = _dashColor;
            doJump = true;
        }
        else if (this._velocity.y == 0 || _hitWallDirection != 0f || _coyoteTimer <= _maxCoyoteTime) doJump = true;

        if (doJump)
        {
            _jumpSound.Play();
            this._gravity = _loweredGravity;
            this._velocity.y = _jumpImpulse;
            _currentWallJumpTimer = _maxWallJumpTimer;
            if (_hitWallDirection != 0f)
            {
                this._velocity.x = -_hitWallDirection / Mathf.Abs(_hitWallDirection) * _wallJumpImpulse;
                _currentWallJumpTimer = 0f;
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

        _dashSound.Play();
        float directionNormalized = Mathf.Sign(direction);
        _isDashing = true;
        _canJump = false;
        _affectedByGravity = false;
        this._velocity.y = 0;
        SetHorizontalSpeed(40 * directionNormalized);
        _currentDash++;

        _camera.TriggerShake();
        _spriteRenderer.color = _dashColor;
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

            float y = Mathf.Abs(delta.y);
            if (y * 100 > 5)
            {
                Instantiate(_landingParticle, this.transform.position + new Vector3(0, -0.5f, 0), Quaternion.identity);
            }
            if (y * 10 > 3)
            {
                _camera.TriggerShake();
            }
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

            if (_hitWallDirection > 0) // je vais à droite
            {
                _wallParticleRight.transform.position = this.transform.position;
                _wallParticleRight.gameObject.SetActive(true);
            }
            else
            {
                _wallParticleLeft.transform.position = this.transform.position;
                _wallParticleLeft.gameObject.SetActive(true);
            }

            delta.y /= _wallGrabStrength;
            this._velocity.y /= _wallGrabStrength;
        }
    }

    public override void StopHittingWall()
    {
        print("stop");
        if (_hitWallDirection == 0f) return;

        _prevHitWallDirection = _hitWallDirection;
        _hitWallDirection = 0f;
        print("do stop");
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

    public override void Die()
    {
        Instantiate(_deathParticle, this.transform.position, Quaternion.identity);
        this.transform.position = new Vector3(-3, 19, 0);
        this.Velocity = Vector3.zero;
        this._currentAirJumps = 0;
        this._currentDash = 0;
        this.IsDashing = false;
        this.IsRunning = false;
        this._hitWallDirection = 0;
        this._prevHitWallDirection = 0;
        _deathSound.Play();
        
        base.Die();
    }
}
