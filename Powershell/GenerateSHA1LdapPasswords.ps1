$infile = 'C:\dev\ldapsource.ldif'
$outfile = 'C:\dev\ldapchanged.ldif'
$passwordsFile = 'C:\dev\passwords.csv'
Remove-Item $passwordsFile -ErrorAction Continue
Remove-Item $outfile -ErrorAction Continue

Add-Type -AssemblyName System.Web


Function Get-StringHash([String] $String,$HashName = "SHA1") 
{ 
    $s = [System.Security.Cryptography.HashAlgorithm]::Create($HashName).ComputeHash([System.Text.Encoding]::UTF8.GetBytes($String))
    return [System.Convert]::ToBase64String($s)
}


$user = "";
$password = "";
$passwordsContent = ""
$outContent = ""

Get-Content $infile | %{
    if($_ -match "^dn"){
        $_ -match "(?<=uid=)[^,]+" | Out-Null
        $user = $matches[0]
    }

    if($_ -match "^userPassword"){        
        $password = [System.Web.Security.Membership]::GeneratePassword(12, 3)
        $sha = Get-StringHash $password
        $passwordsContent += "$user`t$password`r`n"
        $outContent += "userPassword: {SHA}$sha`r`n"
    } else {
         $outContent += "$_`r`n"
    }
}

[System.IO.File]::WriteAllLines($outfile, $outContent, (New-Object System.Text.UTF8Encoding $False))

$passwordsContent | Out-File -FilePath $passwordsFile
