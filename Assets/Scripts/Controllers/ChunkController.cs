using UnityEngine;
using System.Collections;


[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class ChunkController : MonoBehaviour {
    private MeshFilter _meshFilter;
    private MeshCollider _meshCollider;
    private MeshRenderer _meshRenderer;

	// Use this for initialization
	void Start () {

	}

    void GetMeshComponents() {
        _meshFilter = GetComponent<MeshFilter>();
        _meshCollider = GetComponent<MeshCollider>();
        _meshRenderer = GetComponent<MeshRenderer>();
    }

    public void SetMesh(Mesh mesh) {
        if(_meshFilter == null || _meshCollider == null)
            GetMeshComponents();
       // _meshRenderer.enabled = false;
        _meshFilter.mesh = mesh;
        //Debug.Log("Reset Mesh");
        //_meshCollider.sharedMesh = mesh;
       // _meshRenderer.enabled = true;
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
