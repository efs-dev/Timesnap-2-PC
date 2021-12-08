using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ReloadSceneButton : MonoBehaviour
{
    private void Start()
    {
        GetComponent<Button>().onClick
            .AddListener(() => { SceneManager.LoadScene(SceneManager.GetActiveScene().name); });
    }
}