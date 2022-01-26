using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Preloader : MonoBehaviour
{
    // Start is called before the first frame update
    IEnumerator Start()
    {
        DontDestroyOnLoad(gameObject);
        yield return AssetBundleLoader.Load("scenes", bundle => AssetBundleManager.Scenes = bundle);
        yield return AssetBundleLoader.Load("lab", bundle => AssetBundleManager.Lab = bundle);

        SceneManager.LoadScene("_TS Engine B");
    }
}

// ohio
// boston
// chrisitiana
public static class AssetBundleLoader
{
    public static Dictionary<string, AssetBundle> _bundles = new Dictionary<string, AssetBundle>();

    public static IEnumerator LoadOhio(Action onComplete)
    {
        yield return Load("ohio", bundle => AssetBundleManager.Ohio = bundle);

        AssetBundleManager.UnloadTavern();
        AssetBundleManager.UnloadJail();

        onComplete();
    }

    public static IEnumerator LoadBoston(Action onComplete)
    {
        yield return Load("boston", bundle => AssetBundleManager.Boston = bundle);

        AssetBundleManager.UnloadWorkshop();
        AssetBundleManager.UnloadJail();

        onComplete();
    }

    public static IEnumerator LoadChristiana(Action onComplete)
    {
        yield return Load("christiana", bundle => AssetBundleManager.Christiana = bundle);

        AssetBundleManager.UnloadWorkshop();
        AssetBundleManager.UnloadTavern();

        onComplete();
    }

    public static IEnumerator Load(string id, Action<AssetBundle> onComplete)
    {
        var platform = "PC";
#if UNITY_WEBGL
#if !UNITY_EDITOR
        platform = "WebGL";
#endif
#elif UNITY_MAC
        platform = "Mac";
#endif
        var prefix = AudioManager.GetStreamingPath() + "/AssetBundles/" + platform + "/";

        string url = prefix + id;

        if (_bundles.ContainsKey(url))
            yield break;

        var request = UnityEngine.Networking.UnityWebRequestAssetBundle.GetAssetBundle(url, 0);
        yield return request.SendWebRequest();

        if (!string.IsNullOrEmpty(request.error))
        {
            yield break;
        }

        AssetBundle bundle = UnityEngine.Networking.DownloadHandlerAssetBundle.GetContent(request);
        _bundles.Add(url, bundle);
    }
}

public class AssetBundleManager
{
    public static AssetBundle Scenes;
    public static AssetBundle Lab;
    public static AssetBundle Ohio;
    public static AssetBundle Boston;
    public static AssetBundle Christiana;

    public static void UnloadWorkshop()
    {
        if (Ohio != null)
        {
            Ohio.Unload(true);
            Ohio = null;
        }
    }

    public static void UnloadTavern()
    {
        if (Boston != null)
        {
            Boston.Unload(true);
            Boston = null;
        }
    }

    public static void UnloadJail()
    {
        if (Christiana != null)
        {
            Christiana.Unload(true);
            Christiana = null;
        }
    }
}