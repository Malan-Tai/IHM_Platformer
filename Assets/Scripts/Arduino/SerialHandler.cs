using System;
using System.IO.Ports;
using UnityEngine;

public class SerialHandler : MonoBehaviour
{
    private SerialPort _serial;

    // Common default serial device on a Windows machine
    [SerializeField] private string serialPort = "COM1";
    [SerializeField] private int baudrate = 9600;
    
    void Start()
    {
        _serial = new SerialPort(serialPort,baudrate);
        // Once configured, the serial communication must be opened just like a file : the OS handles the communication.
        _serial.Open();
    }

    void Update()
    {
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

        switch (message)
        {
            default:
                print(message);
                break;
        }
    }

    public void SetLed(bool newState)
    {
        _serial.WriteLine(newState ? "LED ON" : "LED OFF");
    }
    
    private void OnDestroy()
    {
        _serial.Close();
    }
}
