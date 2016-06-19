using UnityEngine;
using System.Collections;



public class Chunk {
    GameObject _chunkGameObject;
    //private ChunkData _chunkData;

    WorldData _worldData;
    Vector3 Position;
    int LevelOfDetail;

    private float[,] NoiseMap;

    public Chunk(GameObject o, ChunkData data) {
        _chunkGameObject = o;
        _worldData = data._worldData;
        Position = data.Position;
        LevelOfDetail = data.LevelOfDetail;

    }

    public ChunkData GetChunkData() {
        return new ChunkData(_worldData,Position,LevelOfDetail);
    }

    public bool IsActive() {
        return _chunkGameObject.activeSelf;
    }

    public void UpdateChunk(int levelOfDetail) {
        if (levelOfDetail != this.LevelOfDetail) {
            this.LevelOfDetail = levelOfDetail;
            //TODO LOD
            Mesh mesh = TerrainGenerator.GenerateTerrainMesh(_worldData.ChunkSizeX, _worldData.ChunkSizeY, NoiseMap);
            _chunkGameObject.GetComponent<ChunkController>().SetMesh(mesh);
        }
        if(!IsActive())
            _chunkGameObject.SetActive(true);
    }
}

public struct ChunkData {
    public readonly WorldData _worldData;
    public Vector3 Position;
    public int LevelOfDetail;

    public ChunkData(WorldData _worldData, Vector3 position, int levelOfDetail) {
        this._worldData = _worldData;
        Position = position;
        LevelOfDetail = levelOfDetail;
    }
}
