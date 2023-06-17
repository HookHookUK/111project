using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(LoadSceneAsync("IngameScene"));
    }

    IEnumerator LoadSceneAsync(string name)
    {
        SceneManager.LoadScene("LoadingScene");
        Time.timeScale = 1f;
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(name);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        yield return new WaitForSeconds(3f);

    }

}
