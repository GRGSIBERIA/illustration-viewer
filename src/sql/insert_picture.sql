BEGIN IMMEDIATE;
insert into pictures values (:picitem, :picsha1, :picwidth, :picheight);
insert into thumbs values (:thumitem, :thumsha1, :thumwidth, :thumheight);
insert into informations(picture_id, thumb_id) values (
    last_insert_rowid(),
    last_insert_rowid()
);
COMMIT;