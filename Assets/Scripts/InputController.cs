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

    [SerializeField]
    private Canvas _menu;

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

        if (Input.GetButtonDown("Run") && (_dynamicPlayer.Velocity.y == 0)) _dynamicPlayer.StartRunning();
        if (Input.GetButtonUp("Run")) _dynamicPlayer.StopRunning();
        if (Input.GetButtonDown("Pause")) Pause();
        if (Input.GetButtonDown("Unpause")) Unpause();
    }

    public void Pause()
    {
        Time.timeScale = 0;
        _menu.gameObject.SetActive(true);
    }

    public void Unpause()
    {
        Time.timeScale = 1;
        _menu.gameObject.SetActive(false);
    }
}
