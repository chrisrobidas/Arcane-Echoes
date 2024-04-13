using UnityEngine;

public class Bootstrapper : MonoBehaviour
{
    /*[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void OnBeforeSceneLoad()
    {
        Debug.Log("TestBoot");
        GameManager.InitializeGame();
    }*/

    private void Start()
    {
        GameManager.InitializeGame();
    }
}

