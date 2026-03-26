using UnityEngine;
using UnityEditor;

/// <summary>
/// Custom Inspector for TerrainGenerator.
/// Adds a "Generate" button and enables live preview when autoUpdate is on.
/// Place this file in an "Editor" folder inside your Scripts directory.
/// </summary>
[CustomEditor(typeof(TerrainGenerator))]
public class TerrainGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        TerrainGenerator generator = (TerrainGenerator)target;

        // Detect if any value was changed in the Inspector
        if (DrawDefaultInspector())
        {
            if (generator.autoUpdate)
            {
                generator.GenerateTerrain();
            }
        }

        // Manual generate button
        if (GUILayout.Button("Generate Terrain"))
        {
            generator.GenerateTerrain();
        }

        // Randomize seed button
        if (GUILayout.Button("Randomize Seed"))
        {
            Undo.RecordObject(generator, "Randomize Seed");
            generator.RandomizeSeed();
        }
    }
}
