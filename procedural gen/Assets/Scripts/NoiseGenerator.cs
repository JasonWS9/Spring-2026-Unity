using UnityEngine;

/// <summary>
/// Generates a 2D noise map using layered Perlin noise (fractal Brownian motion).
/// Each call with the same seed, size, and parameters produces identical results.
/// </summary>
public static class NoiseGenerator
{
    /// <summary>
    /// Generates a 2D array of noise values in the range [0, 1].
    /// </summary>
    /// <param name="width">Width of the noise map.</param>
    /// <param name="height">Height of the noise map.</param>
    /// <param name="seed">Seed for deterministic generation.</param>
    /// <param name="scale">Base zoom level — lower values zoom in (smoother), higher zoom out (noisier).</param>
    /// <param name="octaves">Number of noise layers. More octaves = more detail.</param>
    /// <param name="persistence">How much each octave's amplitude decreases (0–1). Higher = rougher terrain.</param>
    /// <param name="lacunarity">How much each octave's frequency increases (>1). Higher = finer detail per octave.</param>
    /// <param name="offset">Optional world-space offset for tiling or panning.</param>
    /// <returns>A 2D float array with values normalized between 0 and 1.</returns>
    public static float[,] GenerateNoiseMap(
        int width,
        int height,
        int seed,
        float scale,
        int octaves,
        float persistence,
        float lacunarity,
        Vector2 offset)
    {
        float[,] noiseMap = new float[width, height];

        // Use the seed to create unique offsets for each octave.
        // This is how different seeds produce different terrains.
        System.Random prng = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octaves];
        for (int i = 0; i < octaves; i++)
        {
            float offsetX = prng.Next(-100000, 100000) + offset.x;
            float offsetY = prng.Next(-100000, 100000) + offset.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);
        }

        // Prevent division by zero
        if (scale <= 0f)
            scale = 0.0001f;

        // Track min/max for normalization
        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        // Sample from the center of the map so scaling zooms from the middle
        float halfWidth = width / 2f;
        float halfHeight = height / 2f;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float amplitude = 1f;
                float frequency = 1f;
                float noiseHeight = 0f;

                // Layer octaves: each adds finer detail at lower amplitude
                for (int i = 0; i < octaves; i++)
                {
                    float sampleX = (x - halfWidth + octaveOffsets[i].x) / scale * frequency;
                    float sampleY = (y - halfHeight + octaveOffsets[i].y) / scale * frequency;

                    // Mathf.PerlinNoise returns 0–1; remap to -1 to 1 for better layering
                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2f - 1f;
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= persistence;   // Each octave contributes less
                    frequency *= lacunarity;    // Each octave is higher frequency
                }

                if (noiseHeight > maxNoiseHeight)
                    maxNoiseHeight = noiseHeight;
                if (noiseHeight < minNoiseHeight)
                    minNoiseHeight = noiseHeight;

                noiseMap[x, y] = noiseHeight;
            }
        }

        // Normalize all values to 0–1 range
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
            }
        }

        return noiseMap;
    }
}
