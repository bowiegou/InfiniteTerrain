using UnityEngine;
using System.Collections;



public class Chunk {
    GameObject _chunkGameObject;
    //private ChunkData _chunkData;

    WorldData _worldData;
    Vector3 Position;
    int _levelOfDetail;
    private ChunkController _chunkController;


    private float[,] NoiseMap;

    public Chunk(GameObject o, ChunkData data) {
        _chunkGameObject = o;
        _worldData = data._worldData;
        Position = data.Position;
        _levelOfDetail = data.LevelOfDetail;
        NoiseMap = data.NoiseMap;
        _chunkController = _chunkGameObject.GetComponent<ChunkController>();
    }

    public ChunkData GetChunkData() {
        return new ChunkData(_worldData,Position,_levelOfDetail,NoiseMap);
    }

    public bool IsActive() {
        return _chunkGameObject.activeSelf;
    }

    public void setActive(bool active) {
        _chunkGameObject.SetActive(active);
    }

    public void UpdateChunk(int levelOfDetail) {
        if (levelOfDetail != this._levelOfDetail) {
            
            this._levelOfDetail = levelOfDetail;
           // Mesh mesh = TerrainGenerator.GenerateTerrainMesh(_worldData.ChunkSizeX, _worldData.ChunkSizeY, NoiseMap,levelOfDetail);
            //_chunkController.SetMesh(mesh);
            TerrainGenerator.GenerateTerrainMeshInBackground(_worldData.ChunkSizeX, _worldData.ChunkSizeY, NoiseMap, levelOfDetail,this.onReceiveMeshData);
            //Debug.Log("Reset Mesh Done");
        }
        _chunkGameObject.SetActive(true);
    }


    public void onReceiveMeshData(TerrainGenerator.MeshData meshData) {
        Mesh mesh = meshData.GetMesh();
        _chunkController.SetMesh(mesh);
        _chunkGameObject.SetActive(true);
    }
}

public struct ChunkData {
    public readonly WorldData _worldData;
    public Vector3 Position;
    public int LevelOfDetail;
    public float[,] NoiseMap;

    public ChunkData(WorldData _worldData, Vector3 position, int levelOfDetail, float[,] noiseMap) {
        this._worldData = _worldData;
        Position = position;
        LevelOfDetail = levelOfDetail;
        NoiseMap = noiseMap;
    }
}
