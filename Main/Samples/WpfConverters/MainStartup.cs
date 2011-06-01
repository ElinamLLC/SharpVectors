using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;

using System.Windows;
using System.Windows.Threading;

namespace SharpVectors.Converters
{
    static class MainStartup
    {
        #region Main Method

        [STAThread]
        static int Main(string[] args)
        {
            // 1. Get a pointer to the foreground window.  The idea here is that                
            // If the user is starting our application from an existing console                
            // shell, that shell will be the uppermost window.  We'll get it                
            // and attach to it.                
            // Uses this idea from, Jeffrey Knight, since it fits our model instead
            // of the recommended ATTACH_PARENT_PROCESS (DWORD)-1 parameter
            bool startedInConsole = false;
            IntPtr ptr = ConverterWindowsAPI.GetForegroundWindow();
            int processId = -1;
            ConverterWindowsAPI.GetWindowThreadProcessId(ptr, out processId);
            Process process = Process.GetProcessById(processId);

            startedInConsole = (process != null && String.Equals(
                process.ProcessName, "cmd", StringComparison.OrdinalIgnoreCase));

            // 2. Parse the command-line options to determine the requested task...
            ConverterCommandLines commandLines = new ConverterCommandLines(args);
            bool parseSuccess = commandLines.Parse(startedInConsole);

            ConverterUIOption uiOption = commandLines.Ui;
            bool isQuiet = (uiOption == ConverterUIOption.None);
            // 3. If the parsing is successful...
            if (parseSuccess)
            {
                // A test for possible file drag/drop on application icon
                int sourceCount = 0;
                if (commandLines != null && !commandLines.IsEmpty)
                {
                    IList<string> sourceFiles = commandLines.SourceFiles;
                    if (sourceFiles != null)
                    {
                        sourceCount = sourceFiles.Count;
                    }
                    else
                    {
                        string sourceFile = commandLines.SourceFile;
                        if (!String.IsNullOrEmpty(sourceFile) && 
                            File.Exists(sourceFile))
                        {
                            sourceCount = 1;
                        }
                    }
                }

                // For the console or quiet mode...
                if (startedInConsole)
                {
                    // If explicitly asked for a Windows GUI...
                    if (uiOption == ConverterUIOption.Windows)
                    {
                        if (args != null && args.Length != 0 &&
                            args.Length == sourceCount)
                        {
                            // if it passes our simple drag/drop test, show
                            // the minimal window for quick conversion of files...
                            return MainStartup.RunWindows(commandLines, false);
                        }
                        else
                        {
                            //...otherwise, display the main window.
                            return MainStartup.RunWindows(commandLines, true);
                        }
                    }
                    else
                    {   
                        int exitCode = MainStartup.RunConsole(commandLines,
                            isQuiet, startedInConsole, process);

                        // Exit the application...
                        ConverterWindowsAPI.ExitProcess((uint)exitCode);

                        return exitCode;
                    }
                }
                else if (isQuiet || uiOption == ConverterUIOption.Console)
                {
                    int exitCode = MainStartup.RunConsole(commandLines,
                        isQuiet, startedInConsole, process);

                    // Exit the application...
                    ConverterWindowsAPI.ExitProcess((uint)exitCode);

                    return exitCode;
                }
                else //...for the GUI Windows mode...
                {            
                    if (args != null && args.Length != 0 &&
                        args.Length == sourceCount)
                    {
                        // if it passes our simple drag/drop test, show
                        // the minimal window for quick conversion of files...
                        return MainStartup.RunWindows(commandLines, false);
                    }
                    else
                    {
                        //...otherwise, display the main window.
                        return MainStartup.RunWindows(commandLines, true);
                    }         
                }
            }
            else //... else if the parsing failed...
            {
                if (commandLines != null)
                {
                    commandLines.ShowHelp = true;
                }

                if (startedInConsole ||
                    (commandLines != null && uiOption == ConverterUIOption.Console))
                {
                    int exitCode = MainStartup.RunConsoleHelp(commandLines,
                        isQuiet, startedInConsole, process);

                    // Exit the application...
                    ConverterWindowsAPI.ExitProcess((uint)exitCode);

                    return exitCode;
                }
                else
                {
                    return MainStartup.RunWindows(commandLines, true);
                }
            }
        }

        #endregion

        #region Other Methods

        static int RunConsole(ConverterCommandLines commandLines, 
            bool isQuiet, bool startedInConsole, Process process)
        {
            ConsoleApplication theApp = new ConsoleApplication(process);
            theApp.CommandLines = commandLines;
            theApp.InitializeComponent(startedInConsole, isQuiet);

            return theApp.Run();
        }

        static int RunConsoleHelp(ConverterCommandLines commandLines, 
            bool isQuiet, bool startedInConsole, Process process)
        {
            ConsoleApplication theApp = new ConsoleApplication(process);
            theApp.CommandLines = commandLines;
            theApp.InitializeComponent(startedInConsole, isQuiet);

            return theApp.Help();
        }

        static int RunWindows(ConverterCommandLines commandLines, 
            bool isMainWindow)
        {
            MainApplication theApp = new MainApplication();
            theApp.CommandLines = commandLines;
            theApp.InitializeComponent(isMainWindow);

            return theApp.Run();
        }

        #endregion
    }
}
