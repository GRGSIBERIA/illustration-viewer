/* 
    アルバム名を全件取得する
    @input
        :target 並び替え対象のカラム名
        :orderby 昇順・降順
*/


select A.rowid, A.name from albums as A 
    order by :target :orderby;