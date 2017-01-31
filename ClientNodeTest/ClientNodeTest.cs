﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetworkUtilities.Network.ClientNode;

namespace ClientNodeTest {
    [TestClass]
    public class ClientNodeTest {
        [TestMethod]
        public void AddClientTest() {
            var parameters = new ClientNodeModel {
                MaxAtmCellsNumberInCableCloudMessage = 100,
                ClientName = "Janusz",
                CableCloudListeningPort = 10000,
                IpAddress = "127.0.0.1"
            };
        }
    }
}