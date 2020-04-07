using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
	public static bool applicationIsPaused = false;

	public GameObject pauseMenuUI;

	// Update is called once per frame
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			if (applicationIsPaused)
			{
				Resume();
			}
			else
			{
				Pause();
			}
		}
	}

	public void Resume()
	{
		pauseMenuUI.SetActive(false);
		Time.timeScale = 1f;

		applicationIsPaused = false;
	}

	private void Pause()
	{
		pauseMenuUI.SetActive(true);
		Time.timeScale = 0f;

		applicationIsPaused = true;
	}

	public void LoadMenu()
	{
		Debug.Log("Loading Menu...");
		SceneManager.LoadScene("MainMenu");
	}

	public void QuitApplication()
	{
		Debug.Log("Quitting Application...");
		Application.Quit();
	}
}
