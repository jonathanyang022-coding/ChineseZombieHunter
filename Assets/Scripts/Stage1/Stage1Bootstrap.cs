using UnityEngine;

[ExecuteAlways]
public class Stage1Bootstrap : MonoBehaviour
{
    [SerializeField] private Vector3 cameraPosition = new Vector3(0f, 3.4f, -8f);
    [SerializeField] private Vector3 cameraRotation = new Vector3(12f, 0f, 0f);
    [SerializeField] private float cameraFieldOfView = 34f;
    [SerializeField] private Color cameraBackgroundColor = new Color(0.36f, 0.38f, 0.42f);

    [SerializeField] private Vector3 playerStartPosition = new Vector3(0f, 0.5f, 0f);
    [SerializeField] private Color playerColor = new Color(0.2f, 0.9f, 0.2f);

    [SerializeField] private Color lightColor = Color.white;
    [SerializeField] private Vector3 lightRotation = new Vector3(50f, -30f, 0f);

    private void OnEnable()
    {
        Bootstrap();
    }

    private void Awake()
    {
        Bootstrap();
    }

    private void Bootstrap()
    {
        EnsureCamera();
        EnsureDirectionalLight();
        EnsurePlayer();
    }

    private void EnsureCamera()
    {
        Camera sceneCamera = Camera.main;
        if (sceneCamera == null)
        {
            GameObject cameraObject = new GameObject("Stage1Camera");
            cameraObject.tag = "MainCamera";
            sceneCamera = cameraObject.AddComponent<Camera>();
            cameraObject.AddComponent<AudioListener>();
        }

        sceneCamera.transform.SetPositionAndRotation(cameraPosition, Quaternion.Euler(cameraRotation));
        sceneCamera.fieldOfView = cameraFieldOfView;
        sceneCamera.clearFlags = CameraClearFlags.SolidColor;
        sceneCamera.backgroundColor = cameraBackgroundColor;
        sceneCamera.nearClipPlane = 0.1f;
        sceneCamera.farClipPlane = 100f;
    }

    private void EnsureDirectionalLight()
    {
        GameObject lightObject = GameObject.Find("Stage1DirectionalLight");
        if (lightObject == null)
        {
            lightObject = new GameObject("Stage1DirectionalLight");
        }

        Light directionalLight = lightObject.GetComponent<Light>();
        if (directionalLight == null)
        {
            directionalLight = lightObject.AddComponent<Light>();
        }

        directionalLight.type = LightType.Directional;
        directionalLight.intensity = 1.2f;
        directionalLight.color = lightColor;
        lightObject.transform.rotation = Quaternion.Euler(lightRotation);
    }

    private void EnsurePlayer()
    {
        GameObject playerObject = GameObject.Find("Stage1Player");
        if (playerObject == null)
        {
            playerObject = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            playerObject.name = "Stage1Player";
        }

        playerObject.transform.position = playerStartPosition;
        playerObject.transform.localScale = new Vector3(0.7f, 1.0f, 0.7f);
        playerObject.tag = "Player";

        Renderer renderer = playerObject.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = playerColor;
        }

        LaneRunnerController controller = playerObject.GetComponent<LaneRunnerController>();
        if (controller == null)
        {
            playerObject.AddComponent<LaneRunnerController>();
        }
    }
}
