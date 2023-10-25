/*
    1件のタグを保存する
    @input
        tag タグ名
*/

insert into tags(name)
    select :tag from
    where not exists (
        select 1 from tags
        where tags.name = :tag
    );  /* タグが存在しなかったら追加する */