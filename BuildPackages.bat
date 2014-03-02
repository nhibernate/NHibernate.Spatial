if not exist "./NuGet Packages" mkdir "./NuGet Packages"
call ".nuget/NuGet.exe" pack NHibernate.Spatial.MsSql2008\NHibernate.Spatial.MsSql2008.csproj -Build -IncludeReferencedProjects -Properties Configuration=Release -OutputDirectory "./NuGet Packages"
call ".nuget/NuGet.exe" pack NHibernate.Spatial.MySQL\NHibernate.Spatial.MySQL.csproj -Build -IncludeReferencedProjects -Properties Configuration=Release -OutputDirectory "./NuGet Packages"
call ".nuget/NuGet.exe" pack NHibernate.Spatial.PostGis\NHibernate.Spatial.PostGis.csproj -Build -IncludeReferencedProjects -Properties Configuration=Release -OutputDirectory "./NuGet Packages"
