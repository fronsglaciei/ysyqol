Param(
    [switch]$UpdateAsset
)

if ($UpdateAsset) {
    # publish asset generator
    dotnet publish .\FG.Utils.Resources\FG.Utils.Resources.csproj -p:PublishProfile=FolderProfile --force

    # update assets in mod project
    .\FG.Utils.Resources\bin\publish\genassets.exe -i .\FG.Mods.YSYard.QoL\Assets
}

dotnet build .\FG.Mods.YSYard.QoL\FG.Mods.YSYard.QoL.csproj --configuration Release --force
