using UnityEngine;
using System.Collections.Generic;

public class AudienceGenerator : MonoBehaviour
{
    [Header("Sprite Settings")]
    [Tooltip("Trascina qui i 5 sprite: p1, p2, p3, p4, p5")]
    [SerializeField] private Sprite[] audienceSprites; // I 5 sprite della platea
    
    [Header("Area Settings - Platea a Tronco di Cono")]
    [SerializeField] private Transform plateaCenter; // Centro della platea
    [SerializeField] private float innerRadius = 3f; // Raggio interno (file più vicine al centro/palco)
    [SerializeField] private float outerRadius = 12f; // Raggio esterno (file più lontane)
    [SerializeField] private float startAngle = 30f; // Angolo di inizio (gradi) - per forma a ventaglio
    [SerializeField] private float endAngle = 150f; // Angolo di fine (gradi)
    
    [Header("Generation Settings")]
    [SerializeField] private int numberOfRows = 8; // Numero di file concentriche
    [SerializeField] private int minSpritesPerRow = 5; // Sprite nella fila interna (più stretta)
    [SerializeField] private int maxSpritesPerRow = 15; // Sprite nella fila esterna (più larga)
    [SerializeField] private Vector2 spriteSizeRange = new Vector2(0.8f, 1.2f); // Variazione dimensione
    [SerializeField] private bool randomizePositions = true; // Aggiungi casualità
    [SerializeField] private float positionRandomness = 0.2f; // Quanto spostare casualmente
    
    [Header("Sorting Layer")]
    [SerializeField] private string sortingLayerName = "Default";
    [SerializeField] private int baseSortingOrder = 0;
    
    [Header("Trigger Reference")]
    [SerializeField] private GameObject cancelloTrigger; // Il trigger che attiva la generazione
    
    private bool hasGenerated = false;
    private GameObject audienceParent;

    void Start()
    {
        audienceParent = new GameObject("Audience_Generated");
        audienceParent.transform.SetParent(transform);
        
        if (audienceSprites == null || audienceSprites.Length == 0)
        {
            Debug.LogError("ERRORE: Devi assegnare i 5 sprite (p1-p5) nell'Inspector!");
            return;
        }
        
        if (audienceSprites.Length != 5)
        {
            Debug.LogWarning("ATTENZIONE: Dovresti avere esattamente 5 sprite assegnati!");
        }
    }

    void Update()
    {
        if (!hasGenerated && cancelloTrigger != null && !cancelloTrigger.activeInHierarchy)
        {
            GenerateAudience();
            hasGenerated = true;
        }
    }

    public void GenerateAudience()
    {
        if (hasGenerated)
        {
            Debug.LogWarning("La platea è già stata generata!");
            return;
        }

        Debug.Log("Inizio generazione platea a tronco di cono...");
        
        Vector3 center = plateaCenter != null ? plateaCenter.position : transform.position;
        int totalSprites = 0;

        // Genera file concentriche dalla più interna alla più esterna
        for (int row = 0; row < numberOfRows; row++)
        {
            // Calcola il raggio per questa fila (interpolazione lineare)
            float t = (float)row / (numberOfRows - 1);
            float currentRadius = Mathf.Lerp(innerRadius, outerRadius, t);
            
            // Calcola quanti sprite in questa fila (più larga = più sprite)
            int spritesInRow = Mathf.RoundToInt(Mathf.Lerp(minSpritesPerRow, maxSpritesPerRow, t));
            
            // Calcola l'angolo tra ogni sprite
            float totalAngle = endAngle - startAngle;
            float angleStep = totalAngle / (spritesInRow - 1);
            
            // Genera gli sprite lungo l'arco
            for (int i = 0; i < spritesInRow; i++)
            {
                // Angolo per questo sprite
                float angle = startAngle + (angleStep * i);
                float angleRad = angle * Mathf.Deg2Rad;
                
                // Posizione base sull'arco
                float x = currentRadius * Mathf.Cos(angleRad);
                float y = currentRadius * Mathf.Sin(angleRad);
                
                // Aggiungi casualità
                if (randomizePositions)
                {
                    x += Random.Range(-positionRandomness, positionRandomness);
                    y += Random.Range(-positionRandomness, positionRandomness);
                }
                
                Vector3 worldPos = center + new Vector3(x, y, 0);
                
                // Crea lo sprite
                CreateAudienceSprite(worldPos, row, i, totalSprites, angle);
                totalSprites++;
            }
        }

        Debug.Log($"Platea generata! Posizionati {totalSprites} sprite in {numberOfRows} file curve");
        hasGenerated = true;
    }

