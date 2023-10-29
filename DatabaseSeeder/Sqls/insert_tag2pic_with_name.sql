insert into tag2pic(tag_id, pic_id)
select tags.id, @pic_id from tags
where tags.name = @name;