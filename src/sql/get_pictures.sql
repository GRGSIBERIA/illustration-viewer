/* 
    画像を全件取得する。メモリに載るはずがないので非推奨。
    @input
        :target 並び替えする基準の変数名
        :orderby 昇順・降順
        :limit_num リミット
        :offset_num オフセット
*/
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