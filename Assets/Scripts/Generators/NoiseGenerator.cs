using UnityEngine;
using System.Collections;
using System;
public class NoiseGenerator {

    public static float[,] GenerateNoise(int sizeX, int sizeY, NoiseConfig data, float offsetX = 0, float offsetY = 0) {
        return NoiseGenerator.GenerateNoise(sizeX, sizeY, data.Scale, data.Seed, data.Octaves, data.Persistance, data.Lacunarity,offsetX,offsetY);
    }

    public static float[,] GenerateNoise(int sizeX, int sizeY, float scale, int seed, int octaves = 5, float persistance = 0.5f, float lacunarity = 1.5f, float offsetX = 0, float offsetY = 0) {
        float[,] noiseMap = new float[sizeX + 1, sizeY + 1];


        //generat octaveOffset using random number seed
        Vector2[] octaveOffset = new Vector2[octaves];
        System.Random ran = new System.Random(seed);

        for (int i = 0; i < octaves; i++) {
            octaveOffset[i].Set(ran.Next(-10000,10000) + offsetX, ran.Next(-10000,10000) + offsetY);
        }



        for (int x = 0; x < sizeX + 1; x++) {
            for (int y = 0; y < sizeY + 1; y++) {

                float noiseValue = 0;

                float amplitude = 1;
                float frequency = 1;


                // calculate the noiseValue of each Octaves and add them together
                for (int i = 0; i < octaves; i++) {
                    float sampleX = (float)x / scale * frequency + octaveOffset[i].x;
                    float sampleY = (float)y / scale * frequency + octaveOffset[i].y;

                   // Debug.Log(sampleX + " " + sampleY);

                    noiseValue += (Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1) * amplitude; // turn the value into possibly negative

                    amplitude *= persistance;
                    frequency *= lacunarity;
                    

                }

                noiseMap[x, y] = noiseValue;


            }
        }

        return noiseMap;


    }
}

public struct NoiseConfig {
    public float Scale;
    public int Seed;
    public int Octaves;
    public float Persistance;
    public float Lacunarity;

    public NoiseConfig(float scale, int seed, int octaves, float persistance, float lacunarity) {
        this.Scale = scale;
        this.Seed = seed;
        this.Octaves = octaves;
        this.Persistance = persistance;
        this.Lacunarity = lacunarity;
    }
}
