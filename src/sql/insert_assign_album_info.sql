/*
    1つのアルバムに1つの画像を登録する
    @input
        :album_name アルバム名
        :info_id 画像ID
*/
insert into assign_album_info(album_id, info_id)
    select A.rowid, :info_id from albums as A
    where A.name = :album_name;