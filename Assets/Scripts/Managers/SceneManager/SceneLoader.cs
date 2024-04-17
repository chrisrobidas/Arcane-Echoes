using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static event Action<EScenes> OnSceneLoaded;
    public static event Action<EScenes> OnSceneUnload;
    public static event Action<bool> FadeInOutScreen;
    public static event Action OnSceneGroupLoadStart;
    public static event Action OnSceneGroupLoadEnd;

    public static LoadingProgress LoadingProgress => m_loadingProgress;
    private static LoadingProgress m_loadingProgress = new LoadingProgress();

    public static async void LoadScenes(EScenes activeScene, EScenes scenesToLoad, bool reloadDuplicate, Action actionAfterLoad = null)
    {
        FadeInOutScreen?.Invoke(true);
        Debug.Log(SceneLoadingScreen.FadeInOutDuration);
        await Task.Delay(TimeSpan.FromSeconds(SceneLoadingScreen.FadeInOutDuration));
        OnSceneGroupLoadStart?.Invoke();

        //Add active scene if not present in scenes to load
        if (!scenesToLoad.HasFlag(activeScene))
        {
            scenesToLoad |= activeScene;
        }
#if UNITY_EDITOR
        Debug.Log($"<b>[SceneLoader]</b> Scene load requested : ({scenesToLoad})");
#endif
        // Get scenes already loaded and unused
        EScenes loadedScenes = 0;
        EScenes unusedScenes = 0;
        int sceneCount = SceneManager.sceneCount;
        for (int i = 0; i < sceneCount; i++)
        {
            EScenes loadedScene = (EScenes)(1 << SceneManager.GetSceneAt(i).buildIndex);
            loadedScenes |= loadedScene;
            if (!scenesToLoad.HasFlag(loadedScene) && loadedScene != EScenes.Manager)
            {
                unusedScenes |= loadedScene;
            }
        }
#if UNITY_EDITOR
        Debug.Log($"<b>[SceneLoader]</b> Info on scenes : loaded ({loadedScenes}), unused ({unusedScenes}), needed ({scenesToLoad})");
#endif
        if (unusedScenes != EScenes.None)
        {
            await UnloadScenes(unusedScenes);
        }

        if (reloadDuplicate)
        {
            EScenes duplicates = EScenes.None;
            foreach (EScenes value in Enum.GetValues(typeof(EScenes)))
            {
                if (!scenesToLoad.HasFlag(value) || value == EScenes.None)
                {
                    continue;
                }

                if (loadedScenes.HasFlag(value) && scenesToLoad.HasFlag(value))
                {
                    duplicates |= value;
#if UNITY_EDITOR
                    Debug.Log($"<b>[SceneLoader]</b> Found duplicate ({value})");
#endif
                    continue;
                }
            }
            await UnloadScenes(duplicates);
        }

        var operationGroup = new AsyncOperationGroup();

        foreach (EScenes value in Enum.GetValues(typeof(EScenes)))
        {
            if (!scenesToLoad.HasFlag(value) || value == EScenes.None) 
            {
                continue;
            }

            if (!reloadDuplicate && loadedScenes.HasFlag(value))
            {
                scenesToLoad &= ~value;
#if UNITY_EDITOR
                Debug.Log($"<b>[SceneLoader]</b> Skipping scene duplicate reload ({value})");
#endif
                continue;
            }

            var asyncOperation = SceneManager.LoadSceneAsync(value.GetSceneIndex(), LoadSceneMode.Additive);

            operationGroup.Operations.Add(asyncOperation);

            OnSceneLoaded?.Invoke(value);
        }

        while (!operationGroup.IsDone)
        {
            m_loadingProgress.Report(operationGroup.Progress);
            await Task.Delay(100);
        }

        Scene scene = SceneManager.GetSceneByBuildIndex(activeScene.GetSceneIndex());
        if (scene.IsValid())
        { 
            SceneManager.SetActiveScene(scene);
        }

        actionAfterLoad?.Invoke();
        OnSceneGroupLoadEnd?.Invoke();
        FadeInOutScreen?.Invoke(false);
    }

    public static async Task UnloadScenes()
    {
        var scenes = new List<int>();
        var activeScene = SceneManager.GetActiveScene().buildIndex;

        int sceneCount = SceneManager.sceneCount;

        for (int i = sceneCount - 1; i >= 0; i--)
        {
            var sceneAt = SceneManager.GetSceneAt(i);
            if (!sceneAt.isLoaded) continue;

            var sceneIndex = sceneAt.buildIndex;

            if (sceneIndex == 0) continue;
            scenes.Add(sceneAt.buildIndex);
        }

        var operationGroup = new AsyncOperationGroup();
        
        foreach (var scene in scenes)
        {
            var operation = SceneManager.UnloadSceneAsync(scene);
            if (operation == null) continue;

            operationGroup.Operations.Add(operation);

            OnSceneUnload?.Invoke((EScenes)(1 << scene));
        }
        
        while (!operationGroup.IsDone)
        {
            await Task.Delay(100);
        }
    }

    public static async Task UnloadScenes(EScenes scenesToUnload)
    {
        var operationGroup = new AsyncOperationGroup();

        foreach (EScenes value in Enum.GetValues(typeof(EScenes)))
        {
            if (!scenesToUnload.HasFlag(value) || value == EScenes.None) continue;

            var scene = SceneManager.GetSceneByBuildIndex(value.GetSceneIndex());

            if (!scene.isLoaded) continue;

            var operation = SceneManager.UnloadSceneAsync(scene);
            if (operation == null) continue;

            operationGroup.Operations.Add(operation);

            OnSceneUnload?.Invoke(value);
        }

        while (!operationGroup.IsDone)
        {
            await Task.Delay(100);
        }
    }

}
