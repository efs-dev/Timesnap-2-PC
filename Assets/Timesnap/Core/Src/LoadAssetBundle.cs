using System.Collections;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class LoadAssetBundle : MonoBehaviour
{
    public TMP_Text ProgressLabel;
    private bool _loading;

    public void LoadTimesnap1()
    {
        if (_loading) return;
        _loading = true;

        StartCoroutine(_LoadTimesnap1());
    }

    private IEnumerator _LoadTimesnap1()
    {
        //debug
        Caching.ClearCache();
        var req = UnityWebRequestAssetBundle
            .GetAssetBundle("https://storystashshare.com/static/timesnap1")
            .SendWebRequest();

        while (!req.isDone)
        {
            print(req.progress);
            ProgressLabel.text = (req.progress * 100).ToString("00.0") + "%";
            yield return null;
        }

        if (req.webRequest.isNetworkError || req.webRequest.isHttpError)
        {
            var errorMsg = "error downloading AssetBundle: " + req.webRequest.error;
            Debug.LogError(errorMsg);
            ProgressLabel.text = errorMsg;
            yield break;
        }

        // AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "timesnap1"));
        var myLoadedAssetBundle = DownloadHandlerAssetBundle.GetContent(req.webRequest);

        if (myLoadedAssetBundle == null)
        {
            var errorMsg = "Failed to load AssetBundle!";
            Debug.Log(errorMsg);
            ProgressLabel.text = errorMsg;
            yield break;
        }

        var scenePath = myLoadedAssetBundle.GetAllScenePaths()[0];
        print(scenePath);
        SceneManager.LoadScene(Path.GetFileNameWithoutExtension(scenePath));
    }
}