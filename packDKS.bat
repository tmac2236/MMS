D:
rd /q /s D:\shc_workspace\Publish\API
rd /q /s D:\shc_workspace\Publish\SPA
cd D:\shc_workspace\CMS\API
dotnet publish -o ..\..\Publish\API
cd ..\SPA
ng build --prod --build-optimizer=false
