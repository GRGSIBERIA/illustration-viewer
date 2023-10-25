/*
    アルバムから画像を削除する
    @input
        info_id アルバムから削除したい情報ID
*/
delete from assign_album_info as AAI
    where AAI.info_id = :info_id;