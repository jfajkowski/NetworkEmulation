﻿using System.Collections.Generic;
using System.Text;
using System.Threading;
using NetworkUtilities;
using NetworkUtilities.Element;
using NetworkUtilities.Network;

namespace ClientNode {
    public class ClientNode : Node {
        public List<ClientTableRow> ClientTableList = new List<ClientTableRow>();

        public ClientNode(ClientNodeModel parameters)
            : base(parameters.IpAddress, parameters.CableCloudListeningPort, parameters.CableCloudDataPort) {
            CableCloudMessage.MaxAtmCellsNumber = parameters.MaxAtmCellsNumberInCableCloudMessage;
            ClientName = parameters.ClientName;
        }

        public string ClientName { get; private set; }
        public event MessageHandler OnMessageReceived;
        public event MessageHandler OnNewClientTableRow;

        public void ReadClientTable(ClientNodeModel parameters) {
            if (parameters.ClientTable != null) foreach (var client in parameters.ClientTable) AddClient(client);
        }

        protected void MessageReceived(string message) {
            OnMessageReceived?.Invoke(this, message);
        }

        protected void AddClientToComboBox(string clientName) {
            OnNewClientTableRow?.Invoke(this, clientName);
        }

        public void AddClient(ClientTableRow clientTableRow) {
            ClientTableList.Add(clientTableRow);
            AddClientToComboBox(clientTableRow.ClientName);
        }

        public void SendMessage(string message, string receiverName) {
            var clientTableRow = SearchUsingClientName(receiverName);

            if (clientTableRow != null) SendCableCloudMessage(message, clientTableRow);
            else OnUpdateState("Client " + receiverName + " not found.");
        }

        private ClientTableRow SearchUsingClientName(string clientName) {
            foreach (var clientTableRow in ClientTableList)
                if (clientTableRow.ClientName.Equals(clientName)) return clientTableRow;
            return null;
        }

        private void SendCableCloudMessage(string message, ClientTableRow clientTableRow) {
            var vci = clientTableRow.Vci;
            var vpi = clientTableRow.Vpi;
            var portNumber = clientTableRow.PortNumber;

            var cableCloudMessages = Generate(portNumber, vpi, vci, message);

            foreach (var cableCloudMessage in cableCloudMessages) {
                Send(cableCloudMessage);
                OnUpdateState("Sent: " + ExtractAtmCells(cableCloudMessage).Count + " ATMCells.");
            }
        }


        public static List<CableCloudMessage> Generate(int portNumber, int vpi, int vci, string message) {
            var atmCells = AtmCell.Generate(vpi, vci, message);
            var cableCloudMessages = new List<CableCloudMessage>();

            while (atmCells.Count >= CableCloudMessage.MaxAtmCellsNumber) {
                var atmCellsPart = atmCells.GetRange(0, CableCloudMessage.MaxAtmCellsNumber);
                atmCells.RemoveRange(0, CableCloudMessage.MaxAtmCellsNumber);
                cableCloudMessages.Add(new CableCloudMessage(portNumber, atmCellsPart));
            }
            cableCloudMessages.Add(new CableCloudMessage(portNumber, atmCells));

            return cableCloudMessages;
        }

        public string ToString(CableCloudMessage cableCloudMessage) {
            var sb = new StringBuilder();
            foreach (var cell in ExtractAtmCells(cableCloudMessage)) sb.Append(Encoding.UTF8.GetString(cell.Data));
            return sb.ToString();
        }

        protected override void Receive(CableCloudMessage cableCloudMessage) {
            MessageReceived(ToString(cableCloudMessage));
            OnUpdateState("Received: " + ExtractAtmCells(cableCloudMessage).Count + " ATMCells.");
        }
    }
}