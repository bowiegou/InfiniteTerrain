using UnityEngine;
using System.Collections;

public class TerrainController : MonoBehaviour {

    public NoiseConfig DebugNoiseData;

    public bool UpdateInRealTime;

    public WorldData WorldData;

    

    public GameObject ChunkPrefab;

    public GameObject DebugObject;
    public GameObject DebugCamera;



    public int lel;

    public float offsetX;
    public float offsetY;

    private World _world;

   // private GameObject[,] _chunks;



	// Use this for initialization
	void Start () {
	    WorldData.TerrainController = this;
        _world = new World(WorldData);
        //BuildChunk(Vector3.zero);
	}
	
	// Update is called once per frame
	void Update () {
        UpdateChunk();
	}

    void UpdateChunk() {
        Vector2 cameraPositon = new Vector2(Camera.main.transform.position.x, Camera.main.transform.position.z);
        //for (int x = -WorldData.ChunkSizeX; x <= WorldData.ChunkSizeX; x+=WorldData.ChunkSizeX) {
          //  for (int y = -WorldData.ChunkSizeY; y <= WorldData.ChunkSizeY; y += WorldData.ChunkSizeY) {
                _world.BuildChunk(cameraPositon,1);
          //  }
       // }

    }

    public void BuildChunk(Vector2 position) {
        NoiseConfig NoiseData = new NoiseConfig(WorldData.NoiseScale, WorldData.NoiseSeed,
                                        WorldData.NoiseOctaves, WorldData.NoisePersistance, WorldData.NoiseLacunarity);
        //TODO:offset
        float[,] NoiseMap = NoiseGenerator.GenerateNoise(WorldData.ChunkSizeX, WorldData.ChunkSizeY, NoiseData, 0, 0);
        Mesh mesh = TerrainGenerator.GenerateTerrainMesh(WorldData.ChunkSizeX, WorldData.ChunkSizeY, NoiseMap);
        GameObject o = (GameObject)Instantiate(ChunkPrefab, position, Quaternion.identity);
        o.SetActive(true);
        o.GetComponent<ChunkController>().SetMesh(mesh);
        o.transform.parent = this.transform;
    }


    public void DebugChunk() {
        if (DebugObject == null) return;
                DebugNoiseData = new NoiseConfig(WorldData.NoiseScale, WorldData.NoiseSeed, WorldData.NoiseOctaves, WorldData.NoisePersistance, WorldData.NoiseLacunarity);
                float[,] noiseMap = NoiseGenerator.GenerateNoise(WorldData.SizeX, WorldData.SizeY, DebugNoiseData, offsetX, offsetY);
                Mesh mesh = TerrainGenerator.GenerateTerrainMesh(WorldData.SizeX, WorldData.SizeY, noiseMap,lel);
                DebugObject.GetComponent<ChunkController>().SetMesh(mesh);
                DebugObject.SetActive(true);
//        Vector2 cameraPositon = new Vector2(DebugCamera.transform.position.x, DebugCamera.transform.position.y);
//        for (int x = -WorldData.ChunkSizeX; x <= WorldData.ChunkSizeX; x += WorldData.ChunkSizeX) {
//            for (int y = -WorldData.ChunkSizeY; y <= WorldData.ChunkSizeY; y += WorldData.ChunkSizeY) {
//                _world.BuildChunk(cameraPositon - new Vector2(x, y), lel);
//            }
//        }

    }

    void OnValidate() {

        if (WorldData.SizeY < 1) {
            WorldData.SizeY = 1;
        }
        if (WorldData.SizeX < 1) {
            WorldData.SizeX = 1;
        }

        if(WorldData.NoiseScale < 1) {
            WorldData.NoiseScale = 1;
        }

        if(WorldData.NoiseOctaves < 1) {
            WorldData.NoiseOctaves = 1;
        }

        if(WorldData.NoiseSeed < 1) {
            WorldData.NoiseSeed = 1;
        }

        if(WorldData.NoisePersistance < 0) {
            WorldData.NoisePersistance = 0.0001f;
        } else if(WorldData.NoisePersistance > 1) {
            WorldData.NoisePersistance = 0.9999f;
        }

        if(WorldData.NoiseLacunarity < 1) {
            WorldData.NoiseLacunarity = 1;
        }
        
    }
}
