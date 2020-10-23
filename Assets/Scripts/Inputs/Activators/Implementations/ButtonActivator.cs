using UnityEngine;

[CreateAssetMenu(fileName = "NewButtonActivator", menuName = "Custom/Activators/Button")]
public class ButtonActivator : Activator<bool>
{
    public override string ShortName => "Bt";

    protected override void OnPerformed(bool input) => Execute(Null.Default);
}