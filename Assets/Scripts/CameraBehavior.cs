using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehavior : MonoBehaviour {

	public float speed;
	private Vector3 startPosition;

	// Use this for initialization
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		//Camera direction
		if (Input.GetMouseButton(1))
		{
			float mouseX = Input.GetAxis("Mouse X") * 4;
			float mouseY = -(Input.GetAxis("Mouse Y") * 4);
			transform.Rotate(0, mouseX, 0, Space.World);
			transform.Rotate(mouseY, 0, 0, Space.Self);
		}

		if (Input.GetKey(KeyCode.W))
			transform.position = transform.position + Camera.main.transform.forward * speed;
		else if (Input.GetKey(KeyCode.S))
			transform.position = transform.position + Camera.main.transform.forward * speed * -1;

		if (Input.GetKey(KeyCode.A))
			transform.position = transform.position + Camera.main.transform.right * speed * -1;
		else if (Input.GetKey(KeyCode.D))
			transform.position = transform.position + Camera.main.transform.right * speed;

		if (Input.GetKey(KeyCode.Space))
			transform.position = transform.position + Camera.main.transform.up * speed;
		else if (Input.GetKey(KeyCode.LeftControl))
			transform.position = transform.position + Camera.main.transform.up * speed * -1;
	}
}
