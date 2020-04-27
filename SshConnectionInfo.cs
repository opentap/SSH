using System;
using Renci.SshNet;
using System.IO;
using OpenTap;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using System.ComponentModel;
using System.Security;
using System.Runtime.InteropServices;

namespace OpenTap.Plugins.SshStep
{
    /// <summary>
    /// Class containing settings related to an SSH connection (host name, user name, etc.). 
    /// These settings are embedded (<see cref="OpenTap.EmbedPropertiesAttribute"/>) in <see cref="SshInstrument"/> and <see cref="SshSessionStep"/>
    /// </summary>
    public class SshConnectionInfo
    {
        public enum SshAuthMethod
        {
            Password,
            [Display("SSH Key")]
            Key,
        }

        #region Settings
        [Display("Host Name", "Host name or IP address of the machine to connect to.", Order: 0)]
        public string Host { get; set; }
        [Display(Name: "Host Port", Order: 1)]
        public int Port { get; set; }
        [Display("User Name", Order: 2)]
        public string Username { get; set; }
        [Display("Authentication Method", Order: 3)]
        public SshAuthMethod AuthMethod { get; set; }
        [Display("Password", Order: 4)]
        [EnabledIf(nameof(AuthMethod), SshAuthMethod.Password, HideIfDisabled = true)]
        public SecureString Password { get; set; }
        private string _privateKey;

        [Display("SSH Private Key", Order: 4)]
        [EnabledIf(nameof(AuthMethod), SshAuthMethod.Key, HideIfDisabled = true)]
        [FilePath]
        public string PrivateKey
        {
            get => _privateKey;
            set
            {
                if (File.Exists(value))
                {
                    string hotFileContent = File.ReadAllText(value);
                    _privateKey = hotFileContent;
                }
                else
                {
                    _privateKey = value;
                }
            }
        }
        #endregion


        public SshConnectionInfo()
        {
            Host = "localhost";
            Port = 22;
            Username = "demo";
            Password = new SecureString();
        }

        /// <summary>
        /// Get an SSH client configured with the host settings specified in this class.
        /// </summary>
        public Renci.SshNet.ConnectionInfo GetConnectionInfo()
        {
            if (AuthMethod == SshAuthMethod.Key)
            {
                using (var stream = new MemoryStream())
                using (var writer = new StreamWriter(stream))
                {
                    writer.Write(PrivateKey);
                    writer.Flush();
                    stream.Position = 0;
                    PrivateKeyFile sshKey = new PrivateKeyFile(stream);
                    return new PrivateKeyConnectionInfo(Host, Port, Username, new[] { sshKey });
                }
            }
            else if (AuthMethod == SshAuthMethod.Password)
            {
                return new PasswordConnectionInfo(Host, Port, Username, ConvertToUnsecureString(Password));
            }
            else
                throw new NotImplementedException();
        }

        [XmlIgnore]
        public object Owner { get; set; }

        public override string ToString()
        {
            if (Owner is ITestStep step)
                return step.Name;
            if (Owner is IResource resource)
                return resource.Name;
            return Owner.ToString();
        }

        private static string ConvertToUnsecureString(System.Security.SecureString securePassword)
        {
            if (securePassword == null)
                throw new ArgumentNullException(nameof(securePassword));

            IntPtr unmanagedString = IntPtr.Zero;
            try
            {
                unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(securePassword);
                return Marshal.PtrToStringUni(unmanagedString);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
            }
        }
    }
}
