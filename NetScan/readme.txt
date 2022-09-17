ABOUT
-----
NetScan is a command line tool that searches your local network for connected
devices.  There are many great and far more robust tools for such a task
(for example: https://nmap.org/) that I highly recommend you check out.
However, I wrote this for two reasons:

  1. Nmap wasn't always returning host names, even when I tried to force it.
  2. I wanted the output in a clean, easy-to-read table.

INSTALLATION
------------
Run "setup.exe."  It will check to ensure you have the correct .NET framework
version installed on your computer, then proceed to install netscan in your
Program Files folder, typically:

  C:\Program Files (x86)\ReardenTools\NetScan

I recommend you add the installation folder to your computer's PATH variable
so that you can run netscan from anywhere.

USAGE
-----
Currently there are no command-line arguments or parameters.  Just open a
command prompt, go to the installation folder (unless you updated PATH), and
type: "netscan"

CREDIT
------
The tool uses the excellent package IPNetwork by Luc Dvchosal for some of the
network discovery tasks.  Thank you for making your hard work available!
Check it out at https://github.com/lduchosal/ipnetwork

FEEDBACK, QUESTIONS, SUPPORT
----------------------------
I welcome your input!  Feel free to reach out:

  https://reardentools.com/
  
  https://github.com/dwt12777/NetScan