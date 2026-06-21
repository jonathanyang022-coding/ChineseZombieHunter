using UnityEngine;

public class Stage1Bootstrap : MonoBehaviour
{
    [SerializeField] private Vector3 cameraPosition = new Vector3(0f, 3.8f, -8.5f);
    [SerializeField] private Vector3 cameraRotation = new Vector3(12f, 0f, 0f);
    [SerializeField] private float cameraFieldOfView = 40f;
    [SerializeField] private Color cameraBackgroundColor = new Color(0.28f, 0.3f, 0.34f);

    [SerializeField] private Vector3 playerStartPosition = new Vector3(0f, 0.5f, 0f);
    [SerializeField] private Color playerColor = new Color(0.2f, 0.9f, 0.2f);

    [SerializeField] private Color ambientColor = new Color(0.22f, 0.22f, 0.24f);
    [SerializeField] private Color lightColor = Color.white;
    [SerializeField] private Vector3 lightRotation = new Vector3(45f, -30f, 0f);
    [SerializeField] private float lightIntensity = 0.9f;

    private void Awake()
    {
        Bootstrap();
    }

    private void Bootstrap()
    {
        EnsureCamera();
        EnsureDirectionalLight();
        EnsureGameManager();
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
        sceneCamera.farClipPlane = 150f;
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
        directionalLight.intensity = lightIntensity;
        directionalLight.color = lightColor;
        lightObject.transform.rotation = Quaternion.Euler(lightRotation);

        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
        RenderSettings.ambientLight = ambientColor;
        RenderSettings.ambientIntensity = 0.35f;
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

        Rigidbody playerBody = playerObject.GetComponent<Rigidbody>();
        if (playerBody == null)
        {
            playerBody = playerObject.AddComponent<Rigidbody>();
        }

        playerBody.useGravity = false;
        playerBody.isKinematic = true;
        playerBody.constraints = RigidbodyConstraints.FreezeRotation;

        Renderer renderer = playerObject.GetComponent<Renderer>();
        if (renderer != null)
        {
            Stage1Visuals.SetColor(renderer, playerColor);
        }

        LaneRunnerController controller = playerObject.GetComponent<LaneRunnerController>();
        if (controller == null)
        {
            playerObject.AddComponent<LaneRunnerController>();
        }
    }

    private void EnsureGameManager()
    {
        GameObject managerObject = GameObject.Find("Stage1GameManager");
        if (managerObject == null)
        {
            managerObject = new GameObject("Stage1GameManager");
        }

        Stage1GameManager gameManager = managerObject.GetComponent<Stage1GameManager>();
        if (gameManager == null)
        {
            managerObject.AddComponent<Stage1GameManager>();
        }
    }
}
