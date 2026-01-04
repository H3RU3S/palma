using UnityEngine;
using UnityEngine.AI;

public class NPC_IA : MonoBehaviour {
    [Header("Parametri IA")]
    public float raggioMovimento = 15f; // Raggio entro cui cercare la meta
    private NavMeshAgent agent;
    private Animator anim;

    void Start() {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        
        // Configurazione automatica dell'agente
        agent.acceleration = 10f; 
        agent.angularSpeed = 360f; 
        
        IniziaNuovoPercorso();
    }

    void Update() {
        // Sincronizza l'animazione con la velocità reale dell'agente
        if (anim != null) {
            anim.SetFloat("Speed", agent.velocity.magnitude);
        }

        // Se l'NPC è quasi arrivato o è bloccato, cambia meta
        if (!agent.pathPending && agent.remainingDistance < 0.5f) {
            IniziaNuovoPercorso();
        }
    }

    void IniziaNuovoPercorso() {
        // Cerca un punto casuale
        Vector3 puntoCasuale = transform.position + Random.insideUnitSphere * raggioMovimento;
        NavMeshHit hit;
        
        // Verifica che il punto sia effettivamente sulla zona azzurra (NavMesh)
        // Se non trova un punto entro 5 metri dal punto casuale, riprova
        if (NavMesh.SamplePosition(puntoCasuale, out hit, 5.0f, NavMesh.AllAreas)) {
            agent.SetDestination(hit.position);
        }
    }
}