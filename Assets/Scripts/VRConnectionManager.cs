using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.InputSystem;

public class VRConnectionManager : MonoBehaviour
{
    [Header("Configuration Réseau")]
    public string serverIP = "10.104.17.24";
    public string defaultUser = "OculusVR";
    public string defaultSession = "s01";

    private NetworkManager netManager;

    void Start()
    {
        netManager = NetworkManager.Singleton;
    }

    // Cette méthode apparaîtra dans l'UnityEvent car elle n'a pas d'arguments
    public void JoinDefaultWorld()
    {
        JoinSharedWorld(defaultUser, defaultSession);
    }

    // Votre méthode principale (appelable par code)
    public void JoinSharedWorld(string userName, string sessionName)
    {
        Debug.Log($"Connexion de {userName} à {sessionName}...");
        
        // Configuration de l'IP sur le transport
        var transport = netManager.GetComponent<UnityTransport>();
        if (transport != null)
        {
            transport.ConnectionData.Address = serverIP;
        }

        if (!netManager.IsClient && !netManager.IsServer)
        {
            netManager.StartClient();
        }
        else
        {
            Debug.Log("Déjà connecté au réseau !");
        }
    }

    void Update()
    {
        // Touche C pour reprendre le contrôle sur Desktop
        if (Keyboard.current.cKey.wasPressedThisFrame)
        {
            CatchCamera();
        }
    }

    public void CatchCamera()
    {
        Debug.Log("Tentative de reprise de la caméra locale...");
        // On cherche la caméra du XR Origin local pour la réactiver
        Camera mainCam = Camera.main;
        if (mainCam != null) 
        {
            mainCam.enabled = true;
            // On peut aussi forcer le Tag "MainCamera" si nécessaire
        }
    }
}