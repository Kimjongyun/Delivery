using UnityEngine;
using System.Collections;
using TMPro; // TextMeshPro를 사용하기 위해 추가

public class GPSManager : MonoBehaviour
{
    public static GPSManager Instance { get; private set; } // Singleton 패턴

    public float latitude { get; private set; } // 현재 위도
    public float longitude { get; private set; } // 현재 경도

    [SerializeField] private TextMeshProUGUI debugGpsText; // 디버그용 TextMeshProUGUI (에디터에서 연결)

    private LocationServiceStatus currentLocationStatus = LocationServiceStatus.Stopped;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환 시 파괴되지 않도록
        }
        else
        {
            Destroy(gameObject);
        }
    }

    IEnumerator Start()
    {
        // 위치 서비스 권한 확인
        if (!Input.location.isEnabledByUser)
        {
            if (debugGpsText != null) debugGpsText.text = "위치 서비스가 비활성화되어 있습니다.";
            currentLocationStatus = LocationServiceStatus.Failed;
            yield break;
        }

        // 위치 서비스 시작 (정확도 10m, 최소 이동 거리 1m)
        Input.location.Start(10f, 1f); 

        int maxWait = 20; // 최대 20초 대기
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            --maxWait;
        }

        if (Input.location.status == LocationServiceStatus.Failed)
        {
            if (debugGpsText != null) debugGpsText.text = "위치 정보 초기화 실패";
            currentLocationStatus = LocationServiceStatus.Failed;
            yield break;
        }

        currentLocationStatus = Input.location.status;
        // 초기 위치 정보 설정
        latitude = Input.location.lastData.latitude;
        longitude = Input.location.lastData.longitude;

        if (debugGpsText != null)
        {
            debugGpsText.text = $"GPS Started!\nLat: {latitude:F6}\nLon: {longitude:F6}";
        }
    }

    void Update()
    {
        if (currentLocationStatus == LocationServiceStatus.Running)
        {
            // 위치 정보가 업데이트될 때마다 위도, 경도 갱신
            latitude = Input.location.lastData.latitude;
            longitude = Input.location.lastData.longitude;

            if (debugGpsText != null)
            {
                debugGpsText.text = $"Current GPS\nLat: {latitude:F6}\nLon: {longitude:F6}\nTime: {Input.location.lastData.timestamp}";
            }
        }
    }

    void OnApplicationQuit()
    {
        // 앱 종료 시 위치 서비스 중지
        Input.location.Stop();
    }

    public LocationServiceStatus GetLocationStatus()
    {
        return currentLocationStatus;
    }
}