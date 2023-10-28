pragma foreign_keys = ON;

/* 画像 */
create table if not exists pictures (
	id integer primary key autoincrement not null,
	picture blob not null,
	thumbnail blob not null,
	sha1 text not null,
	width integer not null,
	height integer not null,
	import_path text not null,
	saved_at text not null default (DATETIME('now', 'localtime')),
	created_at text not null
);
create unique index if not exists pictures_idpk_idx on pictures(id);
create index if not exists pictures_savedid_idx on pictures(id, saved_at);
create index if not exists pictures_createdid_idx on pictures(id, created_at);
create index if not exists pictures_sha1_idx on pictures(sha1);


/* タグ */
create table if not exists tags (
	id integer primary key autoincrement not null,
	name text not null unique,
	parent_id integer default NULL references tags(id)
		on delete cascade on update cascade
);
create unique index if not exists tags_idname_idx on tags(id, name);
create unique index if not exists tags_idparent_idx on tags(id, parent_id);


/* アルバム */
create table if not exists albums (
	id integer primary key autoincrement not null,
	name text not null unique
);
create unique index if not exists albums_idname_idx on albums(id, name);


/* コレクション */
create table if not exists collections (
	id integer primary key autoincrement not null,
	name text not null unique
);
create unique index if not exists collections_idname_idx on collections(id, name);


/* タグと画像のアサイン */
create table if not exists tag2pic (
	tag_id integer not null references tags(id),
	pic_id integer not null references pictures(id)
);
create unique index if not exists tag2pic_tag_pic_idx on tag2pic(tag_id, pic_id);


/* アルバムと画像のアサイン */
create table if not exists album2pic (
	album_id integer not null references albums(id)
		on delete cascade on update cascade,
	pic_id integer not null references pictures(id)
		on delete cascade on update cascade
);
create unique index if not exists album2pic_album_pic_idx on album2pic(album_id, pic_id);


/* コレクションと画像のアサイン */
create table if not exists col2pic (
	col_id integer not null references collections(id)
		on delete cascade on update cascade,
	pic_id integer not null references pictures(id)
		on delete cascade on update cascade
);
create unique index if not exists col2pic_col_pic_idx on col2pic(col_id, pic_id);
