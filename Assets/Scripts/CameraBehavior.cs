using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraBehavior : MonoBehaviour 
{
	#region Attributs

	public float speed;
	public static bool teleportCamera = false;

	#endregion

	#region Methods

	// FixedUpdate to update a certain amount of times per second
	void FixedUpdate()
	{
		//Camera direction
		if (Input.GetMouseButton(1) && MainMenu.renderMode == 1)
		{
			float mouseX = Input.GetAxis("Mouse X") * 16;
			float mouseY = -(Input.GetAxis("Mouse Y") * 16);

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

	public static void RefocusCamera(List<Dictionary<string, object>> pointList)
	{
		GameObject newBall = (GameObject)pointList.Last()["DataBall"] as GameObject;
        if (SceneManager.GetActiveScene().name != "ScatterPlotMatrix")
        {
            if (MainMenu.renderMode == 1)
                Camera.main.transform.position = new Vector3(newBall.transform.position.x + 2.5f, newBall.transform.position.y + 1.5f, newBall.transform.position.z - 2.5f);
            else
                Camera.main.transform.position = new Vector3(newBall.transform.position.x, newBall.transform.position.y, newBall.transform.position.z - 8f);

            Camera.main.transform.LookAt(newBall.transform);
        }
		if (TargetingScript.selectedTarget != null)
		{
			TargetingScript.selectedTarget.GetComponent<Renderer>().material.color = TargetingScript.colorOff;
			TargetingScript.selectedTarget.transform.localScale += new Vector3(-0.01f, -0.01f, -0.01f);
		}

		TargetingScript.selectedTarget = newBall;
		TargetingScript.colorOff = TargetingScript.selectedTarget.GetComponent<Renderer>().material.color;
		TargetingScript.selectedTarget.GetComponent<Renderer>().material.color = Color.white;
		TargetingScript.selectedTarget.transform.localScale += new Vector3(+0.01f, +0.01f, +0.01f);
	}

	#endregion
}
