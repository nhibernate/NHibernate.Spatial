if not exist "./NuGet Packages" mkdir "./NuGet Packages"
set version=3.3.3-pre
call ".nuget/NuGet.exe" pack NHibernate.Spatial.MsSql2008\NHibernate.Spatial.MsSql2008.csproj -Version %version% -Build -IncludeReferencedProjects -Properties Configuration=Release -OutputDirectory "./NuGet Packages"
call ".nuget/NuGet.exe" pack NHibernate.Spatial.MySQL\NHibernate.Spatial.MySQL.csproj -Version %version% -Build -IncludeReferencedProjects -Properties Configuration=Release -OutputDirectory "./NuGet Packages"
call ".nuget/NuGet.exe" pack NHibernate.Spatial.PostGis\NHibernate.Spatial.PostGis.csproj -Version %version% -Build -IncludeReferencedProjects -Properties Configuration=Release -OutputDirectory "./NuGet Packages"
