using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraFollow : MonoBehaviour
{
    private Transform target;
    public Vector3 offset = new Vector3(0, 2, -10);
    public float smoothSpeed = 0.5f;
    public int activateOnSceneIndex = 3;
    private float fixedZ;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        fixedZ = transform.position.z;
    }

    void LateUpdate()
    {
        if (target != null && SceneManager.GetActiveScene().buildIndex >= activateOnSceneIndex)
        {
            Vector3 desiredPosition = new Vector3(
                target.position.x + offset.x,
                target.position.y + offset.y,
                fixedZ
            );

            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex >= activateOnSceneIndex)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                target = player.transform;
            }
        }
    }
}
