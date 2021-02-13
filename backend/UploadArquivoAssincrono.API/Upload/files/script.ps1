$r=@()
$t=$C=1
try {
Import-Excel -Path C:\estudos-angular\TESTE\guid.xlsx|Foreach-Object -Process {
    #Append rows in an array
    $r += $_

    #Save in a new excel when count reaches 3
    if($C -eq 2000){
        $r | Export-Excel -Path C:\estudos-angular\TESTE\test_$t.xlsx

        #reset values
        $r=@()
        $c=1
       $t++
    }
    else{
        #increment row count
        $c++
    }
}

#save remaining rows
$r|Export-Excel -Path C:\estudos-angular\TESTE\test_$t.xlsx
}
catch
{
	Write-Error $_.Exception.ToString()
    Read-Host -Prompt "The above error occurred. Press Enter to exit."
}