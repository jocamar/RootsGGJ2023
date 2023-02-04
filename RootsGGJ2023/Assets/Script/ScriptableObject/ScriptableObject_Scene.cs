using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "Scene", menuName = "ScriptableObject/Scene", order = 1)]
public class ScriptableObject_Scene : ScriptableObject
{
    [SerializeField]
    public Scene scene;
}
