using Renci.SshNet;

namespace OpenTap.Plugins.Ssh
{
    [Display("Ssh Resource Baseclass", Group: "SSH", Description: "Resource controlled via SSH.")]
    public abstract class SshResource : Resource
    {
        protected SshClient sshClient;
        protected ScpClient scpClient;
        private bool IsResourceOpened;

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
        
        /// <summary>
        /// Get an SshClient to the host represented by this resource. 
        /// The connection will established when this property is first accessed, 
        /// and terminated again when the Resource is closed (normally at the end of the TestPlan run).
        /// </summary>
        public SshClient SshClient
        {
            get
            {
                if (sshClient == null && IsResourceOpened)
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
                if (scpClient == null && IsResourceOpened)
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
            IsResourceOpened = true;
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
            IsResourceOpened = false;
            if (sshClient == null)
                sshClient.Disconnect();
            sshClient = null;
            if (scpClient == null)
                scpClient.Disconnect();
            scpClient = null;
            IsConnected = true;
        }

        public override string ToString()
        {
            return base.ToString() + $"({Connection.Username}@{Connection.Host}:{Connection.Port})";
        }
    }
}
