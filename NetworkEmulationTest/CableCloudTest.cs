﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using NetworkEmulation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NetworkEmulationTest {
    [TestClass]
    public class CableCloudTest {
        private UdpClient udpClient;
        [TestMethod]
        public void CableCloudBindEndpointTest() {
            CableCloud cableCloud = new CableCloud();

            UdpClient udpClient = new UdpClient();

            byte[] bytesToSend = BitConverter.GetBytes(10001);

            var nodeIpEndpoint = new IPEndPoint(IPAddress.Loopback, 10001);
            var tcpListener = new TcpListener(nodeIpEndpoint);
            tcpListener.Start();

            var acceptTask = Task.Run(async () => {
                await tcpListener.AcceptTcpClientAsync();

                var nodesTcpClients = (Dictionary<int,TcpClient>) new PrivateObject(cableCloud).GetField("nodesTcpClients");
                Assert.AreEqual(1, nodesTcpClients.Count);
            });

            var cableCloudIpEndpoint = new IPEndPoint(IPAddress.Loopback, 10000);
            udpClient.Send(bytesToSend, bytesToSend.Length, cableCloudIpEndpoint);

            acceptTask.Wait();
        }
    }
}
