using UnityEngine;

public class FourShieldsModifier : ModifierBase
{
    protected override void ApplyModifier()
    {
        ModifierTarget.GetComponent<InstrumentNetworkController>().UseMultipleShieldsClientRpc(1);
    }

    protected override void DisposeModifier()
    {
        ModifierTarget.GetComponent<InstrumentNetworkController>().UseMultipleShieldsClientRpc(0);
    }
}
