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
    TM.item,
    TM.width,
    TM.height
from informations as I
    inner join assign_info_tags as IT on IT.info_id = I.rowid
    inner join tags as T on IT.tag_id = T.rowid
    inner join pictures as P on I.picture_id = P.rowid
    inner join thumbs as TM on I.thumb_id = P.rowid
where T.name in (???)
    order by :target :orderby
    LIMIT :limit_num OFFSET :offset_num;

/* where句でin句を使う場合、配列を指定できないため、プレースホルダーの?をその個数だけ置き換える */
/* [A, B, C] ならば replace("???", "?, ?, ?")