using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(ChanceSystem))]
[RequireComponent(typeof(ChangePointsOnHit))]
public class DropOnDeath : MonoBehaviour
{
    public ModifiersList ModifiersList;
    private ChanceSystem _chanceSystem;

    private void Awake()
    {
        _chanceSystem = GetComponent<ChanceSystem>();
        GetComponent<ChangePointsOnHit>().Death.AddListener(OnDeath);
    }

    private void OnDeath()
    {
        if (!NetworkManager.Singleton.IsServer && !NetworkManager.Singleton.IsHost) return;
        if (!_chanceSystem.Happens("Drop")) return;
        GameObject modifierInstance = ModifiersList.weightedList.Next();
        Debug.Log("Dropping " + modifierInstance.name);
        GameObject instancedModifier = Instantiate(modifierInstance, transform.position, Quaternion.identity);
        instancedModifier.GetComponent<NetworkObject>().Spawn();
    }
}
