using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Singleton

    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    #endregion

    [Header("Settings")]
    [SerializeField] float validDistance = 0.25f;

    [Header("References")]
    [SerializeField] AudioPlayable backgroundMusic;
    [SerializeField] AudioPlayable backgroundAmbience;
    [SerializeField] AudioPlayable verifyFailAudio;
    [SerializeField] AudioPlayable verifySuccessAudio;
    [SerializeField] AudioPlayable rocketLeavingAudio;

    private void Start()
    {
        AudioManager.Instance.PlayClip2D(backgroundMusic, "music");
        AudioManager.Instance.PlayClip2D(backgroundAmbience, "ambience");
    }

    public void ValidateConstallation()
    {
        Rope tether = GameObject.Find("Tether").GetComponent<Rope>();
        Constellation constellation = GameObject.FindFirstObjectByType<Constellation>();


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
                AudioManager.Instance.PlayClip2D(verifyFailAudio, "verifyFail");
                // Validation Failed!
                //Debug.Log("VALIDATION FAILED!!");
                return;
            }
        }

        // Constellation points are all near a tether point!
        AudioManager.Instance.PlayClip2D(verifySuccessAudio, "verifySuccess");
        //Debug.Log("VALIDATION PASSED!!");
        StartCoroutine(nameof(WinLevel));
        //return true;
    }

    IEnumerator WinLevel()
    {
        GameObject player = GameObject.Find("Player");
        player.SetActive(false);

        AudioManager.Instance.PlayClip2D(rocketLeavingAudio, "rocketLeaving");
        //Play some sort of animation.

        yield return new WaitForSeconds(5.8f);

        SceneManager.Instance.LoadScene(0, null);
    }
}
