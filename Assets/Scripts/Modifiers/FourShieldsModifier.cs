using UnityEngine;

public class FourShieldsModifier : ModifierBase
{
    protected override void ApplyModifier()
    {
        ModifierTarget.GetComponent<InstrumentNetworkController>().UseMultipleShieldsClientRpc(true);
    }

    protected override void DisposeModifier()
    {
        ModifierTarget.GetComponent<InstrumentNetworkController>().UseMultipleShieldsClientRpc(false);
    }
}
