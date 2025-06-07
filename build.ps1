Get-ChildItem -Path '.\artifacts' | Remove-Item -Force -Recurse

dotnet pack src\StatusNamaa\StatusNamaa.csproj -o artifacts