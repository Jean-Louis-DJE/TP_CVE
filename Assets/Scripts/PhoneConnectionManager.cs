using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

[RequireComponent(typeof(NetworkManager))]
[RequireComponent(typeof(UnityTransport))]
public class ARPhoneConnectionManager : MonoBehaviour
{
    [Header("Configuration Réseau")]
    [SerializeField] private string serverIP = "10.104.17.24";
    [SerializeField] private ushort serverPort = 7777;

    [Header("Infos Session (optionnel)")]
    [SerializeField] private string defaultUser = "ARPhone";
    [SerializeField] private string defaultSession = "s01";

    private NetworkManager netManager;
    private UnityTransport transport;

    private void Start()
    {
        netManager = NetworkManager.Singleton;

        if (netManager == null)
        {
            Debug.LogError("[ARPhone] NetworkManager.Singleton introuvable.");
            return;
        }

        transport = netManager.GetComponent<UnityTransport>();

        if (transport == null)
        {
            Debug.LogError("[ARPhone] UnityTransport introuvable sur le NetworkManager.");
            return;
        }

        netManager.OnClientConnectedCallback += OnClientConnected;
        netManager.OnClientDisconnectCallback += OnClientDisconnected;

#if UNITY_NETCODE_GAMEOBJECTS_2_0_0_OR_NEWER
        netManager.OnTransportFailure += OnTransportFailure;
#endif
    }

    public void JoinDefaultWorld()
    {
        JoinSharedWorld(defaultUser, defaultSession);
    }

    public void JoinSharedWorld(string userName, string sessionName)
    {
        if (netManager == null || transport == null)
        {
            Debug.LogError("[ARPhone] Netcode non initialisé.");
            return;
        }

        if (netManager.IsClient || netManager.IsServer)
        {
            Debug.LogWarning("[ARPhone] Déjà connecté ou en cours d'exécution réseau.");
            return;
        }

        Debug.Log($"[ARPhone] Connexion de {userName} à {sessionName} via {serverIP}:{serverPort}");

        // Configure l'IP/port du PC host
        transport.SetConnectionData(serverIP, serverPort);

        bool started = netManager.StartClient();

        if (!started)
        {
            Debug.LogError("[ARPhone] StartClient() a échoué.");
        }
        else
        {
            Debug.Log("[ARPhone] Client démarré, attente de connexion...");
        }
    }

    public void Disconnect()
    {
        if (netManager != null && (netManager.IsClient || netManager.IsServer))
        {
            Debug.Log("[ARPhone] Déconnexion...");
            netManager.Shutdown();
        }
    }

    private void OnClientConnected(ulong clientId)
    {
        if (netManager != null && clientId == netManager.LocalClientId)
        {
            Debug.Log($"[ARPhone] Connecté au PC. LocalClientId = {clientId}");
        }
    }

    private void OnClientDisconnected(ulong clientId)
    {
        if (netManager != null && clientId == netManager.LocalClientId)
        {
            Debug.LogWarning("[ARPhone] Le client local a été déconnecté.");
        }
    }

#if UNITY_NETCODE_GAMEOBJECTS_2_0_0_OR_NEWER
    private void OnTransportFailure()
    {
        Debug.LogError("[ARPhone] Échec du transport réseau.");
    }
#endif

    private void OnDestroy()
    {
        if (netManager != null)
        {
            netManager.OnClientConnectedCallback -= OnClientConnected;
            netManager.OnClientDisconnectCallback -= OnClientDisconnected;

#if UNITY_NETCODE_GAMEOBJECTS_2_0_0_OR_NEWER
            netManager.OnTransportFailure -= OnTransportFailure;
#endif
        }
    }
}