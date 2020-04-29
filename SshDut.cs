namespace OpenTap.Plugins.Ssh
{
    [Display("SSH DUT", Group: "SSH", Description: "DUT controlled via SSH.")]
    public class SshDut : SshResource, IDut
    { }
}
