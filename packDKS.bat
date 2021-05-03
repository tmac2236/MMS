D:
rd /q /s D:\shc_workspace\Publish\MMS-API
rd /q /s D:\shc_workspace\Publish\MMS-SPA
cd D:\shc_workspace\MMS\MMS-API
dotnet publish -o ..\..\Publish\MMS-API
cd ..\MMS-SPA
ng build --prod --build-optimizer=false
