//Copyright 2019-2020 Keysight Technologies
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

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using OpenTap;
using Renci.SshNet;

namespace OpenTap.Plugins.Ssh
{
    [Display("SSH Session", "Starts/stops an SSH login session to a remote host. Use child steps to interact with this session.", Group: "SSH")]
    [AllowChildrenOfType(typeof(SshCommandStep))]
    [AllowChildrenOfType(typeof(ScpUploadFileStep))]
    [AllowChildrenOfType(typeof(ScpDownloadFileStep))]
    public class SshSessionStep : TestStep
    { 
        private SshResource SshResource;
        internal SshResource GetSshResource() => this.SshResource;

        /// <summary>
        /// The SSH client. Childsteps can use this. 
        /// </summary>
        public SshClient SshClient { get; private set; }


        #region Settings
        [EmbedProperties]
        public SshConnectionInfo Connection { get; set; }
        #endregion

        public SshSessionStep()
        {
            Connection = new SshConnectionInfo() { Owner = this };
            Name = "SSH Session on {User Name}@{Host Name}";
            SshResource = new SshInstrument(true, this) { Connection = this.Connection, Name = this.GetFormattedName() };
        }

        public override void Run()
        {
            using (var ssh = new SshClient(Connection.GetConnectionInfo()))
            {
                ssh.Connect();
                SshClient = ssh;
                RunChildSteps();
                ssh.Disconnect();
            }
        }
    }
}
