using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class DevTools : MonoBehaviour
{
    [Header("Keys")]
    [SerializeField] private Key toggleWindowKey = Key.Backquote;
    [SerializeField] private Key respawnKey = Key.F5;
    [SerializeField] private Key reloadSceneKey = Key.F6;

    [Header("References")]
    [SerializeField] private Rigidbody playerRb;
    [Tooltip("Optional. If empty, the player respawns where they started the scene.")]
    [SerializeField] private Transform playerSpawn;

    [Header("FPS")]
    [SerializeField, Min(0.05f)] private float fpsRefreshInterval = 0.25f;

    private bool _windowVisible;
    private readonly Rect _windowRect = new Rect(20, 20, 230, 0);

    private float _fps;
    private float _fpsTimer;
    private int _fpsFrames;

    private Vector3 _startPosition;
    private Quaternion _startRotation;

    private GUIStyle _labelStyle;

    private void Start()
    {
        if (playerRb == null) return;
        _startPosition = playerRb.position;
        _startRotation = playerRb.rotation;
    }

    private void Update()
    {
        Keyboard kb = Keyboard.current;
        if (kb != null)
        {
            if (kb[toggleWindowKey].wasPressedThisFrame) _windowVisible = !_windowVisible;
            if (kb[respawnKey].wasPressedThisFrame) RespawnPlayer();
            if (kb[reloadSceneKey].wasPressedThisFrame) ReloadActiveScene();
        }

        UpdateFps();
    }

    // Averages over an interval so the number is readable instead of flickering every frame
    private void UpdateFps()
    {
        _fpsFrames++;
        _fpsTimer += Time.unscaledDeltaTime;

        if (_fpsTimer >= fpsRefreshInterval)
        {
            _fps = _fpsFrames / _fpsTimer;
            _fpsFrames = 0;
            _fpsTimer = 0;
        }
    }

    private void OnGUI()
    {
        if (!_windowVisible) return;

        _labelStyle ??= new GUIStyle(GUI.skin.label) { fontSize = 14 };
        GUILayout.Window(987654, _windowRect, DrawWindow, "Dev Tools");
    }

    private void DrawWindow(int id)
    {
        GUILayout.Label($"FPS    {_fps:0}  ({1000f / Mathf.Max(_fps, 0.001f):0.0} ms)", _labelStyle);

        if (playerRb != null)
        {
            Vector3 v = playerRb.linearVelocity;
            GUILayout.Label($"Speed  {new Vector2(v.x, v.z).magnitude:0.00} u/s", _labelStyle);
            GUILayout.Label($"Vert   {v.y:0.00} u/s", _labelStyle);
        }
        else
        {
            GUILayout.Label("Assign Player Rb for speed", _labelStyle);
        }

        GUILayout.Space(8);
        GUILayout.Label($"[{respawnKey}]  Respawn", _labelStyle);
        GUILayout.Label($"[{reloadSceneKey}]  Reload Scene", _labelStyle);
    }

    // Teleports the Rigidbody and kills all momentum so you don't carry fall speed into the spawn
    private void RespawnPlayer()
    {
        if (playerRb == null)
        {
            Debug.LogWarning("DevTools: assign Player Rb to use respawn.");
            return;
        }

        Vector3 pos = playerSpawn != null ? playerSpawn.position : _startPosition;
        Quaternion rot = playerSpawn != null ? playerSpawn.rotation : _startRotation;

        playerRb.linearVelocity = Vector3.zero;
        playerRb.angularVelocity = Vector3.zero;

        // Set both so the move is immediate this frame and the physics body agrees with it
        playerRb.position = pos;
        playerRb.rotation = rot;
        playerRb.transform.SetPositionAndRotation(pos, rot);
    }

    private void ReloadActiveScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}