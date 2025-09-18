# PowerShell script to check if a parent process ID is running
# Usage: .\CheckParentProcess.ps1 -ParentProcessId <PID>

param(
    [Parameter(Mandatory=$true)]
    [int]$ParentProcessId
)

function Test-ParentProcessRunning {
    param([int]$ProcessId)
    
    try {
        # Get the process by ID
        $process = Get-Process -Id $ProcessId -ErrorAction Stop
        
        # Check if process is running and not null
        if ($process -and $process.Responding) {
            Write-Host "✅ Parent Process ID $ProcessId is RUNNING" -ForegroundColor Green
            Write-Host "Process Name: $($process.ProcessName)" -ForegroundColor Cyan
            Write-Host "Process Path: $($process.Path)" -ForegroundColor Cyan
            Write-Host "Start Time: $($process.StartTime)" -ForegroundColor Cyan
            Write-Host "CPU Time: $($process.TotalProcessorTime)" -ForegroundColor Cyan
            Write-Host "Memory Usage: $([math]::Round($process.WorkingSet64 / 1MB, 2)) MB" -ForegroundColor Cyan
            return $true
        }
        else {
            Write-Host "❌ Parent Process ID $ProcessId is NOT RESPONDING" -ForegroundColor Red
            return $false
        }
    }
    catch [System.Management.Automation.PSArgumentException] {
        Write-Host "❌ Parent Process ID $ProcessId is NOT RUNNING (Process not found)" -ForegroundColor Red
        return $false
    }
    catch {
        Write-Host "❌ Error checking process: $($_.Exception.Message)" -ForegroundColor Red
        return $false
    }
}

# Alternative method using WMI
function Test-ParentProcessRunningWMI {
    param([int]$ProcessId)
    
    try {
        $process = Get-WmiObject -Class Win32_Process -Filter "ProcessId = $ProcessId" -ErrorAction Stop
        
        if ($process) {
            Write-Host "✅ Parent Process ID $ProcessId is RUNNING (WMI)" -ForegroundColor Green
            Write-Host "Process Name: $($process.Name)" -ForegroundColor Cyan
            Write-Host "Command Line: $($process.CommandLine)" -ForegroundColor Cyan
            Write-Host "Creation Date: $($process.CreationDate)" -ForegroundColor Cyan
            return $true
        }
        else {
            Write-Host "❌ Parent Process ID $ProcessId is NOT RUNNING (WMI)" -ForegroundColor Red
            return $false
        }
    }
    catch {
        Write-Host "❌ WMI Error: $($_.Exception.Message)" -ForegroundColor Red
        return $false
    }
}

# Main execution
Write-Host "🔍 Checking if Parent Process ID $ParentProcessId is running..." -ForegroundColor Yellow
Write-Host "=" * 50 -ForegroundColor Yellow

# Method 1: Using Get-Process
Write-Host "`n📋 Method 1: Using Get-Process" -ForegroundColor Magenta
$isRunning1 = Test-ParentProcessRunning -ProcessId $ParentProcessId

# Method 2: Using WMI
Write-Host "`n📋 Method 2: Using WMI" -ForegroundColor Magenta
$isRunning2 = Test-ParentProcessRunningWMI -ProcessId $ParentProcessId

# Summary
Write-Host "`n📊 Summary:" -ForegroundColor Yellow
Write-Host "Get-Process Result: $(if($isRunning1) {'RUNNING'} else {'NOT RUNNING'})" -ForegroundColor $(if($isRunning1) {'Green'} else {'Red'})
Write-Host "WMI Result: $(if($isRunning2) {'RUNNING'} else {'NOT RUNNING'})" -ForegroundColor $(if($isRunning2) {'Green'} else {'Red'})

# Return boolean result
return $isRunning1