using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsofieldGenerator : MonoBehaviour {

    public float size;
    public float resolution;
    public float threshold;

    public Gradient colorGrad;

    private Mesh mesh;
    private MeshFilter mf;
    private DensityFieldGenerator df;

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position + new Vector3(size / 2, size / 2, size / 2), new Vector3(size, size, size));
    }

    private void Start()
    {
        df = gameObject.GetComponent<DensityFieldGenerator>();
        generateMesh(size, size, size, resolution, threshold);
    }

    private void Update()
    {
        resolution = Mathf.Clamp(resolution, 0.5f, 200.0f);
        generateMesh(size, size, size, resolution, threshold);
    }

    // function to execute marching cubes algorithm aproximately within the space provided (x, y, z dimensions)
    // threshold denotes the value where a surface should lie (in terms of density)
    private void generateMesh(float xDim, float yDim, float zDim, float cellWidth, float threshold)
    {
        // set up mesh infrastructure
        mesh = new Mesh();
        mf = gameObject.GetComponent<MeshFilter>();
        List<Vector3> verticies = new List<Vector3>();
        List<int> tris = new List<int>();
        List<Color> colors = new List<Color>();

        Vector3 originalPos = transform.position;
        int xSize = (int)(xDim / cellWidth);
        int ySize = (int)(yDim / cellWidth);
        int zSize = (int)(zDim / cellWidth);

        //get density field
        float[,,] densityField = df.sampleField(originalPos, cellWidth, xSize, ySize, zSize);
        
        for(int i = 0; i < xSize - 1; i++)
        {
            for(int j = 0; j < ySize - 1; j++)
            {
                for(int k = 0; k < zSize - 1; k++)
                {
                    Vector3 origin = new Vector3(i * cellWidth, j * cellWidth, k * cellWidth);
                    float[] vertexInfo = sampleVertexInfo(densityField, i, j, k);
                    genCubeFacets(origin, vertexInfo, threshold, cellWidth, verticies, tris, colors);
                }
            }
        }

        mesh.vertices = verticies.ToArray();
        mesh.triangles = tris.ToArray();
        mesh.colors = colors.ToArray();
        mesh.RecalculateNormals();
        mf.mesh = mesh;
    }

    private float[] sampleVertexInfo(float[,,] densityField, int xPos, int yPos, int zPos)
    {
        float[] vertexInfo = new float[8];

        for(int i = 0; i < 8; i++)
        {
            int xOff = (int)MarchingCubesTables.vertexTable[i].x;
            int yOff = (int)MarchingCubesTables.vertexTable[i].y;
            int zOff = (int)MarchingCubesTables.vertexTable[i].z;
            vertexInfo[i] = densityField[xPos + xOff, yPos + yOff, zPos + zOff];
        }
        return vertexInfo;
    }

    private void genCubeFacets(Vector3 origin, float[] vertexInfo, float isoThreshold, float cellWidth, List<Vector3> verticies, List<int> triangles, List<Color> colors)
    {
        Vector3[] intersectList = new Vector3[12];

        //generate index for lookup tables
        int cubeIndex = 0;
        for (int i = 0; i < 8; i++)
        {
            if(vertexInfo[i] < isoThreshold)
            {
                cubeIndex |= MarchingCubesTables.pow2[i];
            }
        }

        int edgeInfo = MarchingCubesTables.edgeTable[cubeIndex];
        if (edgeInfo == 0)
        {
            return;
        }

        for (int i = 0; i < 12; i++)
        {
            if ((edgeInfo & MarchingCubesTables.pow2[i]) > 0)
            {
                intersectList[i] = VertexInterpolate(MarchingCubesTables.edgePairs[i, 0], MarchingCubesTables.edgePairs[i, 1], isoThreshold, cellWidth, origin, vertexInfo);
            }
        }

        for (int i = 0; MarchingCubesTables.triTable[cubeIndex, i] != -1; i += 3)
        {
            Vector3 vertex;

            vertex = intersectList[MarchingCubesTables.triTable[cubeIndex, i    ]];
            triangles.Add(verticies.Count);
            verticies.Add(vertex);
            colors.Add(colorGrad.Evaluate(Mathf.InverseLerp(transform.position.y, transform.position.y + size, vertex.y)));

            vertex = intersectList[MarchingCubesTables.triTable[cubeIndex, i + 1]];
            triangles.Add(verticies.Count);
            verticies.Add(vertex);
            colors.Add(colorGrad.Evaluate(Mathf.InverseLerp(transform.position.y, transform.position.y + size, vertex.y)));

            vertex = intersectList[MarchingCubesTables.triTable[cubeIndex, i + 2]];
            triangles.Add(verticies.Count);
            verticies.Add(vertex);
            colors.Add(colorGrad.Evaluate(Mathf.InverseLerp(transform.position.y, transform.position.y + size, vertex.y)));
        }
    }

    private Vector3 VertexInterpolate(int v0Index, int v1Index, float isoThreshold, float cellWidth, Vector3 origin, float[] vertexInfo)
    {
        float s0 = vertexInfo[v0Index];
        float s1 = vertexInfo[v1Index];
        Vector3 p0 = MarchingCubesTables.vertexTable[v0Index];
        Vector3 p1 = MarchingCubesTables.vertexTable[v1Index];

        
        if (p1.isLessThan(p0))
        {
            Vector3 tempV = p0;
            p0 = p1;
            p1 = tempV;
            float tempF = s0;
            s0 = s1;
            s1 = tempF;
        }

        Vector3 p;
        if (Mathf.Abs(s0 - s1) > 0.00001f)
        {
            p = p0 + (p1 - p0) / (s1 - s0) * (isoThreshold - s0);
        }
        else
        {
            p = p0;
        }
        return p * cellWidth + origin;
    }
}
