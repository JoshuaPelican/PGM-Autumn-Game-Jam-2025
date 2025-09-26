#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class ConstellationBuilder : MonoBehaviour
{
    [ContextMenu("Build Constellation From Children")]
    public void BuildConstellationFromChildren()
    {
        ConstellationData data = ScriptableObject.CreateInstance<ConstellationData>();
        


        foreach (Transform child in transform)
        {
            if(child.name.Contains("star"))
                data.Stars.Add(new StarData(child.transform.position, child.transform.localScale.magnitude));
        }

        AssetDatabase.CreateAsset(data, "Assets/Scriptables/Constellations/NewConstellation.asset");
        AssetDatabase.SaveAssets();
    }
}
#endif