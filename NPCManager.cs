using UnityEngine;

public class NPCManager : MonoBehaviour
{
    public static NPCManager Instance { get; private set; }
    private NPC currentInteractingNPC;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public void InteractWithNPC(NPC npc)
    {
        if (currentInteractingNPC != null && currentInteractingNPC != npc)
        {
            currentInteractingNPC.DeactivateAllPlaceholders();
            currentInteractingNPC = null;
        }

        currentInteractingNPC = npc;
        npc.DeactivateAllPlaceholders();
        npc.ActivatePlaceholderAt(0);
    }

    public void EndInteractionWithNPC(NPC npc)
    {
        if (currentInteractingNPC == npc)
        {
            currentInteractingNPC.DeactivateAllPlaceholders();
            currentInteractingNPC = null;
        }
    }
}
