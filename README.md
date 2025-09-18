# Parent Process ID Checker

This repository contains both PowerShell and C# solutions to check if a parent process ID is running in the task manager.

## Files

- `CheckParentProcess.ps1` - PowerShell script
- `CheckParentProcess.cs` - C# console application
- `CheckParentProcess.csproj` - C# project file

## PowerShell Script Usage

### Basic Usage
```powershell
# Check if a specific process ID is running
.\CheckParentProcess.ps1 -ParentProcessId 1234

# Example with a real process ID
.\CheckParentProcess.ps1 -ParentProcessId 1234
```

### Features
- ✅ Two methods: Get-Process and WMI
- ✅ Detailed process information display
- ✅ Color-coded output
- ✅ Error handling
- ✅ Process status, name, path, start time, CPU time, and memory usage

### Example Output
```
🔍 Checking if Parent Process ID 1234 is running...
==================================================

📋 Method 1: Using Get-Process
✅ Parent Process ID 1234 is RUNNING
Process Name: notepad
Process Path: C:\Windows\System32\notepad.exe
Start Time: 12/15/2023 10:30:45 AM
CPU Time: 00:00:01.2345678
Memory Usage: 15.67 MB

📋 Method 2: Using WMI
✅ Parent Process ID 1234 is RUNNING (WMI)
Process Name: notepad.exe
Command Line: "C:\Windows\System32\notepad.exe"
Creation Date: 20231215103045.123456+000

📊 Summary:
Get-Process Result: RUNNING
WMI Result: RUNNING
```

## C# Application Usage

### Building the Application
```bash
# Restore packages
dotnet restore

# Build the application
dotnet build

# Run the application
dotnet run [processId]
```

### Command Line Usage
```bash
# Check current process (if no ID provided)
dotnet run

# Check specific process ID
dotnet run 1234

# Build and run executable
dotnet build -c Release
./bin/Release/net6.0/CheckParentProcess.exe 1234
```

### Features
- ✅ Three methods: Process.GetProcessById, Windows API, and WMI
- ✅ Detailed process information
- ✅ ProcessInfo class for structured data
- ✅ Comprehensive error handling
- ✅ Cross-platform compatibility (with platform-specific features)

### Example Output
```
🔍 Parent Process ID Checker
==================================================
Checking Process ID: 1234

📋 Method 1: Using Process.GetProcessById
✅ Parent Process ID 1234 is RUNNING
Process Name: notepad
Process Path: C:\Windows\System32\notepad.exe
Start Time: 12/15/2023 10:30:45 AM
CPU Time: 00:00:01.2345678
Memory Usage: 15.67 MB

📋 Method 2: Using Windows API
✅ Parent Process ID 1234 is RUNNING (API)

📋 Method 3: Using WMI
✅ Parent Process ID 1234 is RUNNING (WMI)
Process Name: notepad.exe
Command Line: "C:\Windows\System32\notepad.exe"
Creation Date: 20231215103045.123456+000

📋 Method 4: Getting Detailed Process Info
Process: notepad (ID: 1234)
Path: C:\Windows\System32\notepad.exe
Start Time: 12/15/2023 10:30:45 AM
CPU Time: 00:00:01.2345678
Memory: 15.67 MB

📊 Summary:
GetProcessById Result: RUNNING
API Result: RUNNING
WMI Result: RUNNING
```

## Testing Scenarios

### Test with Running Process
1. Open Task Manager (Ctrl+Shift+Esc)
2. Note a process ID from the Details tab
3. Run the script/application with that ID

### Test with Non-Existent Process
```powershell
# PowerShell
.\CheckParentProcess.ps1 -ParentProcessId 99999
```

```bash
# C#
dotnet run 99999
```

### Test with Current Process
```powershell
# PowerShell - get current process ID
$currentPID = $PID
.\CheckParentProcess.ps1 -ParentProcessId $currentPID
```

```bash
# C# - automatically uses current process if no ID provided
dotnet run
```

## Error Handling

Both solutions handle common scenarios:
- ✅ Process not found (non-existent PID)
- ✅ Process has exited
- ✅ Access denied (insufficient permissions)
- ✅ System errors

## Performance Considerations

- **PowerShell**: Get-Process is faster than WMI for basic checks
- **C#**: Process.GetProcessById is fastest, followed by Windows API, then WMI
- **WMI**: Most comprehensive but slowest method

## Platform Compatibility

- **PowerShell**: Windows only (uses Windows-specific cmdlets)
- **C#**: Cross-platform (.NET 6+), but WMI and Windows API features are Windows-specific

## Dependencies

### PowerShell
- Windows PowerShell 5.1+ or PowerShell Core 6+
- No additional modules required

### C#
- .NET 6.0 or later
- System.Management NuGet package (for WMI support)

## Use Cases

1. **Process Monitoring**: Check if parent processes are still running
2. **Service Dependencies**: Verify required services are active
3. **Automation Scripts**: Ensure processes haven't crashed
4. **Debugging**: Troubleshoot process-related issues
5. **System Administration**: Monitor critical system processes