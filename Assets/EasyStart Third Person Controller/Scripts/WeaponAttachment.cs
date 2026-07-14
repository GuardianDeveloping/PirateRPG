using UnityEngine;
//This Saves Pos and Rotation before drawing weapon, stores it and then when sheatheing weapons returns it to the correct pos and rotation of parent
public class WeaponAttachment : MonoBehaviour
{
    [SerializeField] private Transform sword;
    [SerializeField] private Transform hipSocket;
    [SerializeField] private Transform handSocket;

    private Vector3 hipLocalPosition;
    private Quaternion hipLocalRotation;
    private Vector3 hipLocalScale;

    //Calls for the position and rotation of Sword on project start then stores those values
    private void Awake()
    {
        // Sword must start as a direct child of hipSocket.
        hipLocalPosition = sword.localPosition;
        hipLocalRotation = sword.localRotation;
        hipLocalScale = sword.localScale;

        // Debug.Log($"Saved hip position: {hipLocalPosition}");
        //Debug.Log($"Saved hip rotation: {hipLocalRotation.eulerAngles}");
    }
    //Attaches sword to hand with values set from varibles set up by Guardian
    public void AttachSwordToHand()
    {
        sword.SetParent(handSocket, false);

        sword.localPosition = Vector3.zero;
        sword.localRotation = Quaternion.identity;
        sword.localScale = hipLocalScale;
    }

    //This is when the values stored from Awake() get reused so the sword does not clip into player or sit too high on the hip
    public void AttachSwordToHip()
    {
        sword.SetParent(hipSocket, false);

        sword.localPosition = hipLocalPosition;
        sword.localRotation = hipLocalRotation;
        sword.localScale = hipLocalScale;
    }
}