/*
    タグにぶら下がっている画像だけ除外する
    @input
        info_id タグから除外したい情報ID
        tag_id 除外するタグID
*/
delete from assign_info_tag as AIT
    where AIT.info_id = :info_id and AIT.tag_id = :tag_id;