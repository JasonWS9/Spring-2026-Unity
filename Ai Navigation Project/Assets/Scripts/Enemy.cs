using Unity.Behavior;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("References")]
    public Transform player;

    [Header("Hearing")]
    public float hearingRadius = 8f;

    [Header("Vision")]
    public float visionDistance = 10f;
    public float capsuleRadius = 0.5f;
    public float capsuleHeight = 2f;

    [SerializeField] private BehaviorGraphAgent graph;
    
    void Update()
    {
        CheckHearing();
        CheckVision();
    }

    void CheckHearing()
    {
        graph.BlackboardReference.SetVariableValue("PlayerHeard", false);


        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= hearingRadius)
        {
            Vector3 playerVelocity = GetPlayerVelocity();

            if (playerVelocity.magnitude > 0.1f)
            {
                graph.BlackboardReference.SetVariableValue("PlayerHeard", true);
            }
        }
    }

    void CheckVision()
    {
        graph.BlackboardReference.SetVariableValue("PlayerSeen", false);

        Vector3 point1 = transform.position + Vector3.up * (capsuleHeight * 0.5f);
        Vector3 point2 = transform.position - Vector3.up * (capsuleHeight * 0.5f);

        RaycastHit hit;

        if (Physics.CapsuleCast(point1, point2, capsuleRadius, transform.forward, out hit, visionDistance))
        {
            if (hit.transform == player)
            {
                graph.BlackboardReference.SetVariableValue("PlayerSeen", true);
            }
        }
    }

    Vector3 GetPlayerVelocity()
    {
        CharacterController playerController = player.GetComponent<CharacterController>();
        if (playerController != null) return playerController.velocity;

        Rigidbody rb = player.GetComponent<Rigidbody>();
        if (rb != null) return rb.linearVelocity;

        return Vector3.zero;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, hearingRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * visionDistance);
    }
}