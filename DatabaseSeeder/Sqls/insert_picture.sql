insert into pictures(picture, thumbnail, sha1, ext, width, height, import_path, created_at, saved_at)
	select @picture, @thumbnail, @sha1, @ext, @width, @height, @import_path, @created_at, @saved_at
	from pictures where not exists (
		select 1 from pictures
		where sha1 = @sha1
	);