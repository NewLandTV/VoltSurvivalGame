using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum Scenes
{
    SG_Title = 4,
    SG_Loading = 5,
    SG_Game = 6
}

public class SG_Loading : MonoBehaviour
{
    private static Scenes nextScenes;

    private void Start()
    {
        StartCoroutine(LoadSceneCoroutine());
    }

    public static void LoadScene(Scenes scene)
    {
        nextScenes = scene;

        SceneManager.LoadScene((int)Scenes.SG_Loading);
    }

    private IEnumerator LoadSceneCoroutine()
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync((int)nextScenes);

        asyncOperation.allowSceneActivation = false;

        float timer = 0f;

        while (!asyncOperation.isDone)
        {
            if (asyncOperation.progress >= 0.9f)
            {
                if (timer >= 1f)
                {
                    asyncOperation.allowSceneActivation = true;

                    yield break;
                }

                timer += Time.unscaledDeltaTime;
            }

            yield return null;
        }
    }
}
