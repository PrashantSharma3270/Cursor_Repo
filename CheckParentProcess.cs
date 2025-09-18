using System;
using System.Diagnostics;
using System.Management;
using System.Runtime.InteropServices;

namespace ProcessChecker
{
    public class ParentProcessChecker
    {
        // Windows API for checking if process is responding
        [DllImport("kernel32.dll")]
        private static extern bool IsProcessRunning(int processId);

        [DllImport("kernel32.dll")]
        private static extern IntPtr OpenProcess(uint processAccess, bool bInheritHandle, int processId);

        [DllImport("kernel32.dll")]
        private static extern bool CloseHandle(IntPtr hObject);

        private const uint PROCESS_QUERY_INFORMATION = 0x0400;

        /// <summary>
        /// Check if a parent process ID is running using Process.GetProcessById
        /// </summary>
        /// <param name="processId">The process ID to check</param>
        /// <returns>True if process is running, false otherwise</returns>
        public static bool IsParentProcessRunning(int processId)
        {
            try
            {
                Process process = Process.GetProcessById(processId);
                
                if (process != null && !process.HasExited)
                {
                    Console.WriteLine($"✅ Parent Process ID {processId} is RUNNING");
                    Console.WriteLine($"Process Name: {process.ProcessName}");
                    Console.WriteLine($"Process Path: {process.MainModule?.FileName ?? "N/A"}");
                    Console.WriteLine($"Start Time: {process.StartTime}");
                    Console.WriteLine($"CPU Time: {process.TotalProcessorTime}");
                    Console.WriteLine($"Memory Usage: {process.WorkingSet64 / (1024 * 1024):F2} MB");
                    return true;
                }
                else
                {
                    Console.WriteLine($"❌ Parent Process ID {processId} is NOT RUNNING");
                    return false;
                }
            }
            catch (ArgumentException)
            {
                Console.WriteLine($"❌ Parent Process ID {processId} is NOT RUNNING (Process not found)");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error checking process: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Check if a parent process ID is running using Windows API
        /// </summary>
        /// <param name="processId">The process ID to check</param>
        /// <returns>True if process is running, false otherwise</returns>
        public static bool IsParentProcessRunningAPI(int processId)
        {
            try
            {
                IntPtr processHandle = OpenProcess(PROCESS_QUERY_INFORMATION, false, processId);
                
                if (processHandle != IntPtr.Zero)
                {
                    Console.WriteLine($"✅ Parent Process ID {processId} is RUNNING (API)");
                    CloseHandle(processHandle);
                    return true;
                }
                else
                {
                    Console.WriteLine($"❌ Parent Process ID {processId} is NOT RUNNING (API)");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ API Error: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Check if a parent process ID is running using WMI
        /// </summary>
        /// <param name="processId">The process ID to check</param>
        /// <returns>True if process is running, false otherwise</returns>
        public static bool IsParentProcessRunningWMI(int processId)
        {
            try
            {
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(
                    $"SELECT * FROM Win32_Process WHERE ProcessId = {processId}"))
                {
                    ManagementObjectCollection processes = searcher.Get();
                    
                    if (processes.Count > 0)
                    {
                        foreach (ManagementObject process in processes)
                        {
                            Console.WriteLine($"✅ Parent Process ID {processId} is RUNNING (WMI)");
                            Console.WriteLine($"Process Name: {process["Name"]}");
                            Console.WriteLine($"Command Line: {process["CommandLine"]}");
                            Console.WriteLine($"Creation Date: {process["CreationDate"]}");
                            return true;
                        }
                    }
                    else
                    {
                        Console.WriteLine($"❌ Parent Process ID {processId} is NOT RUNNING (WMI)");
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ WMI Error: {ex.Message}");
                return false;
            }
            
            return false;
        }

        /// <summary>
        /// Get detailed information about a process if it's running
        /// </summary>
        /// <param name="processId">The process ID to get information for</param>
        /// <returns>Process information or null if not found</returns>
        public static ProcessInfo GetProcessInfo(int processId)
        {
            try
            {
                Process process = Process.GetProcessById(processId);
                
                if (process != null && !process.HasExited)
                {
                    return new ProcessInfo
                    {
                        ProcessId = process.Id,
                        ProcessName = process.ProcessName,
                        ProcessPath = process.MainModule?.FileName,
                        StartTime = process.StartTime,
                        CpuTime = process.TotalProcessorTime,
                        MemoryUsage = process.WorkingSet64,
                        IsRunning = true
                    };
                }
            }
            catch (ArgumentException)
            {
                return new ProcessInfo { ProcessId = processId, IsRunning = false };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting process info: {ex.Message}");
                return new ProcessInfo { ProcessId = processId, IsRunning = false };
            }
            
            return new ProcessInfo { ProcessId = processId, IsRunning = false };
        }
    }

    /// <summary>
    /// Data class to hold process information
    /// </summary>
    public class ProcessInfo
    {
        public int ProcessId { get; set; }
        public string ProcessName { get; set; }
        public string ProcessPath { get; set; }
        public DateTime StartTime { get; set; }
        public TimeSpan CpuTime { get; set; }
        public long MemoryUsage { get; set; }
        public bool IsRunning { get; set; }

        public override string ToString()
        {
            if (!IsRunning)
                return $"Process ID {ProcessId} is not running";
                
            return $"Process: {ProcessName} (ID: {ProcessId})\n" +
                   $"Path: {ProcessPath}\n" +
                   $"Start Time: {StartTime}\n" +
                   $"CPU Time: {CpuTime}\n" +
                   $"Memory: {MemoryUsage / (1024 * 1024):F2} MB";
        }
    }

    /// <summary>
    /// Example usage and testing class
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("🔍 Parent Process ID Checker");
            Console.WriteLine("=" * 50);

            // Get process ID from command line arguments or use current process
            int processId;
            if (args.Length > 0 && int.TryParse(args[0], out processId))
            {
                Console.WriteLine($"Checking Process ID: {processId}");
            }
            else
            {
                // Use current process as example
                processId = Process.GetCurrentProcess().Id;
                Console.WriteLine($"No process ID provided. Using current process ID: {processId}");
            }

            Console.WriteLine("\n📋 Method 1: Using Process.GetProcessById");
            bool isRunning1 = ParentProcessChecker.IsParentProcessRunning(processId);

            Console.WriteLine("\n📋 Method 2: Using Windows API");
            bool isRunning2 = ParentProcessChecker.IsParentProcessRunningAPI(processId);

            Console.WriteLine("\n📋 Method 3: Using WMI");
            bool isRunning3 = ParentProcessChecker.IsParentProcessRunningWMI(processId);

            Console.WriteLine("\n📋 Method 4: Getting Detailed Process Info");
            ProcessInfo processInfo = ParentProcessChecker.GetProcessInfo(processId);
            Console.WriteLine(processInfo);

            Console.WriteLine("\n📊 Summary:");
            Console.WriteLine($"GetProcessById Result: {(isRunning1 ? "RUNNING" : "NOT RUNNING")}");
            Console.WriteLine($"API Result: {(isRunning2 ? "RUNNING" : "NOT RUNNING")}");
            Console.WriteLine($"WMI Result: {(isRunning3 ? "RUNNING" : "NOT RUNNING")}");

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}