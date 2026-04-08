using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    [SerializeField] GameObject targetGroup;
    [SerializeField] GameObject singlePlayer;
    private int playerCount;
    private CinemachineCamera vcam;
    // Start is called before the first frame update
    void Start()
    {
        playerCount = PlayerPrefs.GetInt("PlayerCount");
        vcam = GetComponent<CinemachineCamera>();
        StartCoroutine(setCameraType());
    }

    IEnumerator setCameraType()
    {

        yield return new WaitForSeconds(1);
        if (playerCount == 1)
        {
            Transform player = singlePlayer.transform.GetChild(0);
            vcam.LookAt = player;
            vcam.Follow = player;
            Debug.Log("target changed");
        }
        else
        {
            vcam.ResolveLookAt(targetGroup.transform);
            vcam.ResolveFollow(targetGroup.transform);
        }
    }

}
