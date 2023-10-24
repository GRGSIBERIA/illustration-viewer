select
    A.name,
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
from albums as A
    inner join informations as I on A.info_id = I.rowid
    inner join pictures as P on P.rowid = I.picture_id
    inner join thumbs as T on T.rowid = I.thumb_id
    order by I.rowid ?
    LIMIT ? OFFSET ?;