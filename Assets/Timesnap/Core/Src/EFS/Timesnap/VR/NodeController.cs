using System.Collections.Generic;
using ModestTree;
using UnityEngine;

[ExecuteInEditMode]
public class NodeController : MonoBehaviour
{
    public List<GameObject> Nodes;

    private int _activeNodeIndex = 0;

    public void NextNode()
    {
        Nodes[_activeNodeIndex].SetActive(false);
        _activeNodeIndex = (_activeNodeIndex + 1) % Nodes.Count;
        Nodes[_activeNodeIndex].SetActive(true);
    }

    private void OnTransformChildrenChanged()
    {
        Nodes.Clear();
        foreach (var environment in transform.GetComponentsInChildren<TimesnapEnvironment>(true))
        {
            Nodes.Add(environment.transform.parent.gameObject);
        }
    }
}