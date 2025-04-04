﻿//Copyright 2019-2020 Keysight Technologies
//
//Licensed under the Apache License, Version 2.0 (the "License");
//you may not use this file except in compliance with the License.
//You may obtain a copy of the License at
//
//http://www.apache.org/licenses/LICENSE-2.0
//
//Unless required by applicable law or agreed to in writing, software
//distributed under the License is distributed on an "AS IS" BASIS,
//WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//See the License for the specific language governing permissions and
//limitations under the License.

using Renci.SshNet;
using System.ComponentModel;

namespace OpenTap.Plugins.Ssh
{
    [Display("Ssh Resource Baseclass", Group: "SSH", Description: "Resource controlled via SSH.")]
    public abstract class SshResource : Resource
    {
        protected SshClient sshClient;
        protected ScpClient scpClient;
        protected bool IsOpened;

        #region Settings
        [EmbedProperties]
        public SshConnectionInfo Connection { get; set; }

        [Display("Lazy SSH connection", "Connect SSH client lazily (when it is needed by a Test Step) instead of at the beginning of the Test Plan run.", "Advanced", Order: 6)]
        public bool LazyConnectSsh { get; set; } = false;
        [Display("Lazy SCP connection", "Connect SCP client lazily (when it is needed by a Test Step) instead of at the beginning of the Test Plan run.", "Advanced", Order: 7)]
        public bool LazyConnectScp { get; set; } = true;
        #endregion

        public SshResource()
        {
            Name = "Ssh";
            Connection = new SshConnectionInfo() { Owner = this };
        }
        protected SshResource(bool session, ITestStep step)
        {
            Name = "Ssh";
            Connection = new SshConnectionInfo() { Owner = this };
            _step = step;
            IsSession = session;
        }
        
        /// <summary>
        /// Get an SshClient to the host represented by this resource. 
        /// The connection will established when this property is first accessed, 
        /// and terminated again when the Resource is closed (normally at the end of the TestPlan run).
        /// </summary>
        public SshClient SshClient
        {
            get
            {
                if (sshClient == null && IsOpened)
                {
                    sshClient = new SshClient(Connection.GetConnectionInfo());
                    sshClient.Connect();
                    IsConnected = true;
                }
                return sshClient;
            }
        }

        /// <summary>
        /// Get an ScpClient to the host represented by this resource. 
        /// The connection will established when this property is first accessed, 
        /// and terminated again when the Resource is closed (normally at the end of the TestPlan run).
        /// </summary>
        public ScpClient ScpClient
        {
            get
            {
                if (scpClient == null && IsOpened)
                {
                    scpClient = new ScpClient(Connection.GetConnectionInfo());
                    scpClient.Connect();
                    IsConnected = true;
                }
                return scpClient;
            }
        }

        /// <summary>
        /// Open procedure for the instrument.
        /// </summary>
        public override void Open()
        {
            IsOpened = true;
            if(!LazyConnectSsh)
            {
                IsConnected = SshClient.IsConnected; // just do somthing to trigger the getter.
            }
            if (!LazyConnectScp)
            {
                IsConnected = ScpClient.IsConnected; // just do somthing to trigger the getter.
            }
        }

        /// <summary>
        /// Close procedure for the instrument.
        /// </summary>
        public override void Close()
        {
            IsOpened = false;
            if (sshClient != null)
                sshClient.Disconnect();
            sshClient = null;
            if (scpClient != null)
                scpClient.Disconnect();
            scpClient = null;
            IsConnected = true;
        }

        [Browsable(false)]
        public bool IsSession { get; set; } 
        // If this is from a session step, and _step is null, then this resource is invalid.
        // In that case, a step should use the resource from its parent instead.
        // This can happen in copy-paste scenarios, or after serialization.
        internal bool Invalid => _step == null && IsSession; 
        private ITestStep _step;

        public override string ToString()
        {
            if (IsSession) return _step.GetFormattedName();
            return base.ToString() + $"({Connection.Username}@{Connection.Host}:{Connection.Port})";
        }
    }
}
