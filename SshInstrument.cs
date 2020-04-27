namespace OpenTap.Plugins.SshStep
{
    [Display("SSH Instrument", Group: "SSH", Description: "Instrument controlled via SSH.")]
    public class SshInstrument : SshResource, IInstrument
    { }
}