    private void CreateAudienceSprite(Vector3 position, int row, int col, int index, float facingAngle)
    {
        GameObject spriteObj = new GameObject($"Person_R{row}_C{col}");
        spriteObj.transform.SetParent(audienceParent.transform);
        spriteObj.transform.position = position;

        SpriteRenderer renderer = spriteObj.AddComponent<SpriteRenderer>();
        
        // Seleziona uno sprite casuale
        Sprite randomSprite = audienceSprites[Random.Range(0, audienceSprites.Length)];
        renderer.sprite = randomSprite;
        
        // Sorting layer - le file più interne (row più basso) sono davanti
        renderer.sortingLayerName = sortingLayerName;
        renderer.sortingOrder = baseSortingOrder - row; // Negativo così le file interne sono davanti

        // Variazione dimensione
        float randomScale = Random.Range(spriteSizeRange.x, spriteSizeRange.y);
        spriteObj.transform.localScale = Vector3.one * randomScale;

        // Ruota lo sprite per guardare verso il centro (palco)
        // Gli sprite guardano verso il centro della platea
        float rotationToCenter = facingAngle + 180f; // +180 per guardare verso il centro
        spriteObj.transform.rotation = Quaternion.Euler(0, 0, rotationToCenter);

        // Flip casuale per varietà
        if (Random.value > 0.5f)
        {
            renderer.flipX = true;
        }
    }

    public void ClearAudience()
    {
        if (audienceParent != null)
        {
            DestroyImmediate(audienceParent);
            audienceParent = new GameObject("Audience_Generated");
            audienceParent.transform.SetParent(transform);
        }
        
        hasGenerated = false;
        Debug.Log("Platea cancellata!");
    }

    void OnDrawGizmosSelected()
    {
        if (plateaCenter == null) return;

        Vector3 center = plateaCenter.position;
        
        // Disegna gli archi per ogni fila
        for (int row = 0; row < numberOfRows; row++)
        {
            float t = (float)row / (numberOfRows - 1);
            float currentRadius = Mathf.Lerp(innerRadius, outerRadius, t);
            
            // Colore diverso per ogni fila
            Gizmos.color = Color.Lerp(Color.yellow, Color.red, t);
            
            DrawArcGizmo(center, currentRadius, startAngle, endAngle);
        }
        
        // Disegna le linee laterali che connettono gli archi
        Gizmos.color = Color.green;
        
        // Linea sinistra
        float startAngleRad = startAngle * Mathf.Deg2Rad;
        Vector3 innerStart = center + new Vector3(
            innerRadius * Mathf.Cos(startAngleRad),
            innerRadius * Mathf.Sin(startAngleRad),
            0
        );
        Vector3 outerStart = center + new Vector3(
            outerRadius * Mathf.Cos(startAngleRad),
            outerRadius * Mathf.Sin(startAngleRad),
            0
        );
        Gizmos.DrawLine(innerStart, outerStart);
        
        // Linea destra
        float endAngleRad = endAngle * Mathf.Deg2Rad;
        Vector3 innerEnd = center + new Vector3(
            innerRadius * Mathf.Cos(endAngleRad),
            innerRadius * Mathf.Sin(endAngleRad),
            0
        );
        Vector3 outerEnd = center + new Vector3(
            outerRadius * Mathf.Cos(endAngleRad),
            outerRadius * Mathf.Sin(endAngleRad),
            0
        );
        Gizmos.DrawLine(innerEnd, outerEnd);
        
        // Disegna il centro (palco)
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(center, 0.5f);
    }

    private void DrawArcGizmo(Vector3 center, float radius, float startAngle, float endAngle)
    {
        int segments = 30;
        float angleStep = (endAngle - startAngle) / segments;
        
        Vector3 prevPoint = center + new Vector3(
            radius * Mathf.Cos(startAngle * Mathf.Deg2Rad),
            radius * Mathf.Sin(startAngle * Mathf.Deg2Rad),
            0
        );

        for (int i = 1; i <= segments; i++)
        {
            float angle = (startAngle + angleStep * i) * Mathf.Deg2Rad;
            Vector3 newPoint = center + new Vector3(
                radius * Mathf.Cos(angle),
                radius * Mathf.Sin(angle),
                0
            );
            Gizmos.DrawLine(prevPoint, newPoint);
            prevPoint = newPoint;
        }
    }
}