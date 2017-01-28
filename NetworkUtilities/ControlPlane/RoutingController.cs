﻿using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetworkUtilities.Element;
using NetworkUtilities.GraphAlgorithm;

namespace NetworkUtilities.ControlPlane {
    class RoutingController : ControlPlaneElement {
        private List<Link> _linkList = new List<Link>();

        public RoutingController(NetworkAddress networkAddress) : base(networkAddress) {
        }

        public override void ReceiveMessage(SignallingMessage message) {
            switch (message.Operation) {
                case SignallingMessageOperation.RouteTableQuery:
                    var list = message.Payload as object[];
                    var snpps = list[0] as SubnetworkPointPool[];
                    var capacity = (int) list[1];
                    if (snpps != null) HandleRouteTableQuery(snpps[0], snpps[1], capacity, message);
                    break;
                case SignallingMessageOperation.LocalTopology:
                    _linkList.Add((Link) message.Payload);
                    break;
                case SignallingMessageOperation.NetworkTopology:

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void SendNetworkTopology(SignallingMessage message) {
            var networkTopology = message;
            networkTopology.Operation = SignallingMessageOperation.NetworkTopology;
            networkTopology.Payload = _linkList;
            //networkTopology.DestinationAddress = ???
            SendMessage(networkTopology);
        }

        private SubnetworkPointPool[] Route(SubnetworkPointPool snppStart, SubnetworkPointPool snppEnd, int capacity) {
            var preparedList = _linkList.Where(link => link.Capacity >= capacity).ToArray();
            var graph = new Graph();
            graph.Load(preparedList);
            var paths = Floyd.runAlgorithm(graph, snppStart, snppEnd);
            return paths[0].SubnetworkPointPools;
        }
        private void HandleRouteTableQuery(SubnetworkPointPool snppStart, SubnetworkPointPool snppEnd, int capacity, SignallingMessage message) {
           
            message.Operation = SignallingMessageOperation.RouteTableQueryResponse;
            message.Payload =Route(snppStart,snppEnd,capacity);
            SendMessage(message);
        }
    }
}