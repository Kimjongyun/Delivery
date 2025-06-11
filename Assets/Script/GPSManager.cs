using UnityEngine;
using System.Collections;
using TMPro;

public class GPSManager : MonoBehaviour
{
    public static GPSManager Instance;

    public float latitude;
    public float longitude;

    public TextMeshProUGUI gpsText;

    [SerializeField] float arrivalRadius = 0.00027f; // 30m 반경

    void Awake()
    {
        Instance = this;
    }

    IEnumerator Start()
    {
        Input.location.Start();

        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            --maxWait;
        }

        if (Input.location.status == LocationServiceStatus.Failed)
        {
            gpsText.text = "위치 정보를 가져올 수 없음";
            yield break;
        }

        latitude = Input.location.lastData.latitude;
        longitude = Input.location.lastData.longitude;

        gpsText.text = $"현재 위치\n위도: {latitude}\n경도: {longitude}";
    }

    void Update()
    {
        if (Input.location.status == LocationServiceStatus.Running)
        {
            latitude = Input.location.lastData.latitude;
            longitude = Input.location.lastData.longitude;

            gpsText.text = $"현재 위치\n위도: {latitude}\n경도: {longitude}";
        }
    }
}
