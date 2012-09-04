The namespace "Tests.NHibernate.Spatial" is used instead of
"NHibernate.Spatial.Tests" because the latter interfers with 
the name resolution of NHibernate.Expression.Expression class
without full namespace qualification [eg. .Add(Expression.Eq(...]
