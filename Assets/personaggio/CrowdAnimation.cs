using UnityEngine;

public class CrowdPerson : MonoBehaviour
{
    [Header("Salto")]
    public float jumpHeight = 0.2f;   // altezza del salto
    public float jumpSpeed = 4f;      // velocità del salto

    private bool attivo = false;
    private float baseY;

    private void Start()
    {
        // Salviamo la posizione Y iniziale
        baseY = transform.localPosition.y;
    }

    // Chiamato dal trigger
    public void Attiva()
    {
        attivo = true;
    }

    private void Update()
    {
        if (!attivo) return;

        // --- SALTO SU E GIÙ ---
        float newY = baseY + Mathf.Sin(Time.time * jumpSpeed) * jumpHeight;
        Vector3 pos = transform.localPosition;
        transform.localPosition = new Vector3(pos.x, newY, pos.z);
    }
}
