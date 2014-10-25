if not exist "./NuGet Packages" mkdir "./NuGet Packages"
set version=4.0.1.4000
call ".nuget/NuGet.exe" pack NHibernate.Spatial.MsSql\NHibernate.Spatial.MsSql.csproj -Version %version% -Build -IncludeReferencedProjects -Properties Configuration=Release -OutputDirectory "./NuGet Packages"
call ".nuget/NuGet.exe" pack NHibernate.Spatial.MySQL\NHibernate.Spatial.MySQL.csproj -Version %version% -Build -IncludeReferencedProjects -Properties Configuration=Release -OutputDirectory "./NuGet Packages"
call ".nuget/NuGet.exe" pack NHibernate.Spatial.PostGis\NHibernate.Spatial.PostGis.csproj -Version %version% -Build -IncludeReferencedProjects -Properties Configuration=Release -OutputDirectory "./NuGet Packages"
