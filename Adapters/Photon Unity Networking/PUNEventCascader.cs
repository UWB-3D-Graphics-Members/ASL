using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ASL.Adapters.PUN
{
    public static class PUNEventCascader
    {
        public static void Join()
        {
            RaiseEventOptions options = new RaiseEventOptions();
            options.Receivers = ReceiverGroup.All;
            PhotonNetwork.RaiseEvent(UWBNetworkingPackage.ASLEventCode.EV_JOIN, null, true, options);
        }
    }
}