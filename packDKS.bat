D:
rd /q /s D:\shc_workspace\Publish\CMS-API
rd /q /s D:\shc_workspace\Publish\CMS-SPA
cd D:\shc_workspace\CMS\CMS-API
dotnet publish -o ..\..\Publish\CMS-API
cd ..\CMS-SPA
ng build --prod --build-optimizer=false
