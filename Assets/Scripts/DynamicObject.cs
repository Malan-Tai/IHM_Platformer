using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicObject : MonoBehaviour
{
    [SerializeField]
    protected float _gravity;

    [SerializeField]
    private float _airControl;

    [SerializeField]
    protected bool _affectedByGravity;

    [SerializeField]
    private float _noInputParabolaFriction;

    [SerializeField]
    private ParticleSystem _stickyParticle;

    protected bool _inThinPlatformLastFrame;

    protected Vector3 _velocity;
    public Vector3 Velocity
    {
        get { return _velocity; }
        protected set { _velocity = value; }
    }

    protected BoxCollider2D _collider;

    private bool _hitWallPrevFrame = false;

    private Vector3 _windForces;

    private void Start()
    {
        _collider = GetComponent<BoxCollider2D>();
        _windForces = Vector3.zero;
        _inThinPlatformLastFrame = false;
    }

    protected void Update()
    {
        Vector3 deltaV = Vector3.zero;

        if (_affectedByGravity)
        {
            deltaV.y += _gravity * Time.deltaTime;
        }
        this._velocity += deltaV;
        Vector3 delta = this._velocity * Time.deltaTime;
        Vector3 movingPlatformSpeed = Vector3.zero;

        List<Collider2D> colliders = new List<Collider2D>();
        Collider2D[] verticalColliders;

        bool currentlyInThin = false;
        if (_collider.OverlapCollider(new ContactFilter2D(), colliders) > 0)
        {
            foreach (Collider2D col in colliders)
            {
                if (col.tag == "ThinPlatform")
                {
                    currentlyInThin = true;
                    break;
                }
            }
        }

        float prevDeltaY = delta.y;

        _collider.offset = new Vector2(0, delta.y);
        if (_collider.OverlapCollider(new ContactFilter2D(), colliders) > 0)
        {
            verticalColliders = new Collider2D[colliders.Count];
            colliders.CopyTo(verticalColliders);

            bool thin = true;
            bool wind = true;
            bool moving = true;
            bool bounce = false;
            float bounceImpulse = 0;

            foreach (Collider2D col in colliders)
            {
                if (col.tag != "WindZone")
                {
                    wind = false;
                }

                if (col.tag != "ThinPlatform" && col.tag != "WindZone")
                {
                    thin = false;
                }

                BouncingPlatform bouncingPlatform = col.GetComponent<BouncingPlatform>();
                if (bouncingPlatform != null)
                {
                    bounce = true;
                    bounceImpulse = bouncingPlatform.bounciness;
                }

                StickyPlatform stickyPlatform = col.GetComponent<StickyPlatform>();
                if ((stickyPlatform != null) && (delta.x != 0))
                {
                    delta.x /= stickyPlatform.stickiness;
                    _stickyParticle.transform.position = this.transform.position;
                    _stickyParticle.gameObject.SetActive(true);
                }

                MovingPlatform movingComp = col.GetComponent<MovingPlatform>();
                if (movingComp != null)
                {
                    movingPlatformSpeed += movingComp.GetDelta();
                }
                else
                {
                    moving = false;
                }
            }

            if (wind) _inThinPlatformLastFrame = false;
            else
            {
                bool stoppedByFloor = false;
                if (!thin) stoppedByFloor = true;
                else
                {
                    if (!_inThinPlatformLastFrame && delta.y < 0)
                    {
                        stoppedByFloor = true;
                    }
                    _inThinPlatformLastFrame = true;
                }

                if (stoppedByFloor)
                {
                    float prevVelocityY = this._velocity.y;
                    _inThinPlatformLastFrame = false;
                    Land(delta);
                    if (bounce && delta.y < 0 && prevVelocityY < -15)
                    {
                        this._velocity.y = -bounceImpulse * prevVelocityY;
                        delta.y = this._velocity.y * Time.deltaTime;
                    }
                    else delta.y = 0;
                }
            }

            if (moving || (prevDeltaY < 0 && movingPlatformSpeed.y > 0))
            {
                delta += movingPlatformSpeed;
            }
        }
        else
        {
            _inThinPlatformLastFrame = false;
            verticalColliders = new Collider2D[0];
        }

        _collider.offset = new Vector2(delta.x, 0);
        if (_collider.OverlapCollider(new ContactFilter2D(), colliders) > 0)
        {
            bool thin = true;
            bool wind = true;
            foreach (Collider2D col in colliders)
            {
                if (col.tag != "WindZone")
                {
                    wind = false;
                }

                if (col.tag != "ThinPlatform")
                {
                    thin = false;
                }
            }

            if (!wind && (!thin || delta.y <= 0) && !_inThinPlatformLastFrame)
            {
                delta.x = 0;
                _hitWallPrevFrame = true;
                HitWall(ref delta);
                if (currentlyInThin || (verticalColliders.Length == 1 && verticalColliders[0] == colliders[0])) delta.y = Mathf.Min(prevDeltaY, 10 * _gravity * Time.deltaTime * Time.deltaTime);
            }
        }
        else if (_hitWallPrevFrame)
        {
            _hitWallPrevFrame = false;
            StopHittingWall();
        }

        _collider.offset = Vector2.zero;

        this.transform.position += delta;
    }

    public void SetHorizontalSpeed(float speed)
    {
        if (this._velocity.y != 0 && speed != 0) speed *= _airControl;
        else if (this._velocity.y != 0 && _windForces.x == 0) speed = this._velocity.x * _noInputParabolaFriction;

        this._velocity.x = speed;

        this._velocity += _windForces;
    }

    public virtual void Land(Vector3 delta)
    {
        this._velocity.y = 0;
    }

    public virtual void HitWall(ref Vector3 delta)
    {

    }

    public virtual void StopHittingWall()
    {

    }

    public void AddWind(Vector3 force)
    {
        _windForces += force;
    }
}
