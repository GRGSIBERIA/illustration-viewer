/*
    タグ名でタグを削除する
    on delete cascadeされているので、
    assign_info_tagのレコードも削除される
    @input
        tagname タグ名
*/
delete from tags as TAG
    where TAG.name = :tagname;