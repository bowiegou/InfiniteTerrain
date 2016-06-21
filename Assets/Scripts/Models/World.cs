using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class World {
    Dictionary<Vector2, Chunk> _chunksDictionary;
    private WorldData _worldData;

    private GameObject _chunkPrefab;


    public World(WorldData data) {
        _chunksDictionary = new Dictionary<Vector2, Chunk>();
        _worldData = data;
        _chunkPrefab = data.TerrainController.ChunkPrefab;
        //_terrainController = data.TerrainController;
    }

    public World(WorldData data,Dictionary<Vector2, Chunk> chunksDictionary) {
        _chunksDictionary = chunksDictionary;
        _worldData = data;
        _chunkPrefab = data.TerrainController.ChunkPrefab;
    }

    public Vector2 getChunkIndex(Vector3 worldPosition) {
        return new Vector2(Mathf.RoundToInt(worldPosition.x/_worldData.ChunkSizeX), Mathf.RoundToInt(worldPosition.z/_worldData.ChunkSizeY));

        
    }


    public void BuildChunk(Vector2 position, int levelOfDetail) {
        Vector3 fixedPosition = new Vector3(position.x, 0, position.y);
        Vector2 chunkIndex = getChunkIndex(fixedPosition);
        Vector3 chunkPosition = new Vector3(( + chunkIndex.x) * _worldData.ChunkSizeX , 0 , ( chunkIndex.y) * _worldData.ChunkSizeY ); 
        //Debug.Log(chunkIndex.ToString());
        Chunk chunk;

        //Debug.Log(position.ToString());

        if (!_chunksDictionary.TryGetValue(chunkIndex, out chunk)) {

            NoiseConfig NoiseData = new NoiseConfig(_worldData.NoiseScale, _worldData.NoiseSeed,
                _worldData.NoiseOctaves, _worldData.NoisePersistance, _worldData.NoiseLacunarity);
            //TODO:offset
            Debug.Log(chunkPosition);
            float[,] NoiseMap = NoiseGenerator.GenerateNoise(_worldData.ChunkSizeX, _worldData.ChunkSizeY, NoiseData,
                chunkPosition.x, -chunkPosition.z );
            Mesh mesh = TerrainGenerator.GenerateTerrainMesh(_worldData.ChunkSizeX, _worldData.ChunkSizeY, NoiseMap, 2);
            GameObject o = (GameObject) GameObject.Instantiate(_chunkPrefab, chunkPosition, Quaternion.identity);
            o.SetActive(true);
            o.GetComponent<ChunkController>().SetMesh(mesh);
            o.transform.parent = _worldData.TerrainController.transform;

            chunk = new Chunk(o, new ChunkData(_worldData, position, levelOfDetail));

            _chunksDictionary.Add(chunkIndex, chunk);
        }
        else {

            chunk.UpdateChunk(levelOfDetail);
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

    public TerrainController TerrainController;
}
