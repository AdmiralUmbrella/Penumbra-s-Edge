using UnityEngine;
using System.Collections.Generic;

public class SecretPathsManager : MonoBehaviour
{
    [System.Serializable]
    public class NPCDoorPair
    {
        public GameObject npc;
        public List<GameObject> doorsToRemove;
    }

    public List<NPCDoorPair> npcDoorPairs = new List<NPCDoorPair>();
    private List<GameObject> interactedNPCs = new List<GameObject>();

    // Método para llamar cuando un jugador interactúa con un NPC
    public void OnNPCInteraction(GameObject npc)
    {
        if (!interactedNPCs.Contains(npc))
        {
            interactedNPCs.Add(npc);
            RemoveCorrespondingDoors(npc);
        }
    }

    private void RemoveCorrespondingDoors(GameObject npc)
    {
        foreach (var pair in npcDoorPairs)
        {
            if (pair.npc == npc)
            {
                foreach (var door in pair.doorsToRemove)
                {
                    if (door != null)
                    {
                        // Aquí puedes añadir efectos visuales o sonoros antes de eliminar la puerta
                        Destroy(door);
                    }
                }
                break;
            }
        }
    }

    // Método para verificar si todos los NPCs han sido interactuados
    public bool AllNPCsInteracted()
    {
        return interactedNPCs.Count == npcDoorPairs.Count;
    }
}