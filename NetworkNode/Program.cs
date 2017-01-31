﻿using System;
using NetworkUtilities.Network.NetworkNode;
using NetworkUtilities.Utilities.Serialization;

namespace NetworkNode {
    internal class Program {
        private static void Main(string[] args) {
            var xmlArgs = string.Join(" ", args);
            Console.WriteLine(XmlSerializer.FormatXml(xmlArgs));

            var parameters =
                (NetworkNodeModel) XmlSerializer.Deserialize(xmlArgs, typeof(NetworkNodeModel));

            Console.Title =
                $"Network Node ({parameters.NetworkAddress})";

            var networkNode = new NetworkUtilities.Network.NetworkNode.NetworkNode(parameters);
            networkNode.UpdateState += (sender, state) => Console.WriteLine(state);
            networkNode.Initialize();
        }
    }
}