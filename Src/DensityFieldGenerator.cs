using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DensityFieldGenerator : MonoBehaviour {

    public bool isPlanet = false;
    public float frequency;
    public float amplitude;
    public float lacurnarity;
    public float persistence;
    public float floorHeight;
    public float ceilHeight;
    public int numOctaves;

	public float density(Vector3 pos)
    {
        float density;
        if (isPlanet)
        {
            density = pos.magnitude;
            density -= Mathf.Exp(-(pos.magnitude - floorHeight));
            density += Mathf.Exp(pos.magnitude - ceilHeight);
        }
        else
        {
            density = pos.y;
            density -= Mathf.Exp(-(pos.y - floorHeight));
            density += Mathf.Exp(pos.y - ceilHeight);
        }
        
        for (int i = 0; i < numOctaves; i++)
        {
            density += perlinNoise3D(pos * frequency * Mathf.Pow(lacurnarity, i)) * amplitude * Mathf.Pow(persistence, i);
        }

        return density;
    } //density function 
    

    public float[,,] sampleField(Vector3 origin, float cellWidth, int xDim, int yDim, int zDim)
    {
        float[,,] densityField = new float[xDim, yDim, zDim];

        for(int i = 0; i < xDim; i++)
        {
            for(int j = 0; j < yDim; j++)
            {
                for(int k = 0; k < zDim; k++)
                {
                    Vector3 pos = new Vector3(cellWidth * i, cellWidth * j, cellWidth * k) + origin;
                    densityField[i, j, k] = density(pos);
                }
            }
        }
        return densityField;
    }

    private float perlinNoise3D(Vector3 pos)
    {
        pos += new Vector3(1000, 8770, 2938);
        float AB = Mathf.PerlinNoise(pos.x, pos.y);
        float BC = Mathf.PerlinNoise(pos.y, pos.z);
        float AC = Mathf.PerlinNoise(pos.x, pos.z);

        float BA = Mathf.PerlinNoise(pos.y, pos.x);
        float CB = Mathf.PerlinNoise(pos.z, pos.y);
        float CA = Mathf.PerlinNoise(pos.z, pos.x);

        float ABC = AB + BA + BC + CB + AC + CA; 
        return ((ABC / 6.0f) * 2f) - 1;
    }
}
