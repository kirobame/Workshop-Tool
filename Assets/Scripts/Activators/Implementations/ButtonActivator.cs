using UnityEngine;

[CreateAssetMenu(fileName = "NewButtonActivator", menuName = "Custom/Activators/Button")]
public class ButtonActivator : Activator<bool>
{
    protected override void OnPerformed(bool input) => Execute(Null.Default);
}