using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

public class TimesnapEditorStuff : MonoBehaviour
{
    [MenuItem("Tools/Timesnap/ReplaceLaserVisuals")]
    static void Foo()
    {
        foreach (var it in SceneManager.GetActiveScene().GetRootGameObjects()
                                       .SelectMany(it => it.GetComponentsInChildren<GvrLaserVisual>(true)))
        {
            if (it is TimesnapLaserVisual)
            {
                continue;
            }

            var t = it.GetType();
            var props = t.GetFields();

            var ts = it.gameObject.AddComponent<TimesnapLaserVisual>();
            print("--------------- modifying " + it);

            foreach (var info in props)
            {
                if (info.IsPublic)
                {
                    info.SetValue(ts, info.GetValue(it));
                }
            }

            DestroyImmediate(it);
        }
    }
}