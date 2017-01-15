﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkUtilities.ControlPlane {
    public abstract class ControlPlaneElement {
        public delegate void MessageToSendHandler(object sender, SignallingMessage message);

        private readonly List<UniqueId> _currentlyHandledSessions = new List<UniqueId>();

        public event MessageToSendHandler OnMessageToSend;

        protected void SendMessage(SignallingMessage message) {
            OnMessageToSend?.Invoke(this, message);
        }

        protected bool IsCurrentlyHandled(UniqueId sessionId) {
            return _currentlyHandledSessions.Contains(sessionId);
        }

        private void StartSession(UniqueId sessionId) {
            _currentlyHandledSessions.Add(sessionId);
        }

        protected void EndSession(UniqueId sessionId) {
            _currentlyHandledSessions.Remove(sessionId);
        }

        public virtual void RecieveMessage(SignallingMessage message) {
            var sessionId = message.SessionId;

            if (!IsCurrentlyHandled(sessionId)) {
                StartSession(sessionId);
            }
        }
    }
}
