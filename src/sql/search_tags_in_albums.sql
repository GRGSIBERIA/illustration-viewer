select 
    A.name,
    I.rowid,
    I.created_at,
    I.imported_at,
    I.is_star,
    I.goods,
    I.imported_path,
    P.item,
    P.width,
    P.height,
    TM.item,
    TM.width,
    TM.height
from informations as I, tags as TAG
    inner join albums as A on A.info_id = I.rowid
    inner join pictures as P on P.rowid = I.picture_id
    inner join thumbs as T on T.rowid = I.thumb_id
    inner join assign_info_tags as AIT on AIT.info_id = I.rowid 
    inner join tags as TAG on AIT.tag_id = TAG.rowid
where TAG.name in (???) and A.name = :album_name
    order by :target :orderby
    LIMIT :limit_num OFFSET :offset_num;