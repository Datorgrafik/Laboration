using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
	#region Attributs

	public static bool applicationIsPaused = false;
	public GameObject pauseMenuUI;

	#endregion

	#region Methods

	// Update is called once per frame
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			if (applicationIsPaused)
				Resume();
			else
				Pause();
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
		SceneManager.LoadScene("MainMenu");

		// Reset KNN-Mode to false when leaving to MainMenu
		KNN.KNNMode = false;

		// Resume the pause that was activated in previous scene
		Resume();
	}

	public void QuitApplication()
	{
		Application.Quit();
	}

	#endregion
}
