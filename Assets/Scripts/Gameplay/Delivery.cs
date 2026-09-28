using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;

public class Delivery : MonoBehaviour
{
    public List<Npc> NPCs;

    public List<Transform> NPCPositions;

    [SerializeField]
    private List<Npc> npcPrefabs;

    [SerializeField]
    private Transform spawnPoint;

    [SerializeField]
    private Transform exitPoint;

    private void Start()
    {
        MoveQueueForward();
        FillQueue();
    }

    public void ShowTooltip()
    {
        HUD.Instance.ShowTooltip("Press E to deliver Burger");
    }

    public void Deliver()
    {
        if (NPCs.Count == 0)
            return;

        var front = NPCs[0];
        NPCs.RemoveAt(0);
        front.Leave(exitPoint.position);

        MoveQueueForward();
        FillQueue();
    }

    public void HideTooltip()
    {
        HUD.Instance.HideTooltip();
    }

    private void MoveQueueForward()
    {
        for (var i = 0; i < NPCs.Count && i < NPCPositions.Count; i++)
        {
            NPCs[i].MoveTo(NPCPositions[i].position);
        }
    }

    private void FillQueue()
    {
        if (npcPrefabs.Count == 0)
        {
            Debug.LogWarning("Delivery has no NPC prefabs assigned.", this);
            return;
        }

        StartCoroutine(FillQueueRoutine());
    }

    private IEnumerator FillQueueRoutine()
    {
        while (NPCs.Count < NPCPositions.Count)
        {
            var prefab = npcPrefabs[Random.Range(0, npcPrefabs.Count)];
            var npc = Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);
            NPCs.Add(npc);
            npc.MoveTo(NPCPositions[NPCs.Count - 1].position);

            yield return new WaitForSeconds(1);
        }
    }
}
