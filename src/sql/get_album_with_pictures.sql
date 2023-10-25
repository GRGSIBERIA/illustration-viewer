/* 
    アルバムにぶら下がっている画像を全件取得する 
    @input
        :target 並び替えする基準の変数名
        :orderby 昇順・降順
        :limit_num リミット
        :offset_num オフセット
*/
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
    T.item,
    T.width,
    T.height
from albums as A
    inner join informations as I on A.info_id = I.rowid
    inner join pictures as P on P.rowid = I.picture_id
    inner join thumbs as T on T.rowid = I.thumb_id
    order by :target :orderby
    LIMIT :limit_num OFFSET :offset_num;