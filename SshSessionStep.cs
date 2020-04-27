using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using OpenTap;
using Renci.SshNet;

namespace OpenTap.Plugins.SshStep
{
    [Display("SSH Session", "Starts/stops an SSH login session to a remote host. Use SSH Command steps as child steps to run commands in this session.", Group: "SSH")]
    [AllowChildrenOfType(typeof(SshCommandStep))]
    public class SshSessionStep : TestStep
    {
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
