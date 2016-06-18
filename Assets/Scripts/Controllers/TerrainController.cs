using UnityEngine;
using System.Collections;

public class TerrainController : MonoBehaviour {

    public NoiseConfig NoiseData;

    public bool UpdateInRealTime;

    public int SizeX;
    public int SizeY;

    public float OffsetX;
    public float OffsetY;

    public int NoiseScale;
    public int NoiseSeed;
    public int NoiseOctaves;

    public float NoisePersistance;
    public float NoiseLacunarity;

    public GameObject ChunkPrefab;

    public GameObject DebugObject;


    private GameObject[,] _chunks;



	// Use this for initialization
	void Start () {
        BuildChunk(Vector3.zero);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void BuildChunk(Vector3 position) {
        NoiseData = new NoiseConfig(NoiseScale, NoiseSeed, NoiseOctaves, NoisePersistance, NoiseLacunarity);
        float[,] noiseMap = NoiseGenerator.GenerateNoise(SizeX, SizeY, NoiseData, OffsetX, OffsetY);
        Mesh mesh = TerrainGenerator.GenerateTerrainMesh(SizeX, SizeY, noiseMap);
        GameObject o = (GameObject)Instantiate(ChunkPrefab, position, Quaternion.identity);
        o.SetActive(true);
        o.GetComponent<ChunkController>().setMesh(mesh);
        o.transform.parent = this.transform;



        //NoiseGenerator.GenerateNoise()
    }


    public void DebugChunk() {
        if (DebugObject == null) return;
        NoiseData = new NoiseConfig(NoiseScale, NoiseSeed, NoiseOctaves, NoisePersistance, NoiseLacunarity);
        float[,] noiseMap = NoiseGenerator.GenerateNoise(SizeX, SizeY, NoiseData, OffsetX, OffsetY);
        Mesh mesh = TerrainGenerator.GenerateTerrainMesh(SizeX, SizeY, noiseMap);
        DebugObject.GetComponent<ChunkController>().setMesh(mesh);
        DebugObject.SetActive(true);
    }

    void OnValidate() {
        if(NoiseScale < 1) {
            NoiseScale = 1;
        }

        if(NoiseOctaves < 1) {
            NoiseOctaves = 1;
        }

        if(NoiseSeed < 1) {
            NoiseSeed = 1;
        }

        if(NoisePersistance < 0) {
            NoisePersistance = 0.0001f;
        } else if(NoisePersistance > 1) {
            NoisePersistance = 0.9999f;
        }

        if(NoiseLacunarity < 1) {
            NoiseLacunarity = 1;
        }
        
    }
}
