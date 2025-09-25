using UnityEngine;

public class StarUtil : MonoBehaviour
{
    public static Color32[] StarColors = new Color32[5]
    {
        new Color32(255, 204, 111, 255), // Red
        new Color32(255, 210, 161, 255), // Orange
        new Color32(255, 244, 234, 255), // Yellow (Sun-like)
        //new Color(248, 247, 255), // Yellow-white
        new Color32(202, 215, 255, 255), // White
        //new Color(170, 191, 255), // Blue-white  
        new Color32(155, 176, 255, 255)  // Blue
    };

    public static float[] StarRarity = new float[5]
    {
        .70f,
        .12f,
        .08f,
        .06f,
        .04f,
    };

    public static int GetWeightedRandomStarType()
    {
        float rand = Random.value;
        for (int i = 0; i < StarRarity.Length; i++)
        {
            rand -= StarRarity[i];
            if (rand <= 0)
                return i;
        }

        return 0;
    }
}
