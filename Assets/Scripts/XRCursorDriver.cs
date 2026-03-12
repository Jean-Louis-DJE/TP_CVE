 using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit.Interactors; 

public class XRCursorDriver : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private XRRayInteractor rightRayInteractor;
    [SerializeField] private GameObject objectToCreate;

    [Header("XR Input")]
    [SerializeField] private XRNode controllerNode = XRNode.RightHand;
    [SerializeField] private bool requireTriggerHold = true;

    [Header("Fallback")]
    [SerializeField] private float fallbackDistance = 2.0f;

    private InputDevice rightController;
    private bool active;
    private bool previousAButtonState;

    private void Start()
    {
        if (!HasAuthority || !IsSpawned)
            return;

        InitializeLocalState();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (!HasAuthority)
            return;

        InitializeLocalState();
    }

    private void InitializeLocalState()
    {
        active = false;
        TryInitializeRightController();
    }

    private void Update()
    {
        if (!HasAuthority || !IsSpawned)
            return;

        EnsureRightControllerIsValid();

        if (rightRayInteractor == null)
        {
            Debug.LogWarning("[CursorDriver] rightRayInteractor is not assigned.");
            return;
        }

        HandleActivation();
        UpdateCursorPositionFromInteractor();
        HandleSpawn();
    }

    private void TryInitializeRightController()
    {
        rightController = InputDevices.GetDeviceAtXRNode(controllerNode);

        if (rightController.isValid)
            return;

        var desiredCharacteristics =
            InputDeviceCharacteristics.HeldInHand |
            InputDeviceCharacteristics.Controller |
            InputDeviceCharacteristics.Right;

        var devices = new List<InputDevice>();
        InputDevices.GetDevicesWithCharacteristics(desiredCharacteristics, devices);

        if (devices.Count > 0)
            rightController = devices[0];
    }

    private void EnsureRightControllerIsValid()
    {
        if (!rightController.isValid)
            TryInitializeRightController();
    }

    private void HandleActivation()
    {
        if (!rightController.isValid)
        {
            active = false;
            return;
        }

        bool triggerPressed = false;
        rightController.TryGetFeatureValue(CommonUsages.triggerButton, out triggerPressed);

        active = requireTriggerHold ? triggerPressed : true;
    }

    private void UpdateCursorPositionFromInteractor()
    {
        if (!active)
            return;

        RaycastHit hit;
        bool has3DHit = rightRayInteractor.TryGetCurrent3DRaycastHit(out hit);

        if (has3DHit)
        {
            transform.position = hit.point;
            return;
        }

        if (rightRayInteractor.TryGetCurrentRaycast(out RaycastHit fallbackHit, out int _, out int _, out bool _))
        {
            transform.position = fallbackHit.point;
            return;
        }

        Transform attachTransform = rightRayInteractor.transform;
        transform.position = attachTransform.position + attachTransform.forward * fallbackDistance;
    }

    private void HandleSpawn()
    {
        if (!active || !rightController.isValid || objectToCreate == null)
            return;

        bool aButtonPressed = false;
        rightController.TryGetFeatureValue(CommonUsages.primaryButton, out aButtonPressed);

        bool aButtonDown = aButtonPressed && !previousAButtonState;
        previousAButtonState = aButtonPressed;

        if (!aButtonDown)
            return;

        Debug.Log("[CursorDriver] Spawn requested.");

        GameObject newObject = Instantiate(objectToCreate);
        NetworkObject networkObject = newObject.GetComponent<NetworkObject>();

        if (networkObject == null)
        {
            Debug.LogError("[CursorDriver] ObjectToCreate must contain a NetworkObject component.");
            Destroy(newObject);
            return;
        }

        newObject.transform.position = transform.position;
        networkObject.Spawn();
    }
}