using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

public class World {
    private readonly Dictionary<Vector2, Chunk> _chunksDictionary;
    private readonly Dictionary<Vector2, Chunk> _lastUpdatedChunks;
    private readonly Dictionary<Vector2, Chunk> _thisUpdatedChunks;
    private WorldData _worldData;

    private readonly GameObject _chunkPrefab;

    private static Queue<KeyValuePair<MeshData, Action<MeshData>>> _meshGenerationQueue;


    public World(WorldData data) {
        _chunksDictionary = new Dictionary<Vector2, Chunk>();
        _lastUpdatedChunks = new Dictionary<Vector2, Chunk>();
        _thisUpdatedChunks = new Dictionary<Vector2, Chunk>();
        _worldData = data;
        _chunkPrefab = data.TerrainController.ChunkPrefab;

        _meshGenerationQueue = new Queue<KeyValuePair<MeshData, Action<MeshData>>>();
    }

    public World(WorldData data,Dictionary<Vector2, Chunk> chunksDictionary) {
        _chunksDictionary = chunksDictionary;
        _worldData = data;
        _chunkPrefab = data.TerrainController.ChunkPrefab;
    }

    public Vector2 GetChunkIndex(Vector3 worldPosition) {

        return new Vector2(Mathf.Floor(worldPosition.x/_worldData.ChunkSizeX), Mathf.Floor(worldPosition.z/_worldData.ChunkSizeY));

    }

    /// <summary>
    /// To build a chunk at a postion
    /// </summary>
    /// <param name="position">World Position to place chunk</param>
    /// <param name="levelOfDetail">Level of the details of the chunk</param>
    public void BuildChunk(Vector2 position, int levelOfDetail) {
        Vector3 fixedPosition = new Vector3(position.x, 0, position.y);
        Vector2 chunkIndex = GetChunkIndex(fixedPosition);
        //Debug.Log(chunkIndex);
        if (_thisUpdatedChunks.ContainsKey(chunkIndex)) return;

        Vector3 chunkPosition = new Vector3(( + chunkIndex.x) * _worldData.ChunkSizeX , 0 , ( chunkIndex.y) * _worldData.ChunkSizeY ); 
        Chunk chunk;

        if (!_chunksDictionary.TryGetValue(chunkIndex, out chunk)) {

            NoiseConfig noiseData = new NoiseConfig(_worldData.NoiseScale, _worldData.NoiseSeed,
                _worldData.NoiseOctaves, _worldData.NoisePersistance, _worldData.NoiseLacunarity);
            float[,] noiseMap = NoiseGenerator.GenerateNoise(_worldData.ChunkSizeX, _worldData.ChunkSizeY, noiseData,
                chunkPosition.x, -chunkPosition.z );
            MeshConfig meshConfig = new MeshConfig(_worldData.ChunkSizeX, _worldData.ChunkSizeY, noiseMap, levelOfDetail,_worldData.HeightMultiplier);

            GameObject o = (GameObject) GameObject.Instantiate(_chunkPrefab, chunkPosition, Quaternion.identity);
            o.transform.parent = _worldData.TerrainController.transform;

            chunk = new Chunk(o, new ChunkData( position,noiseMap, meshConfig), _worldData);



            TerrainGenerator.GenerateTerrainMeshInBackground(meshConfig,chunk.onReceiveMeshData);

            _chunksDictionary.Add(chunkIndex, chunk);
        }
        else {
            _lastUpdatedChunks.Remove(chunkIndex);
            chunk.UpdateChunk(levelOfDetail);

        }

            _thisUpdatedChunks.Add(chunkIndex,chunk);

    }


    private void CleanUpLastChunks() {
        foreach (KeyValuePair<Vector2, Chunk> chunk in _lastUpdatedChunks) {
                chunk.Value.SetActive(false);
        }
        _lastUpdatedChunks.Clear();
        foreach (KeyValuePair<Vector2, Chunk> chunk in _thisUpdatedChunks) {
            _lastUpdatedChunks.Add(chunk.Key, chunk.Value);
        }
        _thisUpdatedChunks.Clear();
    }

    private static void UpdateMesh() {
        lock (_meshGenerationQueue) {
            while (_meshGenerationQueue.Count > 0) {
            KeyValuePair<MeshData, Action<MeshData>> key =
                _meshGenerationQueue.Dequeue();
                key.Value(key.Key);
            }
    }
    }



    /// <summary>
    /// some actions needed to be done after a frame has been updated
    /// </summary>
    public void OnFinishFrame() {
        CleanUpLastChunks();
        UpdateMesh();
    }

    public static void OnReceiveMeshData(KeyValuePair<MeshData,Action<MeshData>> key) {
        lock (_meshGenerationQueue) {
            _meshGenerationQueue.Enqueue(key);
        }
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

    public int HeightMultiplier;

    public TerrainController TerrainController;

    public List<int> FOVLevel;
}