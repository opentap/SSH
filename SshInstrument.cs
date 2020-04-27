namespace OpenTap.Plugins.Ssh
{
    [Display("SSH Instrument", Group: "SSH", Description: "Instrument controlled via SSH.")]
    public class SshInstrument : SshResource, IInstrument
    { }
}
