using UnityEngine;

public class SpriteRotator : MonoBehaviour
{
    private bool shouldRotate = false;
    private Vector3 rotationTarget;
    private float rotationSpeed = 5f;

    // Chiamato dal trigger per impostare il target di rotazione
    public void SetRotationTarget(Vector3 target, float speed)
    {
        rotationTarget = target;
        rotationSpeed = speed;
        shouldRotate = true;
    }

    private void Update()
    {
        if (!shouldRotate) return;

        RotateTowardsTarget();
    }

    private void RotateTowardsTarget()
    {
        // Calcola la direzione verso il target
        Vector3 direction = rotationTarget - transform.position;
        
        // Ignora la componente verticale (asse Y in 3D)
        direction.y = 0f;

        // Se la direzione è praticamente zero, non ruotare
        if (direction.sqrMagnitude < 0.001f) 
        {
            shouldRotate = false;
            return;
        }

        // Calcola la rotazione target guardando verso la direzione
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        // Ruota gradualmente verso il target SOLO sull'asse Y
        Vector3 currentEuler = transform.eulerAngles;
        float targetY = targetRotation.eulerAngles.y;
        
        // Interpola solo l'asse Y
        float newY = Mathf.LerpAngle(currentEuler.y, targetY, rotationSpeed * Time.deltaTime);
        transform.eulerAngles = new Vector3(0, newY, 0);

        // Ferma la rotazione quando è abbastanza vicino
        float angleDiff = Mathf.Abs(Mathf.DeltaAngle(currentEuler.y, targetY));
        if (angleDiff < 0.5f)
        {
            transform.eulerAngles = new Vector3(0, targetY, 0);
            shouldRotate = false; // Rotazione completata
        }
    }

    // Visualizza nel Scene view la direzione verso cui guarda
    private void OnDrawGizmosSelected()
    {
        if (shouldRotate)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(transform.position, rotationTarget);
        }
    }
}