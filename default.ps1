properties {
    $base_dir = . resolve-path .\
    $Configuration = "Release"
}

task default -depends build

task SetChocolateyPath {
    $script:chocolateyDir = $null
    if ($env:ChocolateyInstall -ne $null) {
        $script:chocolateyDir = $env:ChocolateyInstall;
    } elseif (Test-Path (Join-Path $env:SYSTEMDRIVE Chocolatey)) {
        $script:chocolateyDir = Join-Path $env:SYSTEMDRIVE Chocolatey;
    } elseif (Test-Path (Join-Path ([Environment]::GetFolderPath("CommonApplicationData")) Chocolatey)) {
        $script:chocolateyDir = Join-Path ([Environment]::GetFolderPath("CommonApplicationData")) Chocolatey;
    }

    Write-Output "Chocolatey installed at $script:chocolateyDir";
}

task GitVersion -depends SetChocolateyPath {
    $chocolateyBinDir = Join-Path $script:chocolateyDir -ChildPath "bin";
    $gitVersionExe = Join-Path $chocolateyBinDir -ChildPath "GitVersion.exe";

    & $gitVersionExe /output buildserver /updateassemblyinfo
    
    $nugetVersion = & $gitVersionExe /output json /showvariable NuGetVersionV2
    echo "nuget version set to $nugetVersion"
    
    Get-ChildItem -r project.json | % {
        cd $_.Directory
        dotnet version $nugetVersion 
    }
}

task restore {
    dotnet restore
}

task build {
    dotnet build $base_dir/src/csMACnz.ConcurrentConsole $base_dir/src/csMACnz.ConcurrentConsole.Sample -c $Configuration
}

task test {
    dotnet test tests/csMACnz.ConcurrentConsole.Tests
}

task pack {
    $package_dir = "$base_dir\pack"
    if (Test-Path $package_dir) {
        Remove-Item $package_dir -r
    }
    mkdir $package_dir

    dotnet pack $base_dir/src/csMACnz.ConcurrentConsole -c $Configuration -o $package_dir
}

task watch-build {
    cd $base_dir/src/csMACnz.ConcurrentConsole.Sample
    dotnet watch build
}

task watch-test {
    cd $base_dir/tests/csMACnz.ConcurrentConsole.Tests
    dotnet watch test
}

task sample {
    dotnet run -p $base_dir/src/csMACnz.ConcurrentConsole.Sample
}

task sample-net45 {
    dotnet run -p $base_dir/src/csMACnz.ConcurrentConsole.Sample -f net45
}

task simple-sample {
    dotnet run -p $base_dir/src/csMACnz.ConcurrentConsole.Sample simple
}

task appveyor-install -depends restore, GitVersion

task appveyor-build -depends build

task appveyor-test -depends test, pack
