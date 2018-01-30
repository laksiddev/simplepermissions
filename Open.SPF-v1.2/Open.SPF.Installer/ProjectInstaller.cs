using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Open.SPF.Installer
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();

            // To create this Installer... 
            // Add a new Class Library project named YourProject.Installer
            // Add a "new item" then find "Installer" under the "Code" section
            // Call the installer "ProjectInstaller"
            // Open the ProjectInstaller.cs file
            // Add the line "using System.Diagnostics;" above
            // Insert the follwoing 4 lines, changing the "Source" 
            //		to the name that will be the EventSource for
            //		all Event Log entries.
            // You can add additional eventLogInstallers as needed.

            EventLogInstaller eventlogInstaller = new EventLogInstaller();
            eventlogInstaller.Log = "Application";
            eventlogInstaller.Source = Open.SPF.Utility.EventLogUtility.DefaultEventLogSource;
            Installers.Add(eventlogInstaller);

            // To use this Installer...
            // After you compile the YourProject.Installer.dll
            //		Start -> All Programs -> Microsoft Visual Studio 201x ->
            //		Visual Studio .NET Tools -> Visual Studio .NET 201x Command Prompt
            // This command prompt has the proper path to reference the VS.NET binaries
            // In the command window change directories to the location of YourProject.Installer.dll
            // Type the following "installutil YourProject.Installer.dll"
            // You should see a bunch of messages like "The Install phase completed successfully" and 
            //		"The Commit phase completed successfully" ending with 
            //		"The transacted install has completed."
            // You can verify the registry settings at:
            //		HKLM\System\CurrentControlSet\Services\EventLog\Application\"EventSourceName from above"
        }
    }
}
