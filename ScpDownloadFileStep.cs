using System.Diagnostics;
using System.IO;

namespace OpenTap.Plugins.Ssh
{
    [Display("Download File", Group: "SSH", Description: "Uses SCP to downloads a file from a remote host defined by an SSH Session step, SSH Instrument or SSH Dut.")]
    public class ScpDownloadFileStep : SshStepBase
    {
        #region Settings
        [Display(Name: "Local Download Path", Description: "Save file to here", Group: "Action")]
        public string DownloadPath { get; set; }
        [Display(Name: "Remote File Path", Description: "Download file from here", Group: "Action")]
        public string FilePath { get; set; }
        #endregion

        /// <summary>
        /// Creates a new instance of <see cref="ScpDownloadFileStep">ScpDownloadFileStep</see> class.
        /// </summary>
        public ScpDownloadFileStep()
        {
            FilePath = "/home/ubuntu/myRemoteFile";
            DownloadPath = "/home/ubuntu/myLocalFile";
        }

        /// <summary>
        /// Run method is called when the step gets executed.
        /// </summary>
        public override void Run()
        {
            using (var ms = new MemoryStream())
            {
                var timer = Stopwatch.StartNew();
                SshResource.ScpClient.Download(FilePath, ms);
                using (FileStream file = new FileStream(DownloadPath, FileMode.Create, FileAccess.Write))
                {
                    byte[] bytes = new byte[ms.Length];
                    ms.Seek(0, SeekOrigin.Begin);
                    int len = ms.Read(bytes, 0, (int)ms.Length);
                    file.Write(bytes, 0, len);
                    ms.Close();
                    Log.Debug(timer, $"Downloaded {len / 1024} Kb from {FilePath} to {DownloadPath}.");
                }
            }
        }
    }
}
