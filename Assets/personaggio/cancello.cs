using UnityEngine;

public class TriggerCancelli : MonoBehaviour
{
    [Header("Riferimenti Cancelli")]
    public Transform cancelloRetro;
    public Transform cancelloDavanti;

    [Header("Velocità movimento")]
    public float speedRetro = 2f;
    public float speedDavanti = 2f;

    [Header("Attesa dopo discesa cancello davanti")]
    public float delayBeforeRetro = 5f;

    private bool attivato = false;

    private float retroTargetY;
    private float davantiTargetY = -0.85f;

    private Renderer retroRenderer;
    private bool retroScomparso = false;

    // --- NUOVE VARIABILI ---
    private bool davantiArrivato = false;
    private bool retroInMovimento = false;
    private float delayTimer = 0f;

    private void Start()
    {
        if (cancelloRetro != null)
        {
            retroTargetY = cancelloRetro.position.y + 20f;
            retroRenderer = cancelloRetro.GetComponent<Renderer>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!attivato && other.CompareTag("Player"))
        {
            attivato = true;
        }
    }

    private void Update()
    {
        if (!attivato)
            return;

        // --- C A N C E L L O   D A V A N T I (SCENDE SUBITO) ---
        if (cancelloDavanti != null && !davantiArrivato)
        {
            Vector3 pos = cancelloDavanti.position;
            float newY = Mathf.MoveTowards(pos.y, davantiTargetY, speedDavanti * Time.deltaTime);
            cancelloDavanti.position = new Vector3(pos.x, newY, pos.z);

            // Arrivato a destinazione
            if (Mathf.Abs(newY - davantiTargetY) < 0.01f)
            {
                davantiArrivato = true;
                delayTimer = 0f; // reset timer
            }
        }

        // --- ATTESA DI 5 SECONDI ---
        if (davantiArrivato && !retroInMovimento)
        {
            delayTimer += Time.deltaTime;

            if (delayTimer >= delayBeforeRetro)
            {
                retroInMovimento = true;
            }
        }

        // --- C A N C E L L O   R E T R O (SALE DOPO 5s) ---
        if (retroInMovimento && cancelloRetro != null && !retroScomparso)
        {
            Vector3 pos = cancelloRetro.position;
            float newY = Mathf.MoveTowards(pos.y, retroTargetY, speedRetro * Time.deltaTime);
            cancelloRetro.position = new Vector3(pos.x, newY, pos.z);

            if (Mathf.Abs(newY - retroTargetY) < 0.01f)
            {
                if (retroRenderer != null)
                    retroRenderer.enabled = false;

                retroScomparso = true;
            }
        }
    }
}
