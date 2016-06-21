using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class World {
    private Dictionary<Vector2, Chunk> _chunksDictionary;
    private Dictionary<Vector2, Chunk> _lastUpdatedChunks;
    private Dictionary<Vector2, Chunk> _thisUpdatedChunks;
    private WorldData _worldData;

    private readonly GameObject _chunkPrefab;


    public World(WorldData data) {
        _chunksDictionary = new Dictionary<Vector2, Chunk>();
        _lastUpdatedChunks = new Dictionary<Vector2, Chunk>();
        _thisUpdatedChunks = new Dictionary<Vector2, Chunk>();
        _worldData = data;
        _chunkPrefab = data.TerrainController.ChunkPrefab;
        //_terrainController = data.TerrainController;
    }

    public World(WorldData data,Dictionary<Vector2, Chunk> chunksDictionary) {
        _chunksDictionary = chunksDictionary;
        _worldData = data;
        _chunkPrefab = data.TerrainController.ChunkPrefab;
    }

    public Vector2 GetChunkIndex(Vector3 worldPosition) {
        return new Vector2(Mathf.RoundToInt(worldPosition.x/_worldData.ChunkSizeX), Mathf.RoundToInt(worldPosition.z/_worldData.ChunkSizeY));

        
    }

    /// <summary>
    /// To build a chunk at a postion
    /// </summary>
    /// <param name="position">World Position to place chunk</param>
    /// <param name="levelOfDetail">Level of the details of the chunk</param>
    public void BuildChunk(Vector2 position, int levelOfDetail) {
        Vector3 fixedPosition = new Vector3(position.x, 0, position.y);
        Vector2 chunkIndex = GetChunkIndex(fixedPosition);

        if (_thisUpdatedChunks.ContainsKey(chunkIndex)) return;

        Vector3 chunkPosition = new Vector3(( + chunkIndex.x) * _worldData.ChunkSizeX , 0 , ( chunkIndex.y) * _worldData.ChunkSizeY ); 
        //Debug.Log(chunkIndex.ToString());
        Chunk chunk;



        //Debug.Log(position.ToString());

        if (!_chunksDictionary.TryGetValue(chunkIndex, out chunk)) {

            NoiseConfig noiseData = new NoiseConfig(_worldData.NoiseScale, _worldData.NoiseSeed,
                _worldData.NoiseOctaves, _worldData.NoisePersistance, _worldData.NoiseLacunarity);
            //TODO:offset
            //Debug.Log(chunkPosition);
            float[,] noiseMap = NoiseGenerator.GenerateNoise(_worldData.ChunkSizeX, _worldData.ChunkSizeY, noiseData,
                chunkPosition.x, -chunkPosition.z );
            Mesh mesh = TerrainGenerator.GenerateTerrainMesh(_worldData.ChunkSizeX, _worldData.ChunkSizeY, noiseMap, levelOfDetail);
            GameObject o = (GameObject) GameObject.Instantiate(_chunkPrefab, chunkPosition, Quaternion.identity);
            o.SetActive(true);
            o.GetComponent<ChunkController>().SetMesh(mesh);
            o.transform.parent = _worldData.TerrainController.transform;

            chunk = new Chunk(o, new ChunkData(_worldData, position, levelOfDetail,noiseMap));

            _chunksDictionary.Add(chunkIndex, chunk);
        }
        else {
            _lastUpdatedChunks.Remove(chunkIndex);
            chunk.UpdateChunk(levelOfDetail);

        }

            _thisUpdatedChunks.Add(chunkIndex,chunk);

    }


    private void CleanUpLastChunks() {
        //Debug.Log(_lastUpdatedChunks.ToString());
        foreach (KeyValuePair<Vector2, Chunk> chunk in _lastUpdatedChunks) {
            //Debug.Log("Clean up" + chunk.Value.GetChunkData().Position.ToString());
                chunk.Value.setActive(false);
        }
        _lastUpdatedChunks.Clear();
        foreach (KeyValuePair<Vector2, Chunk> chunk in _thisUpdatedChunks) {
            _lastUpdatedChunks.Add(chunk.Key, chunk.Value);
        }
        _thisUpdatedChunks.Clear();
    }



    /// <summary>
    /// some actions needed to be done after a frame has been updated
    /// </summary>
    public void OnFinishFrame() {
        CleanUpLastChunks();
    }



}

[Serializable]
public struct WorldData {
    public int SizeX;
    public int SizeY;

    public int ChunkSizeX;
    public int ChunkSizeY;

    public int NoiseScale;
    public int NoiseSeed;
    public int NoiseOctaves;

    public float NoisePersistance;
    public float NoiseLacunarity;

    public TerrainController TerrainController;

    public List<int> FOVLevel;
}