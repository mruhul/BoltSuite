@echo off
echo Start running tests before pushing the package...
dotnet test || goto:error
echo All tests passed. Start creating package...
mkdir nupkgs
cd nupkgs
del /F /Q *.*
cd ..
set /p project="Enter Project: "
cd %project%
dotnet restore
dotnet build -c Release
dotnet pack -c Release --no-build --output ../nupkgs
set /p key="Enter Key: "
cd ../nupkgs
dotnet nuget push *.nupkg -s https://www.nuget.org/api/v2/package/ -k %key% || goto:error
echo "Publish completed Successfully"
cd ..
goto:completed
:error
echo "Process failed"
:completed
