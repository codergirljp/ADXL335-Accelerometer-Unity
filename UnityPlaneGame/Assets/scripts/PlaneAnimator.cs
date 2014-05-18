using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using UnityEngine;
using jp.Comms;

public class PlaneAnimator : MonoBehaviour {

	/// <summary>
	/// The plane. (gameObject)
	/// </summary>
	public Transform plane;

	/// <summary>
	/// The x, y, z angles.
	/// calculated for the plane from the accelerometer data
	/// </summary>
	private float xAngle = 0.0f;
	private float yAngle = 0.0f;
	private float zAngle = 0.0f;

	/// <summary>
	/// Track the current direction the plane is going in
	/// Angle is from the Z axis going into the page that represents 
	/// a rotation about the Y axis (up/down)
	/// </summary>
	private float directionAngle = 0f;

	/// <summary>
	/// The DIRECTION_SCALER.
	/// scale down divisor used to make changes in direction smaller 
	/// than actual y-angle change used to show turning
	/// </summary>
	private const float DIRECTION_SCALER = 5;

	/// <summary>
	/// The SPEED_SCALER.
	/// Used to set the forward motion speed of the plane.
	/// </summary>
	private const float SPEED_SCALER = 20f;

	/// <summary>
	/// The message handler.
	/// Protocol object for Accelerometer 
	/// Provides functions to deserialize a message received from the Accelerometer
	/// </summary>
	private Adxl335Protocol adxlMsgHandler = new Adxl335Protocol ();

	/// <summary>
	/// The message.
	/// A message from the serial port to be deserialized by the Adxl335 protocol.
	/// </summary>
	private string message = string.Empty;

	/// <summary>
	/// The serial port manager.
	/// Manages Serial Port connection used the get data from the Accelerometer 
	/// </summary>
	private SerialPortManager serialPortManager;

	/// <summary>
	/// The COM port used for serial communications with the Adxl controller.
	/// TO DO: This should be configurable as it will be different on individual computers
	///       depending on what ports are available.
	/// </summary>
	private string COMPORT = "COM6";
	
	/// <summary>
	/// The Baud Rate used for serial communications with the Adxl controller.
	/// This must match the rate used by the controller.
	/// </summary>
	private int BAUDRATE = 38400;


	/// <summary>
	/// Start this instance.
	/// Called once when the game object is created.
	/// </summary>
	void Start () 
	{
		// Initialize the serial port manager
		// and open the serial port
		serialPortManager = new SerialPortManager (COMPORT, BAUDRATE);
	}
	

	/// <summary>
	/// Update this instance.
	/// Called once per a frame in the game loop.
	/// </summary>
	void Update () 
	{
		// do an update if the accelerometer/controller communications
	    // port is available
		if(serialPortManager != null && serialPortManager.IsAvailable()) 
		{
			// Read Serial Data
			message = serialPortManager.ReadLine ();

			// Reads the message data into adxlMsgHandler's public variables
			// xGValue, yGValue, zGValue
			adxlMsgHandler.GetData (message);
	      
			// set local rotation angles of the plane based on the 
			// data received from the accelerometer
			// xAngle tilts plane up or down
			// yAngle rotates plane around the up/down axis
			// zAngle use show the plane banking as it turns
			xAngle = (float)Math.Round (adxlMsgHandler.xGValue, 1) * 180f;
			yAngle = (float)Math.Round (adxlMsgHandler.yGValue, 1) * -180f;
			zAngle = -yAngle / 2.0f;
			plane.localRotation = Quaternion.Euler (xAngle, directionAngle + yAngle, zAngle);
				   
			// use the time delta to move things forward at a constant rate
			float deltaTime = Time.deltaTime;

			// move the plane forward
			plane.transform.localPosition += transform.forward * SPEED_SCALER * deltaTime;

			// have camera follow the plane object
			Camera.main.transform.position += transform.forward * SPEED_SCALER * deltaTime;

			// rotate camera around plane to stay near back of plane
			float rotateCamera = yAngle/DIRECTION_SCALER;
			Camera.main.transform.RotateAround (plane.position, Vector3.up, rotateCamera);
		}
	}

	/// <summary>
	/// Late update.
	/// Occurs just after an update.
	/// </summary>
	void LateUpdate()
	{
		// Calculate the direction angle for forward motion of the plane
		// Using the yAngle
		if(Math.Abs(yAngle)>0)
		{			
			// Change direction angle as plane turns in a direction
			// but at a slower rate than actual angle change received
			directionAngle += yAngle/DIRECTION_SCALER;
			
			// keep directionAngle inside of bounds
			if (directionAngle > 360f) 
			{
				directionAngle = directionAngle -360f;		
			}
			else if(directionAngle <-360f)
			{
				directionAngle = directionAngle + 360f;
			}
		}
	}

	/// <summary>
	/// End this instance.
	/// Clean up here.
	/// </summary>
	void End()
	{
		if (serialPortManager != null) 
		{
			if (serialPortManager.IsAvailable ()) 
			{
				serialPortManager.CloseSerialPort ();
			}

			serialPortManager = null;
	    }
	}
}



