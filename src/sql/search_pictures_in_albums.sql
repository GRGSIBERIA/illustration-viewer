/* 
    アルバムにぶら下がっている画像を全件取得する 
    @input
        :album_name 検索したいアルバム名
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
    TM.item,
    TM.width,
    TM.height
from informations as I
    inner join pictures as P on P.rowid = I.picture_id
    inner join thumbs as T on T.rowid = I.thumb_id
    inner join assign_album_info as AAI on I.rowid = AAI.info_id
    inner join albums as A on A.rowid = AAI.album_id
    where A.name = :album_name
    order by :target :orderby
    limit :limit_num offset :offset_num;