﻿using UnityEngine;
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

    void getMeshComponents() {
        _meshFilter = GetComponent<MeshFilter>();
        _meshCollider = GetComponent<MeshCollider>();
        _meshRenderer = GetComponent<MeshRenderer>();
    }

    public void setMesh(Mesh mesh) {
        if(_meshFilter == null || _meshCollider == null)
            getMeshComponents();
        _meshFilter.mesh = mesh;
        _meshCollider.sharedMesh = mesh;
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
