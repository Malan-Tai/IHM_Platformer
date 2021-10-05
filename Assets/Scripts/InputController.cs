using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    private DynamicPlayer _dynamicPlayer;

    [SerializeField]
    private float _maxSpeed;
    [SerializeField]
    private float _maxRunSpeed;

    private void Start()
    {
        _dynamicPlayer = GetComponent<DynamicPlayer>();
    }

    private void Update()
    {
        if (_dynamicPlayer == null) return;

        if (!_dynamicPlayer.IsDashing) // && (!_dynamicPlayer.isTurningAround) )
        {
            if (_dynamicPlayer.IsRunning)
            {
                _dynamicPlayer.SetHorizontalSpeed(_maxRunSpeed * Input.GetAxis("Horizontal"));
            }
            else
            {
                //if ( (_dynamicPlayer.Velocity.y == 0) && ( ((_dynamicPlayer._speedPreviousPreviousFrame > 0) && (Input.GetAxis("Horizontal") < 0)) || ((_dynamicPlayer._speedPreviousPreviousFrame < 0) && (Input.GetAxis("Horizontal") > 0)) ) )
                //{
                //    _dynamicPlayer.SetHorizontalSpeed(-_dynamicPlayer._speedPreviousPreviousFrame);
                //    _dynamicPlayer.isTurningAround = true;
                //}
                //else
                //{
                    _dynamicPlayer.SetHorizontalSpeed(_maxSpeed * Input.GetAxis("Horizontal"));
                //}
            }
        }

        //if (_dynamicPlayer.isTurningAround)
        //{
        //    if ( (_dynamicPlayer.Velocity.x > 0) && ((_maxSpeed * Input.GetAxis("Horizontal") > _dynamicPlayer.Velocity.x)) )
        //    {
        //        _dynamicPlayer.SetHorizontalSpeed(_maxSpeed * Input.GetAxis("Horizontal"));
        //    }
        //    else if ((_dynamicPlayer.Velocity.x < 0) && ((_maxSpeed * Input.GetAxis("Horizontal") < _dynamicPlayer.Velocity.x)))
        //    {
        //        _dynamicPlayer.SetHorizontalSpeed(_maxSpeed * Input.GetAxis("Horizontal"));
        //    }
        //}
        
        if (Input.GetKeyDown(KeyCode.Space) && (_dynamicPlayer.CanJump)) _dynamicPlayer.Jump();
        if (Input.GetKeyUp(KeyCode.Space)) _dynamicPlayer.RestoreGravity();

        if (Input.GetKeyDown(KeyCode.C)) _dynamicPlayer.Dash(Input.GetAxis("Horizontal"));

        if (Input.GetKeyDown(KeyCode.R) && (_dynamicPlayer.Velocity.y == 0)) _dynamicPlayer.StartRunning();
        if (Input.GetKeyUp(KeyCode.R)) _dynamicPlayer.StopRunning();
    }
}
