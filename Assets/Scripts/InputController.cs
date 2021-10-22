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

        if (!_dynamicPlayer.IsDashing && !_dynamicPlayer.IsWallJumping)
        {
            if (_dynamicPlayer.IsRunning)
            {
                _dynamicPlayer.SetHorizontalSpeed(_maxRunSpeed * Input.GetAxis("Horizontal"));
            }
            else
            {
                _dynamicPlayer.SetHorizontalSpeed(_maxSpeed * Input.GetAxis("Horizontal"));
            }

            if (Input.GetAxis("Vertical") < -0.5f)
            {
                _dynamicPlayer.TryGettingDownThinPlatform();
            }
        }
        
        if (Input.GetButtonDown("Jump") && (_dynamicPlayer.CanJump)) _dynamicPlayer.Jump();
        if (Input.GetButtonUp("Jump")) _dynamicPlayer.RestoreGravity();

        if (Input.GetButtonDown("Dash")) _dynamicPlayer.Dash(Input.GetAxis("Horizontal"));

        if (Input.GetKeyDown(KeyCode.R) && (_dynamicPlayer.Velocity.y == 0)) _dynamicPlayer.StartRunning();
        if (Input.GetKeyUp(KeyCode.R)) _dynamicPlayer.StopRunning();
    }
}
