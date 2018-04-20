using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideRope : MonoBehaviour {

    private SkinnedMeshRenderer mesh;

    void Awake()
    {
        mesh = GetComponent<SkinnedMeshRenderer>();
    }

    public void HideRopeMesh()
    {
        mesh.enabled = false;
    }

    public void RevealRopeMesh()
    {
        mesh.enabled = true;
    }

}
