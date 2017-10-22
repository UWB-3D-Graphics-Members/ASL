using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace UWBNetworkingPackage
{
    /// <summary>
    /// AndroidLauncher implements launcher functionality specific to the Android platform. Currently, AndroidLauncher
    /// just extends ReceivingClientLauncher, but for further development add android specific funtionality here
    /// </summary>
    public class ReceivingClientLauncher_Android : ReceivingClientLauncher_PC
    {
#if UNITY_ANDROID
        // Insert Android specific functionality/overrides here.
#endif
    }
}
