using System;
using UnityEngine;
using System.Collections;



public class Chunk {
    readonly GameObject _chunkGameObject;
    //private ChunkData _chunkData;

    readonly WorldData _worldData;
    readonly Vector3 _position;
    private readonly ChunkController _chunkController;
    private MeshConfig _meshConfig;


    private readonly float[,] NoiseMap;

    public Chunk(GameObject o, ChunkData data, WorldData worldData) {
        _chunkGameObject = o;
        _worldData = worldData;
        _position = data.Position;
        NoiseMap = data.NoiseMap;
        _meshConfig = data.MeshConfig;
        _chunkController = _chunkGameObject.GetComponent<ChunkController>();

    }

    public ChunkData GetChunkData() {
        return new ChunkData(_position,NoiseMap,_meshConfig);
    }

    public bool IsActive() {
        return _chunkGameObject.activeSelf;
    }

    public void SetActive(bool active) {
        _chunkGameObject.SetActive(active);
    }

    public void UpdateChunk(int levelOfDetail) {
        if (levelOfDetail != this._meshConfig.LevelOfDetail) {

            this._meshConfig.LevelOfDetail = levelOfDetail;
           // Mesh mesh = TerrainGenerator.GenerateTerrainMesh(_worldData.ChunkSizeX, _worldData.ChunkSizeY, NoiseMap,levelOfDetail);
            //_chunkController.SetMesh(mesh);
            TerrainGenerator.GenerateTerrainMeshInBackground(_meshConfig,this.onReceiveMeshData);
            //Debug.Log("Reset Mesh Done");
        }
        _chunkGameObject.SetActive(true);
    }


    public void onReceiveMeshData(MeshData meshData) {
        Mesh mesh = meshData.GetMesh();
        _chunkController.SetMesh(mesh);
        _chunkGameObject.SetActive(true);
    }
}

[Serializable]
public struct ChunkData {
    public Vector3 Position;
    public float[,] NoiseMap;
    public MeshConfig MeshConfig;

    public ChunkData( Vector3 position, float[,] noiseMap, MeshConfig meshConfig) {
        Position = position;
        NoiseMap = noiseMap;
        MeshConfig = meshConfig;
    }
}
