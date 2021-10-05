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

        List<Collider2D> colliders = new List<Collider2D>();
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
                    break;
                }
            }

            if (!wind && (!thin || delta.y <= 0))
            {
                delta.x = 0;
                _hitWallPrevFrame = true;
                HitWall(ref delta);
            }
        }
        else if (_hitWallPrevFrame)
        {
            _hitWallPrevFrame = false;
            StopHittingWall();
        }

        _collider.offset = new Vector2(0, delta.y);
        if (_collider.OverlapCollider(new ContactFilter2D(), colliders) > 0)
        {
            bool thin = true;
            bool wind = true;
            float bounceImpulse = 0;
            bool bounce = false;
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

                BouncingPlatform bouncingPlatform = col.GetComponent<BouncingPlatform>();
                if (bouncingPlatform != null)
                {
                    bounce = true;
                    bounceImpulse = bouncingPlatform.bounciness;
                }

                StickyPlatform stickyPlatform = col.GetComponent<StickyPlatform>();
                if (stickyPlatform != null)
                {
                    delta.x /= stickyPlatform.stickiness;
                }
            }

            if (!wind && (!thin || delta.y < 0))
            {
                float prevVelocityY = this._velocity.y;
                Land(delta);
                if (bounce && delta.y < 0 && prevVelocityY < -5)
                {
                    this._velocity.y = -bounceImpulse * prevVelocityY;
                    delta.y = this._velocity.y * Time.deltaTime;
                }
                else delta.y = 0;
            }
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
