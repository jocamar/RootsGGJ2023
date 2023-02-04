using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneGameManager : MonoBehaviour
{
    [SerializeField]
    private ScriptableObject_Scene GameScene;

    [SerializeField]
    private ScriptableObject_Scene PlayerSelectionScene;

    [SerializeField]
    private ScriptableObject_Scene MenuStartScene;

    public static SceneGameManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != null)
        {
            Debug.Log("Instance already exists, destroying object !");
            Destroy(this);
        }
    }

    private void Start()
    {
        StartMenuStartScene();
    }

    private void StartMenuStartScene()
    {
        SceneManager.LoadScene(MenuStartScene.scene, LoadSceneMode.Additive);
    }

    public void StartPlayerSelectionScene()
    {
        SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName(MenuStartScene.scene));

        GameManager.instance.StartPlayerSelection();

        SceneManager.LoadScene(PlayerSelectionScene.scene, LoadSceneMode.Additive);
    }

    public void StartGameScene()
    {
        SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName(PlayerSelectionScene.scene));

        GameManager.instance.StartGame();

        SceneManager.LoadScene(GameScene.scene, LoadSceneMode.Additive);
    }
}
