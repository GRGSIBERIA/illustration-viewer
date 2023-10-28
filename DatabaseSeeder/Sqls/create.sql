pragma foreign_keys = ON;

/* 画像 */
create table if not exists pictures (
	picture blob not null,
	thumbnail blob not null,
	sha1 text not null
	width integer not null,
	height integer not null,
	import_path text not null,
	saved_at text not null default (DATETIME('now', 'localtime')),
	created_at text not null
);
create unique index if not exists pictures_idpk_idx on pictures(rowid);
create index if not exists pictures_savedid_idx on pictures(rowid, saved_at);
create index if not exists pictures_createdid_idx on pictures(rowid, created_at);
create index if not exists pictures_sha1_idx on pictures(sha1);


/* タグ */
create table if not exists tags (
	name text not null unique,
	parent_id integer default NULL references tags(id)
		on delete cascade on update cascade
);
create unique index if not exists tags_idname_idx on tags(rowid, name);
create unique index if not exists tags_idparent_idx on tags(rowid, parent_id);


/* アルバム */
create table if not exists albums (
	name text not null unique
);
create unique index if not exists albums_idname_idx on albums(rowid, name);


/* コレクション */
create table if not exists collections (
	name text not null unique
);
create unique index if not exists collections_idname_idx on collections(rowid, name);


/* タグと画像のアサイン */
create table if not exists tag2pic (
	tag_id integer not null references tags(rowid),
	pic_id integer not null references pictures(rowid)
);
create unique index if not exists tag2pic_tag_pic_idx on tag2pic(tag_id, pic_id);


/* アルバムと画像のアサイン */
create table if not exists album2pic (
	album_id integer not null references albums(rowid)
		on delete cascade on update cascade,
	pic_id integer not null references pictures(rowid)
		on delete cascade on update cascade
);
create unique index if not exists album2pic_album_pic_idx on album2pic(album_id, pic_id);


/* コレクションと画像のアサイン */
create table if not exists col2pic (
	col_id integer not null references collections(rowid)
		on delete cascade on update cascade,
	pic_id integer not null references pictures(rowid)
		on delete cascade on update cascade
);
create unique index if not exists col2pic_col_pic_idx on col2pic(col_id, pic_id);