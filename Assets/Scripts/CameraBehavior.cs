﻿using UnityEngine;

public class CameraBehavior : MonoBehaviour 
{
	#region Attributs

	public float speed;

	#endregion

	#region Methods

	// Update is called once per frame
	void Update()
	{
		//Camera direction
		if (Input.GetMouseButton(1) && MainMenu.renderMode == 1)
		{
			float mouseX = Input.GetAxis("Mouse X") * 4;
			float mouseY = -(Input.GetAxis("Mouse Y") * 4);

			transform.Rotate(0, mouseX, 0, Space.World);
			transform.Rotate(mouseY, 0, 0, Space.Self);
		}

		if (MainMenu.renderMode == 0)
		{
			if (Input.GetKey(KeyCode.UpArrow))
				transform.position = transform.position + Camera.main.transform.forward * speed;
			else if (Input.GetKey(KeyCode.DownArrow))
				transform.position = transform.position + Camera.main.transform.forward * speed * -1;
			
			if (Input.GetKey(KeyCode.W))
				transform.position = new Vector3(transform.position.x, transform.position.y + speed, transform.position.z);
			else if (Input.GetKey(KeyCode.S))
				transform.position = new Vector3(transform.position.x, transform.position.y + speed * -1, transform.position.z);
		}
		else if (MainMenu.renderMode == 1)
		{
			if (Input.GetKey(KeyCode.W))
				transform.position = transform.position + Camera.main.transform.forward * speed;
			else if (Input.GetKey(KeyCode.S))
				transform.position = transform.position + Camera.main.transform.forward * speed * -1;

			if (Input.GetKey(KeyCode.Space))
				transform.position = new Vector3(transform.position.x, transform.position.y + speed, transform.position.z);
			else if (Input.GetKey(KeyCode.LeftControl))
				transform.position = new Vector3(transform.position.x, transform.position.y + speed * -1, transform.position.z);
		}

		if (Input.GetKey(KeyCode.A))
			transform.position = transform.position + Camera.main.transform.right * speed * -1;
		else if (Input.GetKey(KeyCode.D))
			transform.position = transform.position + Camera.main.transform.right * speed;
	}

	#endregion
}
