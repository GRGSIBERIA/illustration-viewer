/* 
    アルバムにぶら下がっている画像を全件取得する 
    @input
        :album_name 検索したいアルバム名
        :target 並び替えする基準の変数名
        :is_star スターの有無
        :goods グッド数
        :tags タグの集合
        :orderby 昇順・降順
        :limit_num リミット
        :offset_num オフセット
*/

select 
    A.name,
    distinct I.rowid,
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
    inner join assign_info_tags as AIT on I.rowid = AIT.info_id
    inner join albums as A on A.rowid = AAI.album_id
    inner join tags as TAG on AIT.tag_id = TAG.rowid
    where
        A.name = :album_name AND
        I.is_star >= :is_star AND
        I.goods >= :goods AND
        TAG.name in (:tags)
    order by :target :orderby
    limit :limit_num offset :offset_num;