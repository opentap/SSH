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

using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Security.Cryptography;

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

        public List<Pid> BackgroundProcesses { get; set; }
        #endregion

        public SshResource()
        {
            Name = "Ssh";
            Connection = new SshConnectionInfo() { Owner = this };
            BackgroundProcesses = new List<Pid>();
        }
        protected SshResource(bool session, ITestStep step)
        {
            Name = "Ssh";
            Connection = new SshConnectionInfo() { Owner = this };
            _step = step;
            IsSession = session;
            BackgroundProcesses = new List<Pid>();
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
            // Close all bakground processes before disconnecting ssh client
            if (_KillAllBackgroundProcesses())
            if (!_KillAllBackgroundProcesses())
            {
                Log.Warning("Some background processes could not be killed correctly");
            }

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

        // <summary>
        // Starts a process with the given command in the background
        // The process PID is stored in BackgroundProcesses to keep track of all processes started by this SshResource
        // </summary>
        public Pid StartBackgroundProcess(string command)
        {
            // nohup allows the command to run even when the SSH session ends
            // output is suppressed
            // the PID of the last background process '$!' is printed to output
            SshCommand sshCmd = SshClient.CreateCommand($"nohup {command} > /dev/null 2>&1 & echo $!");
            string cmdOut = sshCmd.Execute();
            
            Log.Debug("Running command: " + sshCmd.CommandText);

            Pid pid = new Pid(cmdOut);
            BackgroundProcesses.Add(pid);
            return pid;
        }

        // <summary>
        // Checks if background process with the given pid is running
        // This only works if the process was started within the same session of this SshResource
        // </summary>
        public bool IsBackgroundProcessRunning(Pid pid)
        {
            if (!BackgroundProcesses.Contains(pid))
            {
                Log.Debug($"PID={pid} was not started in this session or it is not alive anymore");
                return false;
            }

            SshCommand command = SshClient.CreateCommand($"ps -p {pid}");
            command.Execute();

            if (command.ExitStatus == 0)
            {
                Log.Debug($"Process with PID={pid} is alive");
                return true;
            }

            Log.Debug($"Process with PID={pid} is not alive. Removing it from the list of background processes");
            BackgroundProcesses.Remove(pid);
            return false;
        }

        // <summary>
        // Kills a background process
        // This only works if the process was started within the same session of this SshResource
        // </summary>
        public bool KillBackgroundProcess(Pid pid)
        {
            if (!IsBackgroundProcessRunning(pid))
            {
                Log.Debug($"No process with PID={pid} to be killed");
                return false;
            }

            SshCommand command = SshClient.CreateCommand($"kill {pid}");
            command.Execute();

            if (command.ExitStatus == 0)
            {
                Log.Debug($"Gracefully killed PID={pid}");
                BackgroundProcesses.Remove(pid);
                return true;
            }

            command = SshClient.CreateCommand($"kill -9 {pid}");
            command.Execute();

            if (command.ExitStatus == 0)
            {
                Log.Debug($"SIGKILLed PID={pid}");
                BackgroundProcesses.Remove(pid);
                return true;
            }

            Log.Error($"Failed killing PID={pid}");
            return false;
        }

        // <summary>
        // Iterates over all background processes, killing them if they are still running
        // If all processes were killed correctly, it returns true, otherwise it returns false
        // <summary>
        private bool _KillAllBackgroundProcesses()
        {
            Log.Debug("Killing all background processes");

            bool killedAll = true;
            while (BackgroundProcesses.Count > 0)
            {
                Pid pid = BackgroundProcesses[0];
                bool killed = KillBackgroundProcess(pid);

                if (!killed)
                {
                    killedAll = false;
                    Log.Debug($"Process with PID={pid} could not be killed correctly");
                }
            }

            return killedAll;
        }
    }
}
