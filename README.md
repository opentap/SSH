# SshSteps OpenTAP Plugin

Plugin to interact with instruments and DUT via ssh and scp.
The pluing is made in an educational manner showing
*  how to create a stand alone step with ssh session and command
*  how to create session in a step and execute command in a child step
*  how to create an instrument and execute commands to it using steps 

Furthermore, the plugin shows examples of how to utilize the OpenTAP API in creating a plugin that provides the user with many options while using few steps.

Some keys functionalities provided by the plugin are
*  establish ssh connection using password or ssh key authentication
*  manage the ssh session in a test step
*  manage the ssh session as an instrument
*  control when the connection is opened - upon start of test plan or when needed
*  perform basic scp actions

The plugin is very useful for performing remote commands to tools or DUTs as part of a test plan, which might allow one to control instances without having a specific plugin developed.
