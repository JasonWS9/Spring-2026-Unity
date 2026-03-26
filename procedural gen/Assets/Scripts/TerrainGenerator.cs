using UnityEngine;
using System.Linq;

/// <summary>
/// Main controller for procedural terrain generation.
/// Attach this to an empty GameObject in your scene.
/// Generates noise → builds mesh → applies material, all driven by a seed.
///
/// SETUP:
///   1. Create an empty GameObject in the scene
///   2. Attach this script
///   3. A MeshFilter and MeshRenderer will be added automatically
///   4. Assign a material in the Inspector (or one will be created at runtime)
///   5. Press Play — terrain generates from the seed
///   6. Toggle "Auto Update" and change parameters in the Inspector to see live updates
/// </summary>
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class TerrainGenerator : MonoBehaviour
{
    [Header("Map Dimensions")]
    [Tooltip("Width of the terrain grid. Higher = more detail but slower.")]
    [Range(2, 513)]
    public int mapWidth = 241;

    [Tooltip("Height (depth) of the terrain grid.")]
    [Range(2, 513)]
    public int mapHeight = 241;

    [Header("Noise Settings")]
    [Tooltip("The seed determines the terrain shape. Same seed = same terrain.")]
    public int seed = 42;

    [Tooltip("Zoom level of the noise. Lower = smoother hills, higher = more zoomed out.")]
    [Range(1f, 500f)]
    public float noiseScale = 50f;

    [Tooltip("Number of noise layers. More = finer detail on top of large shapes.")]
    [Range(1, 8)]
    public int octaves = 4;

    [Tooltip("How much each octave's amplitude shrinks. 0.5 is a good default.")]
    [Range(0f, 1f)]
    public float persistence = 0.5f;

    [Tooltip("How much each octave's frequency grows. 2.0 is a good default.")]
    [Range(1f, 4f)]
    public float lacunarity = 2f;

    [Tooltip("Offset in world space — useful for scrolling or tiling.")]
    public Vector2 offset;

    [Header("Terrain Shape")]
    [Tooltip("Multiplies the height values. Bigger = taller mountains.")]
    public float heightMultiplier = 20f;

    [Tooltip("Remaps height values. Flatten valleys, sharpen peaks, create plateaus.")]
    public AnimationCurve heightCurve = AnimationCurve.Linear(0, 0, 1, 1);

    [Header("Coloring")]
    [Tooltip("Define terrain layers (water, sand, grass, rock, snow) by height threshold.")]
    public TerrainLayer[] terrainLayers;

    [Header("Editor")]
    [Tooltip("Regenerate terrain automatically when Inspector values change.")]
    public bool autoUpdate = true;

    /// <summary>
    /// Generates the terrain. Called at Start and can be called from the custom editor.
    /// </summary>
    public void GenerateTerrain()
    {
        // Step 1: Generate the noise map
        float[,] noiseMap = NoiseGenerator.GenerateNoiseMap(
            mapWidth, mapHeight, seed, noiseScale,
            octaves, persistence, lacunarity, offset
        );

        // Step 2: Build the mesh from the noise map
        Mesh terrainMesh = MeshGenerator.GenerateTerrainMesh(
            noiseMap, heightMultiplier, heightCurve
        );

        // Step 3: Apply mesh to the MeshFilter
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        meshFilter.sharedMesh = terrainMesh;

        // Step 4: Apply height-based vertex colors
        ApplyTerrainColors(noiseMap, terrainMesh);

        // Step 5: Add a MeshCollider so you can walk on it (optional)
        MeshCollider collider = GetComponent<MeshCollider>();
        if (collider == null)
            collider = gameObject.AddComponent<MeshCollider>();
        collider.sharedMesh = terrainMesh;
    }

    /// <summary>
    /// Colors vertices based on their height and the defined terrain layers.
    /// </summary>
    private void ApplyTerrainColors(float[,] noiseMap, Mesh mesh)
    {
        if (terrainLayers == null || terrainLayers.Length == 0)
            return;

        // Sort layers by height threshold in ascending order to guarantee loop logic
        TerrainLayer[] sortedLayers = terrainLayers.OrderBy(l => l.heightThreshold).ToArray();

        int width = noiseMap.GetLength(0);
        int height = noiseMap.GetLength(1);
        Color[] colors = new Color[width * height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float currentHeight = noiseMap[x, y];

                // Loop backwards through the GUARANTEED sorted array
                for (int i = sortedLayers.Length - 1; i >= 0; i--)
                {
                    if (currentHeight >= sortedLayers[i].heightThreshold)
                    {
                        colors[y * width + x] = sortedLayers[i].color;
                        break;
                    }
                }
            }
        }

        mesh.colors = colors;
    }

    void Start()
    {
        GenerateTerrain();
    }

    /// <summary>
    /// Randomizes the seed and regenerates. Call this from a UI button.
    /// </summary>
    public void RandomizeSeed()
    {
        seed = Random.Range(0, 999999);
        GenerateTerrain();
    }
}

/// <summary>
/// Defines a terrain color band (e.g., water, sand, grass, rock, snow).
/// Heights at or above the threshold get this color.
/// </summary>
[System.Serializable]
public class TerrainLayer
{
    public string name;

    [Tooltip("Minimum height (0–1) for this terrain type.")]
    [Range(0f, 1f)]
    public float heightThreshold;

    public Color color = Color.green;
}
