/*
    タグ名でタグを削除する
    @input
        tagname タグ名
*/
delete from tags as TAG
    where TAG.name = :tagname;