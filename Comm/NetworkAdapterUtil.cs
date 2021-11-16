using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;

namespace YEasyInstaller
{
    public class NetworkAdapterUtil
    {

  //      /// <summary>
  //      /// 获取所有适配器类型，适配器被禁用则不能获取到
  //      /// </summary>
  //      /// <returns></returns>
  //      public List<NetworkAdapter> GetAllNetworkAdapters() //如果适配器被禁用则不能获取到
  //      {
  //          IEnumerable<NetworkInterface> adapters = NetworkInterface.GetAllNetworkInterfaces(); //得到所有适配器
  //          return GetNetworkAdapters(adapters);

  //      }

  //      /// <summary>
  //      /// 根据条件获取IP地址集合，
  //      /// </summary>
  //      /// <param name="adapters">网络接口地址集合</param>
  //      /// <param name="adapterTypes">网络连接状态，如,UP,DOWN等</param>
  //      /// <returns></returns>
  //      private List<NetworkAdapter> GetNetworkAdapters(IEnumerable<NetworkInterface> adapters, params NetworkInterfaceType[] networkInterfaceTypes)
  //      {
  //          adapterList = new List<NetworkAdapter>();

  //          foreach (NetworkInterface adapter in adapters)
  //          {
  //              if (networkInterfaceTypes.Length <= 0) //如果没传可选参数，就查询所有
  //              {
  //                  if (adapter != null)
  //                  {
  //                      NetworkAdapter adp = SetNetworkAdapterValue(adapter);
  //                      adapterList.Add(adp); adapter.GetIPProperties().GetIPv4Properties().IsDhcpEnabled;
  //                  }
  //                  else
  //                  {
  //                      return null;
  //                  }
  //              }
  //              else //过滤查询数据
  //              {
  //                  foreach (NetworkInterfaceType networkInterfaceType in networkInterfaceTypes)
  //                  {
  //                      if (adapter.NetworkInterfaceType.ToString().Equals(networkInterfaceType.ToString()))
  //                      {
  //                          adapterList.Add(SetNetworkAdapterValue(adapter));
  //                          break; //退出当前循环
  //                      }
  //                  }
  //              }
  //          }
  //          return adapterList;
  //      }

  ///// <summary>
  ///// 设置网络适配器信息
  ///// </summary>
  ///// <param name="adapter"></param>
  ///// <returns></returns>
  //      private NetworkAdapter SetNetworkAdapterValue(NetworkInterface adapter)
  //      {
  //          NetworkAdapter networkAdapter = new NetworkAdapter();
  //          IPInterfaceProperties ips = adapter.GetIPProperties();
  //          networkAdapter.Description = adapter.Name;
  //          networkAdapter.NetworkInterfaceType = adapter.NetworkInterfaceType.ToString();
  //          networkAdapter.Speed = adapter.Speed / 1000 / 1000 + "MB"; //速度
  //          networkAdapter.MacAddress = adapter.GetPhysicalAddress(); //物理地址集合
  //          networkAdapter.NetworkInterfaceID = adapter.Id;//网络适配器标识符

  //          networkAdapter.Getwaryes = ips.GatewayAddresses; //网关地址集合
  //          networkAdapter.IPAddresses = ips.UnicastAddresses; //IP地址集合
  //          networkAdapter.DhcpServerAddresses = ips.DhcpServerAddresses;//DHCP地址集合
  //          networkAdapter.IsDhcpEnabled = ips.GetIPv4Properties() == null ? false : ips.GetIPv4Properties().IsDhcpEnabled; //是否启用DHCP服务

  //          IPInterfaceProperties adapterProperties = adapter.GetIPProperties();//获取IPInterfaceProperties实例  
  //          networkAdapter.DnsAddresses = adapterProperties.DnsAddresses; //获取并显示DNS服务器IP地址信息 集合
  //          return networkAdapter;
  //      }

    }  
}
