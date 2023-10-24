SELECT
    rowid,
    name,
    parent_id
FROM tags
    order by :target :orderby;