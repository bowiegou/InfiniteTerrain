using UnityEngine;
using System.Collections;
using System;
public class NoiseGenerator {



    public static float[,] GenerateNoise(int sizeX, int sizeY, NoiseConfig data, float offsetX = 0, float offsetY = 0) {
        return NoiseGenerator.GenerateNoise(sizeX, sizeY, data.Scale, data.Seed, data.Octaves, data.Persistance, data.Lacunarity, offsetX, offsetY);
    }

    public static float[,] GenerateNoise(int sizeX, int sizeY, float scale, int seed, int octaves = 5, float persistance = 0.5f, float lacunarity = 1.5f, float offsetX = 0, float offsetY = 0) {
        float[,] noiseMap = new float[sizeX + 1, sizeY + 1];


        //generat octaveOffset using random number seed
        Vector2[] octaveOffset = new Vector2[octaves];
        System.Random ran = new System.Random(seed);
        float maxPossibleHeight = 0;
        float amplitude = 1;

        for (int i = 0; i < octaves; i++) {
            octaveOffset[i].Set(ran.Next(-10000, 10000) + offsetX, ran.Next(-10000, 10000) + offsetY);
            maxPossibleHeight += amplitude;
            amplitude *= persistance;
        }


        float minNoise = float.MaxValue;
        float maxNoise = float.MinValue;


        for (int x = 0; x < sizeX + 1; x++) {
            for (int y = 0; y < sizeY + 1; y++) {

                float noiseValue = 0;

                amplitude = 1;
                float frequency = 1;


                // calculate the noiseValue of each Octaves and add them together
                for (int i = 0; i < octaves; i++) {
                    float sampleX = (x + octaveOffset[i].x) / scale * frequency;
                    float sampleY = (y + octaveOffset[i].y) / scale * frequency;

                    noiseValue += (Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1) * amplitude; // turn the value into possibly negative

                    amplitude *= persistance;
                    frequency *= lacunarity;


                }

                minNoise = minNoise > noiseValue ? noiseValue : minNoise;
                maxNoise = maxNoise < noiseValue ? noiseValue : maxNoise;
                noiseMap[x, y] = noiseValue;


            }
        }



        //this magic part credit goes to @SebLague
        for (int x = 0; x < sizeX + 1; x++) {
            for (int y = 0; y < sizeY + 1; y++) {
                float normalizedHeight = (noiseMap[x, y] + 1) / (maxPossibleHeight / 0.9f);
                noiseMap[x, y] = Mathf.Clamp(normalizedHeight, 0, int.MaxValue);
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
