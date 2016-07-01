using UnityEngine;
using System.Collections;
using UnityEngine.Assertions.Comparers;

public class TerrainController : MonoBehaviour {

    public NoiseConfig DebugNoiseData;

    public bool UpdateInRealTime;

    public WorldData WorldData;

    public GameObject ChunkPrefab;

    public GameObject DebugObject;
    public GameObject DebugCamera;

    public float DebugOffsetX;
    public float DebugOffsetY;

    private World _world;

    private Vector2 _lastCameraPosition;


    // Use this for initialization
    void Start() {
        WorldData.TerrainController = this;
        _world = new World(WorldData);
        _lastCameraPosition = new Vector2(Camera.main.transform.position.x, Camera.main.transform.position.z);
        _lastCameraPosition = new Vector2(Mathf.Infinity, Mathf.Infinity);
    }

    // Update is called once per frame
    void Update() {
        UpdateChunk();
    }

    void UpdateChunk() {
        Vector2 cameraPositon = new Vector2(Camera.main.transform.position.x, Camera.main.transform.position.z);

        if ((cameraPositon - _lastCameraPosition).SqrMagnitude() < WorldData.ChunkSizeX / 2) {

            return;
        }
        //Debug.Log("--------" + cameraPositon.ToString());
        _lastCameraPosition = cameraPositon;

        for (int i = 1; i <= WorldData.FOVLevel.Count; i++) {
            for (int x = -WorldData.ChunkSizeX * i; x <= WorldData.ChunkSizeX * i; x += WorldData.ChunkSizeX) {
                for (int y = -WorldData.ChunkSizeY * i; y <= WorldData.ChunkSizeY * i; y += WorldData.ChunkSizeY) {
                    _world.BuildChunk(new Vector2(cameraPositon.x + x, cameraPositon.y + y), WorldData.FOVLevel[i - 1]);
                }
            }


        }

        _world.OnFinishFrame();

    }

    public void BuildChunk(Vector2 position) {
        NoiseConfig noiseData = new NoiseConfig(WorldData.NoiseScale, WorldData.NoiseSeed,
                                        WorldData.NoiseOctaves, WorldData.NoisePersistance, WorldData.NoiseLacunarity);
        float[,] noiseMap = NoiseGenerator.GenerateNoise(WorldData.ChunkSizeX, WorldData.ChunkSizeY, noiseData, 0, 0);
        MeshConfig meshConfig = new MeshConfig(WorldData.ChunkSizeX, WorldData.ChunkSizeY, noiseMap);
        Mesh mesh = TerrainGenerator.GenerateTerrainMesh(meshConfig);
        GameObject o = (GameObject)Instantiate(ChunkPrefab, position, Quaternion.identity);
        o.SetActive(true);
        o.GetComponent<ChunkController>().SetMesh(mesh);
        o.transform.parent = this.transform;
    }


    public void DebugChunk() {
        if (DebugObject == null) return;
        DebugNoiseData = new NoiseConfig(WorldData.NoiseScale, WorldData.NoiseSeed, WorldData.NoiseOctaves, WorldData.NoisePersistance, WorldData.NoiseLacunarity);
        float[,] noiseMap = NoiseGenerator.GenerateNoise(WorldData.SizeX, WorldData.SizeY, DebugNoiseData, DebugOffsetX, DebugOffsetY);
        MeshConfig meshConfig = new MeshConfig(WorldData.ChunkSizeX, WorldData.ChunkSizeY, noiseMap);
        Mesh mesh = TerrainGenerator.GenerateTerrainMesh(meshConfig);
        DebugObject.GetComponent<ChunkController>().SetMesh(mesh);
        DebugObject.SetActive(true);
    }

    void OnValidate() {

        if (WorldData.SizeY < 1) {
            WorldData.SizeY = 1;
        }
        if (WorldData.SizeX < 1) {
            WorldData.SizeX = 1;
        }

        if (WorldData.NoiseScale < 1) {
            WorldData.NoiseScale = 1;
        }

        if (WorldData.NoiseOctaves < 1) {
            WorldData.NoiseOctaves = 1;
        }

        if (WorldData.NoiseSeed < 1) {
            WorldData.NoiseSeed = 1;
        }

        if (WorldData.NoisePersistance < 0) {
            WorldData.NoisePersistance = 0.0001f;
        } else if (WorldData.NoisePersistance > 1) {
            WorldData.NoisePersistance = 0.9999f;
        }

        if (WorldData.NoiseLacunarity < 1) {
            WorldData.NoiseLacunarity = 1;
        }

    }
}
