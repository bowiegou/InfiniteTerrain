using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine.Assertions;

public class TerrainGenerator {

    public static void GenerateTerrainMeshInBackground(MeshConfig config,
        Action<MeshData> callback) {
        ThreadStart thread = delegate {
            MeshThread(config, callback);
        };

        new Thread(thread).Start();

    }

    static void MeshThread(MeshConfig config, Action<MeshData> callback) {

        MeshData meshdata = GenerateTerrainMeshData(config);
        World.OnReceiveMeshData(new KeyValuePair<MeshData, Action<MeshData>>(meshdata, callback));

    }

    public static Mesh GenerateTerrainMesh(MeshConfig config) {

        return GenerateTerrainMeshData(config).GetMesh();
    }

    public static MeshData GenerateTerrainMeshData(MeshConfig config) {

        List<Vector3> vectices;
        int[] triangles;
        List<Vector2> uvs;

        float fixX = (config.SizeX - 1) / -2f;
        float fixZ = (config.SizeY - 1) / 2f;

        /*
        / create vectices base on noiseMap
        */
        bool valid = config.SizeX % config.LevelOfDetail == 0 && config.SizeY % config.LevelOfDetail == 0;
        Assert.IsTrue(valid);
        if (!valid) {
            config.LevelOfDetail = 1;
        }



        int vectexPerRow = config.SizeX / config.LevelOfDetail + 1;
        int vectexPerCol = config.SizeY / config.LevelOfDetail + 1;
        vectices = new List<Vector3>(vectexPerRow * vectexPerCol);
        for (int y = 0; y < vectexPerCol; y++) {
            for (int x = 0; x < vectexPerRow; x++) {

                Vector3 vectex = new Vector3(fixX + x * config.LevelOfDetail,
                                            config.NoiseMap[x * config.LevelOfDetail, y * config.LevelOfDetail] * config.HeightMultiplier,
                                            fixZ + -y * config.LevelOfDetail); //since the 3D Gizmos defines y to be the raised-up
                vectices.Add(vectex);
            }
        }
        /*
        / create triangles as standard
        */

        int trianglesPerRow = (vectexPerRow - 1) * 2; //each vectex repersent a square, each square represent 2 triangles

        triangles = new int[trianglesPerRow * (vectexPerCol - 1) * 3]; // each triangles composed by three vectices

        int count = 0;

        // -1 becuase the last vectices will not own any triangles
        for (int y = 0; y < vectexPerCol - 1; y++) {
            for (int x = 0; x < vectexPerRow - 1; x++) {
                int index = y * vectexPerRow + x; // the index of the triangle
                triangles[count++] = index;
                triangles[count++] = index + 1 + vectexPerRow;
                triangles[count++] = index + vectexPerRow;

                triangles[count++] = index + 1 + vectexPerRow;
                triangles[count++] = index;
                triangles[count++] = index + 1;

            }
        }


        /*
        / create uvs as standard
        */

        uvs = new List<Vector2>(vectices.Count);
        for (int i = 0; i < vectices.Count; i++) {
            uvs.Add(new Vector2(vectices[i].x, vectices[i].z));
        }
        return new MeshData(vectices, triangles, uvs);
    }


}

public struct MeshConfig {
    public MeshConfig(int sizeX, int sizeY, float[,] noiseMap, int levelOfDetail = 1, int heightMultiplier = 1) {
        this.SizeX = sizeX;
        this.SizeY = sizeY;
        this.NoiseMap = noiseMap;
        this.LevelOfDetail = levelOfDetail;
        HeightMultiplier = heightMultiplier;
    }

    public int SizeX;
    public int SizeY;
    public float[,] NoiseMap;
    public int LevelOfDetail;
    public int HeightMultiplier;

}



public struct MeshData {
    public MeshData(List<Vector3> vectices, int[] triangles, List<Vector2> uvs) {
        this.vectices = vectices;
        this.triangles = triangles;
        this.uvs = uvs;
    }

    public Mesh GetMesh() {
        Mesh mesh = new Mesh();
        mesh.SetVertices(vectices);
        mesh.triangles = triangles;
        mesh.SetUVs(0, uvs);
        return mesh;
    }

    List<Vector3> vectices;
    int[] triangles;
    List<Vector2> uvs;


}
