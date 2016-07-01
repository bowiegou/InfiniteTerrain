using UnityEngine;
using System.Collections;


[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class ChunkController : MonoBehaviour {
    private MeshFilter _meshFilter;
	// Use this for initialization
	void Start () {

	}

    void GetMeshComponents() {
        _meshFilter = GetComponent<MeshFilter>();
    }

    public void SetMesh(Mesh mesh) {
        if(_meshFilter == null)
            GetMeshComponents();
        _meshFilter.mesh = mesh;
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
