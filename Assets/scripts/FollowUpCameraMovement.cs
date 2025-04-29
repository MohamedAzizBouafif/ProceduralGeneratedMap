using UnityEngine;

[RequireComponent(typeof(Camera))]
public class FollowUpCameraMovement : MonoBehaviour
{
    [System.Serializable]
    public class OrbitSettings
    {
        public Transform target;
        public Vector3 targetOffset = new Vector3(0, 1.5f, 0);
        [Min(0.1f)] public float distance = 5f;
        public float minDistance = 2f;
        public float maxDistance = 10f;
    }

    [System.Serializable]
    public class MotionSettings
    {
        [Min(0)] public float orbitSpeed = 30f;
        [Tooltip("Minimum elevation angle")] 
        [Range(15, 89)] public float minElevation = 15f;
        [Tooltip("Maximum elevation angle")]
        [Range(15, 89)] public float maxElevation = 89f;
        [Tooltip("Elevation change speed (degrees/sec)")]
        [Min(0.1f)] public float elevationChangeSpeed = 5f;
        [Min(0)] public float rotationSmoothTime = 0.3f;
        [Min(0)] public float positionSmoothTime = 0.3f;
    }

    [System.Serializable]
    public class CollisionSettings
    {
        public bool enableCollision = true;
        public LayerMask collisionMask = ~0;
        [Min(0.1f)] public float collisionRadius = 0.3f;
    }

    public OrbitSettings orbit = new OrbitSettings();
    public MotionSettings motion = new MotionSettings();
    public CollisionSettings collision = new CollisionSettings();

    private float currentOrbitAngle;
    private float currentElevation;
    private bool elevatingUp = true;
    private Vector3 rotationSmoothVelocity;
    private Vector3 positionSmoothVelocity;
    private float adjustedDistance;

    private void Start()
    {
        if (orbit.target == null)
        {
            Debug.LogError("AutoOrbitCamera: No target assigned!", this);
            enabled = false;
            return;
        }

        InitializeOrbit();
    }

    private void InitializeOrbit()
    {
        currentOrbitAngle = Random.Range(0f, 360f);
        currentElevation = motion.minElevation;
        adjustedDistance = orbit.distance;
        UpdateCameraPosition(1f);
    }

    private void LateUpdate()
    {
        if (orbit.target == null) return;

        UpdateOrbitAngle();
        UpdateElevationAngle();
        HandleCollision();
        UpdateCameraPosition(motion.positionSmoothTime);
    }

    private void UpdateOrbitAngle()
    {
        currentOrbitAngle += motion.orbitSpeed * Time.deltaTime;
        currentOrbitAngle %= 360f;
    }

    private void UpdateElevationAngle()
    {
        // Smoothly oscillate between min and max elevation
        if (elevatingUp)
        {
            currentElevation += motion.elevationChangeSpeed * Time.deltaTime;
            if (currentElevation >= motion.maxElevation)
            {
                currentElevation = motion.maxElevation;
                elevatingUp = false;
            }
        }
        else
        {
            currentElevation -= motion.elevationChangeSpeed * Time.deltaTime;
            if (currentElevation <= motion.minElevation)
            {
                currentElevation = motion.minElevation;
                elevatingUp = true;
            }
        }
    }

    private void HandleCollision()
    {
        if (!collision.enableCollision)
        {
            adjustedDistance = orbit.distance;
            return;
        }

        Vector3 targetPosition = orbit.target.position + orbit.targetOffset;
        Vector3 orbitDirection = CalculateOrbitDirection();

        if (Physics.SphereCast(
            targetPosition,
            collision.collisionRadius,
            orbitDirection,
            out RaycastHit hit,
            orbit.distance,
            collision.collisionMask))
        {
            adjustedDistance = hit.distance;
        }
        else
        {
            adjustedDistance = orbit.distance;
        }
    }

    private Vector3 CalculateOrbitDirection()
    {
        Quaternion rotation = Quaternion.Euler(
            currentElevation,
            currentOrbitAngle,
            0f);
        
        return rotation * Vector3.back;
    }

    private void UpdateCameraPosition(float smoothTime)
    {
        Vector3 targetPosition = orbit.target.position + orbit.targetOffset;
        Vector3 orbitDirection = CalculateOrbitDirection();
        
        Vector3 desiredPosition = targetPosition + orbitDirection * adjustedDistance;
        Quaternion desiredRotation = Quaternion.LookRotation(targetPosition - desiredPosition);

        transform.position = Vector3.SmoothDamp(
            transform.position,
            desiredPosition,
            ref positionSmoothVelocity,
            smoothTime);
            
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            desiredRotation,
            motion.rotationSmoothTime * Time.deltaTime * 10f);
    }

    #if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (orbit.target != null)
        {
            Gizmos.color = Color.cyan;
            Vector3 targetPos = orbit.target.position + orbit.targetOffset;
            
            UnityEditor.Handles.DrawWireDisc(
                targetPos,
                Vector3.up,
                orbit.distance);
                
            Gizmos.DrawWireSphere(transform.position, 0.2f);
            Gizmos.DrawLine(targetPos, transform.position);
            
            if (collision.enableCollision)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(transform.position, collision.collisionRadius);
            }
        }
    }
    #endif
}