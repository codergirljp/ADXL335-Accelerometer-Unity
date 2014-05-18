using UnityEngine;
using System.Collections;

public class PropAnimator : MonoBehaviour {

	/// <summary>
	/// The current angle. 
	/// Used ot set the current angle of rotation of the propeller.
	/// </summary>
	private int currentAngle = 315;

	/// <summary>
	/// The DELTA_ANGLE.
	/// The angle to add to the current angle each update
	/// to rotate the propeller.
	/// </summary>
	private const int DELTA_ANGLE = 40;

	
	/// <summary>
	/// Update this instance.
	/// Update is called once per frame.
	/// </summary>
	void Update () 
	{
		currentAngle+=DELTA_ANGLE; 

		if (currentAngle > 360) 
		{
			currentAngle = 0;
		}

		transform.localRotation = Quaternion.Euler (0f, (float)currentAngle, 0f);
	}
}
