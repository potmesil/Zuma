using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    private GameData _gameData;

    [DllImport("__Internal")]
    private static extern void Reload();

    private void Awake()
    {
        _gameData = GameData.Load();

        if (_gameData != null)
        {
            transform.Find("LoadGameBtn").GetComponent<Button>().interactable = true;
        }

        Time.timeScale = 1;
    }

    public void NewGameBtn_Click()
    {
        GameData.Delete();
        SceneManager.LoadScene(1);
    }

    public void LoadGameBtn_Click()
    {
        SceneManager.LoadScene(_gameData.SceneIndex);
    }

    public void QuitBtn_Click()
    {
		#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
		#elif UNITY_WEBGL
			Reload();
		#else
			Application.Quit();
		#endif
    }
}