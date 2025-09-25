using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] float validDistance = 0.25f;

    [Header("References")]
    [SerializeField] Rope tether;
    [SerializeField] Constellation constellation;
    [SerializeField] AudioPlayable backgroundMusic;
    [SerializeField] AudioPlayable backgroundAmbience;

    private void Start()
    {
        AudioManager.Instance.PlayClip2D(backgroundMusic, "music");
        AudioManager.Instance.PlayClip2D(backgroundAmbience, "ambience");
    }

    public void ValidateConstallation()
    {
        foreach (Vector2 constellationPoint in constellation.LinePoints)
        {
            bool isPointValidated = false;
            foreach (Vector2 ropePoint in tether.LinePositions)
            {
                float dist = (constellationPoint - ropePoint).sqrMagnitude;
                if (dist <  (1 + validDistance) * (1 + validDistance) - 1)
                {
                    isPointValidated = true;
                    break;
                }
            }

            if (isPointValidated == false)
            {
                // Validation Failed!
                Debug.Log("VALIDATION FAILED!!");
                return;
            }
        }

        // Constellation points are all near a tether point!
        Debug.Log("VALIDATION PASSED!!");

        //return true;
    }
}
