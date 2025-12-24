using UnityEngine;
using System.Collections.Generic;

public class TriggerCancelliDown : MonoBehaviour
{
    [Header("Riferimenti Cancelli")]
    public Transform cancelloRetro;
    public Transform cancelloDavanti;

    [Header("Folla - Ricerca Automatica")]
    [Tooltip("Tag degli sprite della folla (lascia vuoto per cercare tutti)")]
    public string crowdTag = "Crowd";
    public bool findAllCrowdPeople = true;
    public bool rotateAllSprites = true;
    
    [Header("Attivazione Randomica")]
    [Range(0f, 1f)]
    [Tooltip("Percentuale di sprite da attivare (0.5 = 50%, 1.0 = 100%)")]
    public float activationPercentage = 0.7f;

    [Header("Velocità movimento")]
    public float retroSpeed = 2f;

    [Header("Posizione target retro")]
    public float retroTargetY = -0.8f;

    [Header("Rotazione folla verso origine")]
    public Vector3 targetPosition = Vector3.zero;
    public float rotationSpeed = 5f;

    [Header("Opzioni Rigidbody")]
    public bool forceMakeKinematic = true;

    private bool attivato = false;
    private bool davantiEliminato = false;
    private Rigidbody retroRb;
    private Vector3 retroTargetPos;
    private CrowdPerson[] allCrowdPeople;
    private SpriteRotator[] allSpriteRotators;

    private void Start()
    {
        if (cancelloRetro != null)
        {
            retroTargetPos = new Vector3(cancelloRetro.position.x, retroTargetY, cancelloRetro.position.z);
            retroRb = cancelloRetro.GetComponent<Rigidbody>();
        }

        if (findAllCrowdPeople)
        {
            allCrowdPeople = FindObjectsOfType<CrowdPerson>();
            Debug.Log($"[TriggerCancelliDown] Trovati {allCrowdPeople.Length} CrowdPerson nella scena");
        }

        if (rotateAllSprites)
        {
            SpriteRenderer[] allSprites = FindObjectsOfType<SpriteRenderer>();
            List<SpriteRotator> rotatorList = new List<SpriteRotator>();

            foreach (SpriteRenderer sr in allSprites)
            {
                if (sr.GetComponent<CrowdPerson>() != null) continue;
                if (!string.IsNullOrEmpty(crowdTag) && !sr.CompareTag(crowdTag)) continue;

                SpriteRotator rotator = sr.GetComponent<SpriteRotator>();
                if (rotator == null)
                {
                    rotator = sr.gameObject.AddComponent<SpriteRotator>();
                }
                rotatorList.Add(rotator);
            }

            allSpriteRotators = rotatorList.ToArray();
            Debug.Log($"[TriggerCancelliDown] Trovati {allSpriteRotators.Length} sprite senza CrowdPerson");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!attivato && other.CompareTag("Player"))
        {
            attivato = true;

            ActivateRandomCrowd();
            RotateAllSprites();

            if (!davantiEliminato && cancelloDavanti != null)
            {
                Destroy(cancelloDavanti.gameObject);
                davantiEliminato = true;
            }

            if (retroRb != null)
            {
                retroRb.linearVelocity = Vector3.zero;
                retroRb.angularVelocity = Vector3.zero;

                if (forceMakeKinematic)
                    retroRb.isKinematic = true;
            }
        }
    }

    private void ActivateRandomCrowd()
    {
        if (allCrowdPeople == null || allCrowdPeople.Length == 0)
        {
            Debug.LogWarning("[TriggerCancelliDown] Nessun CrowdPerson trovato!");
            return;
        }

        int activatedCount = 0;

        foreach (CrowdPerson person in allCrowdPeople)
        {
            if (person == null) continue;

            float randomValue = Random.Range(0f, 1f);
            
            if (randomValue <= activationPercentage)
            {
                person.Attiva();
                person.SetRotationTarget(targetPosition, rotationSpeed);
                person.SetRandomJumpOffset();
                activatedCount++;
            }
        }

        Debug.Log($"[TriggerCancelliDown] Attivati {activatedCount}/{allCrowdPeople.Length} sprite della folla ({(activationPercentage * 100f):F0}%)");
    }

    private void RotateAllSprites()
    {
        if (!rotateAllSprites || allSpriteRotators == null || allSpriteRotators.Length == 0)
            return;

        int rotatedCount = 0;

        foreach (SpriteRotator rotator in allSpriteRotators)
        {
            if (rotator == null) continue;

            float randomValue = Random.Range(0f, 1f);

            if (randomValue <= activationPercentage)
            {
                rotator.SetRotationTarget(targetPosition, rotationSpeed);
                rotatedCount++;
            }
        }

        Debug.Log($"[TriggerCancelliDown] Ruotati {rotatedCount}/{allSpriteRotators.Length} sprite statici");
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

    [ContextMenu("Attiva Tutta La Folla")]
    public void ActivateAllCrowd()
    {
        if (allCrowdPeople == null) return;

        foreach (CrowdPerson person in allCrowdPeople)
        {
            if (person != null)
            {
                person.Attiva();
                person.SetRotationTarget(targetPosition, rotationSpeed);
                person.SetRandomJumpOffset();
            }
        }
    }
}