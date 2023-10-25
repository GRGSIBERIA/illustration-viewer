/*
    画像を1件保存する
    @input
        picitem 画像
        picsha1 画像のSHA1
        picwidth 画像の幅
        picheight 画像の高さ
        thumitem サムネ
        thumsha1 サムネのSHA1
        thumwidth サムネの幅
        thumheight サムネの高さ
*/

BEGIN IMMEDIATE;
insert into pictures values (:picitem, :picsha1, :picwidth, :picheight);
insert into thumbs values (:thumitem, :thumsha1, :thumwidth, :thumheight);
insert into informations(picture_id, thumb_id) values (
    last_insert_rowid(),
    last_insert_rowid()
);
COMMIT;