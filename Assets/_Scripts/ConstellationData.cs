using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Constellation", menuName = "Scriptables/Constellation")]
public class ConstellationData : ScriptableObject
{
    public Sprite Icon;
    public List<StarData> Stars = new ();
    public List<Vector2Int> Lines = new ();
}

[Serializable]
public class StarData
{
    public Vector2 Position;
    public float Magnitude; // Used for size, brightness, etc.

    public StarData(Vector2 position, float magnitude)
    {
        Position = position;
        Magnitude = magnitude;
    }
}