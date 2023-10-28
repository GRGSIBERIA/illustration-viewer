/*
    指定した親タグ名から子タグを検索する
    @input
        :name 親タグ名
        :target 並び替え対象のカラム名
        :orderby 昇順・降順
*/
select
    C.rowid,
    C.name,
    C.parent_id
from tags as C
    inner join tags as P on A.parent_id = P.rowid
where P.name = :name
    order by :target :orderby;