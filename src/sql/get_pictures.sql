select 
    I.rowid,
    I.created_at,
    I.imported_at,
    I.is_star,
    I.goods,
    I.imported_path,
    P.item,
    P.width,
    P.height,
    T.item,
    T.width,
    T.height
from informations as I
    inner join pictures as P on I.picture_id = P.rowid
    inner join thumbs as T on I.thumb_id = T.rowid
    order by :target :orderby
    LIMIT :limit_num OFFSET offset_num;