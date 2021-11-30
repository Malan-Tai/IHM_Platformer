using System;
using System.IO.Ports;
using UnityEngine;

public class SerialHandler : MonoBehaviour
{
    private SerialPort _serial;
    private DynamicPlayer _dynamicPlayer;

    // Common default serial device on a Windows machine
    [SerializeField] private string serialPort = "COM1";
    [SerializeField] private int baudrate = 9600;

    private bool _prevDashState = true;
    private bool _prevJumpState = true;
    
    void Start()
    {
        _dynamicPlayer = GetComponent<DynamicPlayer>();

        _serial = new SerialPort(serialPort,baudrate);
        // Once configured, the serial communication must be opened just like a file : the OS handles the communication.
        _serial.Open();
    }

    void Update()
    {
        //if (_dynamicPlayer.HasAirJumpsLeft() != _prevJumpState)
        //{
        //    SetLed("JUMP", !_prevJumpState);
        //    _prevJumpState = !_prevJumpState;
        //}
        //if (_dynamicPlayer.HasDashesLeft() != _prevDashState)
        //{
        //    SetLed("DASH", !_prevDashState);
        //    _prevDashState = !_prevDashState;
        //}
        SetLed(_dynamicPlayer.HasAirJumpsLeft(), _dynamicPlayer.HasDashesLeft());

        // Prevent blocking if no message is available as we are not doing anything else
        // Alternative solutions : set a timeout, read messages in another thread, coroutines, futures...
        if (_serial.BytesToRead <= 0) return;
        
        var message = _serial.ReadLine();
        
        // Arduino sends "\r\n" with println, ReadLine() removes Environment.NewLine which will not be 
        // enough on Linux/MacOS.
        if (Environment.NewLine == "\n")
        {
            message = message.Trim('\r');
        }

        int number;
        if (int.TryParse(message, out number))
        {
            // TODO: faire qqchose avec number
            print(number);
        }
        else
        {
            switch (message)
            {
                case "JUMP":
                    if (_dynamicPlayer.CanJump) _dynamicPlayer.Jump();
                    break;
                case "DASH":
                    _dynamicPlayer.Dash(Input.GetAxis("Horizontal"));
                    break;
                case "":
                    break;
                default:
                    print(message);
                    break;
            }
        }
    }

    public void SetLed(bool jump, bool dash)
    {
        int value = 0;
        if (jump) value += 1;
        if (dash) value += 2;
        _serial.WriteLine(value.ToString());
    }
    
    private void OnDestroy()
    {
        _serial.Close();
    }
}
