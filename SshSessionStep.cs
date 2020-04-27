using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using OpenTap;
using Renci.SshNet;

namespace OpenTap.Plugins.Ssh
{
    [Display("SSH Session", "Starts/stops an SSH login session to a remote host. Use SSH Command steps as child steps to run commands in this session.", Group: "SSH")]
    [AllowChildrenOfType(typeof(SshCommandStep))]
    [AllowChildrenOfType(typeof(ScpUploadFileStep))]
    public class SshSessionStep : TestStep
    {
        internal class SshSessionStepResource : SshResource
        {
            public SshSessionStep SshSessionStep { get; internal set; }
            public override void Open()
            {
                base.IsOpened = true;
                // Do nothing. It is the SshSessionStep that controls the lifetime of this session
            }

            public override void Close()
            {
                base.IsOpened = false;
                // Do nothing. It is the SshSessionStep that controls the lifetime of this session
            }
            public override string ToString()
            {
                return SshSessionStep.Name;
            }
        }

        internal SshSessionStepResource SshResource;

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
            SshResource = new SshSessionStepResource(){ Connection = this.Connection, SshSessionStep = this };
            Name = "SSH Session on {User Name}@{Host Name}";
        }

        public override void Run()
        {
            using (var ssh = new SshClient(Connection.GetConnectionInfo()))
            //using (var scp = new ScpClient(Connection.GetConnectionInfo()))
            {
                ssh.Connect();
                SshClient = ssh;
                RunChildSteps();
                ssh.Disconnect();
            }
        }
    }
}
