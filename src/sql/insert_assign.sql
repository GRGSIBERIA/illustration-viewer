/*
    アサインを追加する。
    タグ名の存在を確認しないため、事前にタグを登録しなければならない。
    大量に存在する場合はアプリケーション側でトランザクションをかける。
    @input
        :infoid 情報ID
        :tagname タグ名
*/
insert into assign_info_tags(info_id, tag_id)
    select :infoid, T.id from tags as T
    where :tagname = T.name;