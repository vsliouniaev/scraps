function RunProcessAsUser{
    [CmdletBinding()]Param(
        [parameter(Mandatory=$true)]
        [PSCredential]
        $credential,

        [parameter(Mandatory=$true)]
        [string]
        $arguments
    )

    $pinfo = New-Object System.Diagnostics.ProcessStartInfo
    $pinfo.UserName = $credential.UserName
    $pinfo.Password = $credential.Password
    $pinfo.FileName = "powershell.exe"
    $pinfo.RedirectStandardError = $true
    $pinfo.RedirectStandardOutput = $true
    $pinfo.UseShellExecute = $false
    $pinfo.LoadUserProfile = $true
    $pinfo.Arguments = $arguments
    $pinfo.CreateNoWindow = $true
    $p = New-Object System.Diagnostics.Process
    $p.StartInfo = $pinfo
    $p.Start() | Out-Null
    $stdout = $p.StandardOutput.ReadToEnd()
    $stderr = $p.StandardError.ReadToEnd()
    $p.WaitForExit()

    if($p.ExitCode -ne 0){
        throw $stderr
    } else {
        return $stdout
    }
}
