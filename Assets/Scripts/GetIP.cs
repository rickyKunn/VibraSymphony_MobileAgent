using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using TMPro;
using UnityEngine;

public class GetIP : MonoBehaviour
{
    public string IPAddress;
    [HideInInspector]
    public string IPLastString;
    [SerializeField]
    private TextMeshProUGUI SSIDText;
    public bool isNetworked;
    void Start()
    {
        IPAddress = IP();
        if(IPAddress == "127.0.0.1")
        {
            isNetworked = false;
            return;
        }
        else
        {
            isNetworked = true;
        }
        string[] splitIP = IPAddress.Split('.');
        string subnetmask = SubnetMask();
        var subnetList = subnetmask.Split(".");
        for(int i = 0;i < subnetList.Length; i++)
        {
            if (subnetList[i] != "255")
            {
                if(i != 3)
                {
                    IPLastString += splitIP[i] + "B";
                }
                else
                {
                    IPLastString += splitIP[i];
                }
            }
        }
    }

    public string IP()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                print(ip.ToString());
                return ip.ToString();
            }
        }
        return "127.0.0.1";
        throw new Exception("No network adapters with an IPv4 address in the system!");
        //string hostname = Dns.GetHostName();
        //IPAddress[] ipAddresses = Dns.GetHostAddresses(hostname);
        //for(int i  = 0; i < ipAddresses.Length; i++)
        //{
        //    print(ipAddresses[i]);
        //}
        //return ipAddresses[0].ToString();
    }

    public string GetSSID()
    {
        string ssid = "N/A";

        using (var activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity"))
        {
            var wifiManager = activity.Call<AndroidJavaObject>("getSystemService", "wifi");
            ssid = wifiManager.Call<AndroidJavaObject>("getConnectionInfo").Call<string>("getSSID");
        }
        return ssid;
    }

    private string SubnetMask()
    {
        var info = System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces();
        foreach (var item in info)
        {
            if (item.OperationalStatus == System.Net.NetworkInformation.OperationalStatus.Up &&
                item.NetworkInterfaceType != System.Net.NetworkInformation.NetworkInterfaceType.Loopback &&
                item.NetworkInterfaceType != System.Net.NetworkInformation.NetworkInterfaceType.Tunnel)
            {
                var ips = item.GetIPProperties();
                if (ips != null)
                {
                    foreach (var ip in ips.UnicastAddresses)
                    {
                        if (IPAddress == ip.Address.ToString())
                        {
                            string subnetmask = ip.IPv4Mask.ToString();// サブネットマスク
                            return subnetmask;
                        }
                    }
                }
            }
        }
        return "";
    }
}
