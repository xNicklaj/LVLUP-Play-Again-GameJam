using UnityEngine;

public class EightShieldsModifier : ModifierBase
{
    protected override void ApplyModifier()
    {
        ModifierTarget.GetComponent<InstrumentNetworkController>().UseMultipleShieldsClientRpc(2);
    }

    protected override void DisposeModifier()
    {
        ModifierTarget.GetComponent<InstrumentNetworkController>().UseMultipleShieldsClientRpc(0);
    }
}
