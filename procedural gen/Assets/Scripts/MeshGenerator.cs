using UnityEngine;

/// <summary>
/// Builds a Unity Mesh from a 2D height map.
/// Creates vertices at each grid point, assigns Y positions from the height map,
/// and connects them with triangles. Also generates UVs and recalculates normals.
/// </summary>
public static class MeshGenerator
{
    /// <summary>
    /// Creates a terrain mesh from a height map.
    /// </summary>
    /// <param name="heightMap">2D array of height values (0–1).</param>
    /// <param name="heightMultiplier">Scales height values to world units.</param>
    /// <param name="heightCurve">Optional curve to remap heights (e.g., flatten valleys, sharpen peaks).</param>
    /// <returns>A fully constructed Mesh ready to assign to a MeshFilter.</returns>
    public static Mesh GenerateTerrainMesh(float[,] heightMap, float heightMultiplier, AnimationCurve heightCurve)
    {
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);

        // Center the mesh at origin
        float topLeftX = (width - 1) / -2f;
        float topLeftZ = (height - 1) / 2f;

        // --- Vertices & UVs ---
        // One vertex per grid point: (width * height) total
        Vector3[] vertices = new Vector3[width * height];
        Vector2[] uvs = new Vector2[width * height];

        // --- Triangles ---
        // Each grid cell (between 4 adjacent vertices) has 2 triangles = 6 indices
        int[] triangles = new int[(width - 1) * (height - 1) * 6];
        int triIndex = 0;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int vertexIndex = y * width + x;

                // Evaluate the height curve if provided, otherwise use raw value
                float heightValue = heightCurve != null
                    ? heightCurve.Evaluate(heightMap[x, y])
                    : heightMap[x, y];

                vertices[vertexIndex] = new Vector3(
                    topLeftX + x,                       // X position
                    heightValue * heightMultiplier,     // Y = height
                    topLeftZ - y                        // Z position
                );

                // UVs map 0–1 across the mesh for texturing
                uvs[vertexIndex] = new Vector2(
                    x / (float)width,
                    y / (float)height
                );

                // Build two triangles for each cell (skip the last row and column)
                if (x < width - 1 && y < height - 1)
                {
                    // Triangle 1 (top-left of cell)
                    //   a --- b
                    //   |  /
                    //   c
                    int a = vertexIndex;
                    int b = vertexIndex + 1;
                    int c = vertexIndex + width;
                    int d = vertexIndex + width + 1;

                    // Triangle 1 (top-left of cell)
                    //   a --- b
                    //   |  /
                    //   c
                    triangles[triIndex]     = a;
                    triangles[triIndex + 1] = b; // Swapped b and c
                    triangles[triIndex + 2] = c;

                    // Triangle 2 (bottom-right of cell)
                    //       b
                    //     / |
                    //   c---d
                    triangles[triIndex + 3] = b;
                    triangles[triIndex + 4] = d; // Swapped d and c
                    triangles[triIndex + 5] = c;

                    triIndex += 6;
                }
            }
        }

        // --- Assemble the Mesh ---
        Mesh mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32; // Support >65k verts
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();  // Required for correct lighting
        mesh.RecalculateBounds();   // Required for correct culling

        return mesh;
    }
}
