﻿using System;
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

    public void BuildChunk(Vector2 position) {
        NoiseConfig NoiseData = new NoiseConfig(_worldData.NoiseScale, _worldData.NoiseSeed,
                                        _worldData.NoiseOctaves, _worldData.NoisePersistance, _worldData.NoiseLacunarity);
        //TODO:offset
        float[,] NoiseMap = NoiseGenerator.GenerateNoise(_worldData.ChunkSizeX, _worldData.ChunkSizeY, NoiseData, 0, 0);
        Mesh mesh = TerrainGenerator.GenerateTerrainMesh(_worldData.ChunkSizeX, _worldData.ChunkSizeY, NoiseMap);
        GameObject o = (GameObject)GameObject.Instantiate(_chunkPrefab, position, Quaternion.identity);
        o.SetActive(true);
        o.GetComponent<ChunkController>().SetMesh(mesh);
        o.transform.parent = _worldData.TerrainController.transform;
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