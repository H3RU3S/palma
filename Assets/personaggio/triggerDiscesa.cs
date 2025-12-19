using UnityEngine;

public class TriggerCancelliDown : MonoBehaviour
{
    [Header("Riferimenti Cancelli")]
    public Transform cancelloRetro;
    public Transform cancelloDavanti;

    [Header("Folla da attivare")]
    public CrowdPerson[] crowdPeople;

    [Header("Velocità movimento")]
    public float retroSpeed = 2f;

    [Header("Posizione target retro")]
    public float retroTargetY = -0.8f;

    [Header("Opzioni Rigidbody")]
    public bool forceMakeKinematic = true;

    private bool attivato = false;
    private bool davantiEliminato = false;

    private Rigidbody retroRb;
    private Vector3 retroTargetPos;

    private void Start()
    {
        if (cancelloRetro != null)
        {
            retroTargetPos = new Vector3(cancelloRetro.position.x, retroTargetY, cancelloRetro.position.z);
            retroRb = cancelloRetro.GetComponent<Rigidbody>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!attivato && other.CompareTag("Player"))
        {
            attivato = true;

            // --- ATTIVA LA FOLLA ---
            foreach (CrowdPerson p in crowdPeople)
                if (p != null)
                    p.Attiva();

            // Distruggi cancelloDavanti
            if (!davantiEliminato && cancelloDavanti != null)
            {
                Destroy(cancelloDavanti.gameObject);
                davantiEliminato = true;
            }

            // Preparazione Rigidbody del cancello retro
            if (retroRb != null)
            {
                retroRb.linearVelocity = Vector3.zero;
                retroRb.angularVelocity = Vector3.zero;

                if (forceMakeKinematic)
                    retroRb.isKinematic = true;
            }
        }
    }

    private void Update()
    {
        if (!attivato || cancelloRetro == null) return;

        if (retroRb == null)
        {
            Vector3 pos = cancelloRetro.position;
            float newY = Mathf.MoveTowards(pos.y, retroTargetPos.y, retroSpeed * Time.deltaTime);
            cancelloRetro.position = new Vector3(pos.x, newY, pos.z);
        }
    }

    private void FixedUpdate()
    {
        if (!attivato || cancelloRetro == null || retroRb == null) return;

        Vector3 pos = retroRb.position;
        float newY = Mathf.MoveTowards(pos.y, retroTargetPos.y, retroSpeed * Time.fixedDeltaTime);
        Vector3 next = new Vector3(pos.x, newY, pos.z);

        retroRb.MovePosition(next);
    }
}