insert into tags(name)
select name from tags where not exists
	(select 1 from tags where name = @name);