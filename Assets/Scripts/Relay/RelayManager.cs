using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using System.Threading.Tasks;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using Unity.Services.Authentication;
using Unity.Services.Relay.Models;
using Unity.Services.Relay;

public class RelayManager : MonoBehaviour
{
    #region Singleton
    private static RelayManager _instance;
    public static RelayManager Instance
    {
        get
        {
            if (_instance == null)
            {
                var objs = FindObjectsOfType(typeof(RelayManager)) as RelayManager[];
                if (objs.Length > 0)
                    _instance = objs[0];
                if (objs.Length > 1)
                {
                    Debug.LogError("There is more than one " + typeof(RelayManager).Name + " in the scene.");
                }
                if (_instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = string.Format("_{0}", typeof(RelayManager).Name);
                    _instance = obj.AddComponent<RelayManager>();
                }
            }
            return _instance;
        }
    }
    #endregion 

    [SerializeField] private string environment = "production";
    [SerializeField] private int maxConnections = 2;
    [SerializeField] private TMPro.TMP_Text joinCodeText;

    public bool IsRelayEnabled => Transport != null && Transport.Protocol == UnityTransport.ProtocolType.RelayUnityTransport;
    public UnityTransport Transport => NetworkManager.Singleton.gameObject.GetComponent<UnityTransport>();

    public async Task<RelayHostData> SetupRelay()
    {
        Debug.Log("Relay Server Starting with max connections " + maxConnections);

        InitializationOptions options = new InitializationOptions().SetEnvironmentName(environment);
        await UnityServices.InitializeAsync(options);

        // sign into the services anonymously and create an allocation as the host
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        Allocation allocation = await Relay.Instance.CreateAllocationAsync(maxConnections);

        // Parse allocation info to the struct RelayHostData
        RelayHostData relayHostData = new RelayHostData
        {
            Key = allocation.Key,
            Port = (ushort)allocation.RelayServer.Port,
            AllocationID = allocation.AllocationId,
            AllocationIDBytes = allocation.AllocationIdBytes,
            IPv4Address = allocation.RelayServer.IpV4,
            ConnectionData = allocation.ConnectionData
        };

        relayHostData.JoinCode = await Relay.Instance.GetJoinCodeAsync(relayHostData.AllocationID);
        joinCodeText.text = relayHostData.JoinCode;
        Debug.Log(relayHostData.JoinCode);

        Transport.SetRelayServerData(relayHostData.IPv4Address, relayHostData.Port, relayHostData.AllocationIDBytes, 
            relayHostData.Key, relayHostData.ConnectionData);

        Debug.Log("Relay Server join code: " + relayHostData.JoinCode);
        return relayHostData;
    }

    public async Task<RelayJoinData> JoinRelay(string joinCode)
    {
        InitializationOptions options = new InitializationOptions().SetEnvironmentName(environment);
        await UnityServices.InitializeAsync(options);

        // sign into the services anonymously and join an allocation with the joinCode
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        JoinAllocation joinAllocation = await Relay.Instance.JoinAllocationAsync(joinCode);

        // Parse allocation info to the struct RelayJoinData
        RelayJoinData relayJoinData = new RelayJoinData
        {
            Key = joinAllocation.Key,
            Port = (ushort)joinAllocation.RelayServer.Port,
            AllocationID = joinAllocation.AllocationId,
            AllocationIDBytes = joinAllocation.AllocationIdBytes,
            IPv4Address = joinAllocation.RelayServer.IpV4,
            ConnectionData = joinAllocation.ConnectionData,
            HostConnectionData = joinAllocation.HostConnectionData,
            JoinCode = joinCode
        };

        Transport.SetRelayServerData(relayJoinData.IPv4Address, relayJoinData.Port, relayJoinData.AllocationIDBytes, 
            relayJoinData.Key, relayJoinData.ConnectionData, relayJoinData.HostConnectionData);

        Debug.Log("Client joined with join code: " + joinCode);
        joinCodeText.text = "You are connected";
        return relayJoinData;
    }
}
