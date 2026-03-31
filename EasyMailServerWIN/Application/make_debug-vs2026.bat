path %path%;C:\Program Files\Microsoft Visual Studio\18\Community\Common7\IDE

                                                                                 
devenv.exe ..\ServerAPI\API\ServerAPI.csproj /rebuild "Debug"
devenv.exe ..\ServerAPI\API_XML\xml_API.csproj /rebuild "Debug"
devenv.exe ..\MailServer\lsMailServer.csproj /rebuild "Debug"
devenv.exe ..\MailServerService\MailServerService.csproj /rebuild "Debug"

devenv.exe ..\MailServerConfiguration\MailServerConfiguration.csproj /rebuild "Debug"
devenv.exe ..\ServerAPI\UserAPI\UserAPI\UserAPI.csproj /rebuild "Debug"

devenv.exe ..\MailServerManager\MailServerManager.csproj /rebuild "Debug"

devenv.exe ..\Filters\lsDNSBL\lsDNSBL_Filter.csproj /rebuild "Debug"
devenv.exe ..\Filters\lsVirusFilter\lsVirusFilter.csproj /rebuild "Debug"

copy ..\version.txt Debug\version.txt

pause
 


