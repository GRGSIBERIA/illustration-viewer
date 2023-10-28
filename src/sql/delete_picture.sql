/*
    infoを削除する。
    on delete cascadeしているから、画像も削除される
    @input
        info_id 情報ID
*/
delete from informations as I where I.rowid = :info_id;