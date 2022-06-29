using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Mirror_Tanks
{
    public class MainMenuManager : MonoBehaviour
    {
        [SerializeField] TMP_InputField if_PlayerName;
        [SerializeField] Button btn_startClient;
        [SerializeField] Button btn_startHost;

        public void ValidateName()
        {
            btn_startClient.interactable = false;
            btn_startHost.interactable = false;

            if (string.IsNullOrEmpty(if_PlayerName.text))
            {
                return;
            }

            CustomNetworkManager.singleton.PlayerName = if_PlayerName.text;
            btn_startClient.interactable = true;
            btn_startHost.interactable = true;
        }

        public void StartSever()
        {
            CustomNetworkManager.singleton.StartServer();
        }

        public void StartClient()
        {
            CustomNetworkManager.singleton.StartClient();
        }

        public void StartHost()
        {
            CustomNetworkManager.singleton.StartHost();
        }
    }
}