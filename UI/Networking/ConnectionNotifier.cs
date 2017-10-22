using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ASL.UI.Networking
{
    public class ConnectionNotifier : MonoBehaviour
    {
        private UnityEngine.UI.Text displayText;

        public void Awake()
        {
            //displayText = GameObject.Find("NetworkManager").GetComponentInChildren<UnityEngine.UI.Text>();
            displayText = gameObject.GetComponent<UnityEngine.UI.Text>();
        }

        // Update is called once per frame
        public void Update()
        {
            ConnectionState status = PhotonNetwork.connectionState;
            switch (status)
            {
                case ConnectionState.Connected:
                    displayText.color = Color.green;
                    break;
                case ConnectionState.Connecting:
                    displayText.color = Color.yellow;
                    break;
                case ConnectionState.Disconnecting:
                case ConnectionState.Disconnected:
                    displayText.color = Color.red;
                    break;
                case ConnectionState.InitializingApplication:
                    displayText.color = Color.gray;
                    break;
            }

            displayText.text = PhotonNetwork.connectionState.ToString();
            displayText.text += "\n" + PhotonNetwork.room;
            displayText.text += "\n" + PhotonNetwork.lobby;
        }
    }
}