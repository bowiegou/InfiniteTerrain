using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine.Assertions;

public class TerrainGenerator {

    public static void GenerateTerrainMeshInBackground(int sizeX, int sizeY, float[,] noiseMap, int levelOfDetail,
        Action<MeshData> callback) {
        ThreadStart thread = delegate {
            MeshThread(sizeX, sizeY, noiseMap, levelOfDetail, callback);
        };

        new Thread(thread).Start();

    }

    static void MeshThread(int sizeX, int sizeY, float[,] noiseMap, int levelOfDetail,
        Action<MeshData> callback) {

        MeshData meshdata = GenerateTerrainMeshData(sizeX, sizeY, noiseMap, levelOfDetail);
        World.OnReceiveMeshData(new KeyValuePair<MeshData, Action<MeshData>>(meshdata,callback));

    }

    public static Mesh GenerateTerrainMesh(int sizeX, int sizeY, float[,] noiseMap, int levelOfDetail = 2) {

        return GenerateTerrainMeshData(sizeX, sizeY, noiseMap, levelOfDetail).GetMesh();
    }

    public static MeshData GenerateTerrainMeshData(int sizeX, int sizeY, float[,] noiseMap, int levelOfDetail = 2) {
        
        List<Vector3> vectices;
        int[] triangles;
        List<Vector2> uvs;

        float fixX = (sizeX - 1) / -2f;
        float fixZ = (sizeY - 1) / 2f;

        /*
        / create vectices base on noiseMap
        */
        bool valid = sizeX%levelOfDetail == 0 && sizeY%levelOfDetail == 0;
        Assert.IsTrue(valid);
        if (!valid) {
            levelOfDetail = 1;
        }

        

        int vectexPerRow = sizeX / levelOfDetail + 1;
        int vectexPerCol = sizeY / levelOfDetail + 1;
        vectices = new List<Vector3>(vectexPerRow * vectexPerCol);
        for (int y = 0; y < vectexPerCol; y++) {
            for (int x = 0; x < vectexPerRow; x++) {
            
                Vector3 vectex = new Vector3(fixX + x * levelOfDetail, noiseMap[x * levelOfDetail, y * levelOfDetail], fixZ + -y * levelOfDetail); //since the 3D Gizmos defines y to be the raised-up
                vectices.Add(vectex);
            }
        }
        /*
        / create triangles as standard
        */

        int trianglesPerRow = (vectexPerRow - 1) * 2; //each vectex repersent a square, each square represent 2 triangles

        triangles = new int[trianglesPerRow * (vectexPerCol - 1)* 3]; // each triangles composed by three vectices

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
        return new MeshData(vectices,triangles,uvs);
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

}
