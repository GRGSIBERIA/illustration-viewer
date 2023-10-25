SELECT
    rowid,
    name,
    parent_id
from tags
    order by :target :orderby;