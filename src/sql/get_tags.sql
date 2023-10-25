SELECT
    distinct TAG.rowid,
    TAG.name,
    TAG.parent_id,
    TAG.created_at,
    count(AIT.tag_id) as item_count
from tags as TAG
    inner join assign_info_tags as AIT on TAG.rowid = AIT.tag_id
    order by :target :orderby;