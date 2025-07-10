dotnet-coverage collect -f xml -o coverage.xml dotnet test StatusNamaa.sln
reportgenerator -reports:coverage.xml -targetdir:.\report -assemblyfilters:+StatusNamaa.dll
.\report\index.html