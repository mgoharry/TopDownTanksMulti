using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace Mirror_Tanks
{
    public class CustomNetworkManager : NetworkManager
    {
        public static CustomNetworkManager singleton { get; private set; }

        public bool IsServer { get; private set; }
        public bool IsClient { get; private set; }
        public bool IsHost => IsServer && IsClient;
        public string PlayerName { get; set; }

        public List<NetworkPlayer> networkPlayers;

        private void Awake()
        {
            singleton = this;
        }

        private void Start()
        {
            networkPlayers = new List<NetworkPlayer>();
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
            IsServer = true;
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            IsClient = true;
        }

        public override void OnStartHost()
        {
            base.OnStartHost();
            IsServer = IsClient = true;
        }

        public void AddPlayer(NetworkPlayer newPlayer)
        {
            if (!networkPlayers.Contains(newPlayer))
            {
                networkPlayers.Add(newPlayer);
            }
        }

        [Client]
        public NetworkPlayer GetLocalPlayer()
        {
            return networkPlayers.Find(x => x.isLocalPlayer);
        }

        [Client]
        public NetworkPlayer GetOtherPlayer()
        {
            return networkPlayers.Find(x => !x.isLocalPlayer);
        }

        public override void OnStopClient()
        {
            base.OnStopClient();
            networkPlayers.Clear();
        }

        public override void OnStopServer()
        {
            base.OnStopServer();
            networkPlayers.Clear();
        }
    }
}