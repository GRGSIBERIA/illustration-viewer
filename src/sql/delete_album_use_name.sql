/*
    アルバム名でアルバムを削除する
    on delete cascadeされているので、
    assign_album_infoのレコードも削除される
    @input
        album_name アルバム名
*/
delete from albums as A
    where A.name = :album_name;