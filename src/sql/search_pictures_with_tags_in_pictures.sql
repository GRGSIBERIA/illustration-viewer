/* 
    タグ名から全件取得する 
    @input
        :tags 検索したいタグ名
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
    TM.item,
    TM.width,
    TM.height
from informations as I
    inner join assign_info_tags as IT on IT.info_id = I.rowid
    inner join tags as T on IT.tag_id = T.rowid
    inner join pictures as P on I.picture_id = P.rowid
    inner join thumbs as TM on I.thumb_id = P.rowid
where T.name in (:tags)
    order by :target :orderby
    LIMIT :limit_num OFFSET :offset_num;

/* where句でin句を使う場合、配列を指定できないため、プレースホルダーの:tagsをその個数だけ置き換える */
/* [A, B, C] ならば replace(":tags", "'A', 'B', 'C'")