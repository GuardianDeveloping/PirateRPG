using UnityEngine;

public class WeaponAttachment : MonoBehaviour
{
    [SerializeField] private Transform sword;
    [SerializeField] private Transform hipSocket;
    [SerializeField] private Transform handSocket;

    public void AttachSwordToHand()
    {
        sword.SetParent(handSocket);

        sword.localPosition = Vector3.zero;
        sword.localRotation = Quaternion.identity;
    }

    public void AttachSwordToHip()
    {
        sword.SetParent(hipSocket);

        sword.localPosition = Vector3.zero;
        sword.localRotation = Quaternion.identity;
    }
}
