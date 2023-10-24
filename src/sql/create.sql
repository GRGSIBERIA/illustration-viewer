pragma foreign_keys = ON;

/* 画像のデータ */
create table if not exists pictures(
    item blob NOT NULL UNIQUE,
    sha1 text NOT NULL UNIQUE
    width integer NOT NULL,
    height integer NOT NULL
);

/* サムネイルのデータ */
create table if not exists thumbs(
    item blob NOT NULL UNIQUE,
    sha1 text NOT NULL UNIQUE,
    width integer NOT NULL,
    height integer NOT NULL
);

/* 情報 */
create table if not exists informations(
    picture_id integer NOT NULL UNIQUE
        references pictures(rowid)
        on delete cascade, on update cascade,
    thumb_id integer NOT NULL UNIQUE
        references thumbs(rowid)
        on delete cascade, on update cascade,
    created_at text NOT NULL default (DATETIME('now', 'localtime')),
    imported_at text NOT NULL default (DATETIME('now', 'localtime')),
    is_star integer NOT NULL DEFAULT 0,
    goods integer NOT NULL DEFAULT 0,
    imported_path text NOT NULL
);
create index if not exists info_created_index on informations(created_at);
create index if not exists info_imported_index on informations(imported_at);
create index if not exists info_star_index on informations(is_star);
create index if not exists info_goods_index on informations(goods);

/* タグ */
create table if not exists tags(
    name text NOT NULL UNIQUE
);
create unique index if not exists tags_name_unique on tags(name);

/* 情報とタグの紐づけ */
create table if not exists assign_info_tags(
    info_id integer NOT NULL 
        REFERENCES informations(rowid)
        on delete cascade, on update cascade,
    tag_id integer NOT NULL 
        REFERENCES tags(rowid)
        on delete cascade, on update cascade,
    unique(info_id, tag_id)
);
create index if not exists assign_info_index
    on assign_info_index on assign_info_tags(info_id);
create index if not exists assign_tag_index
    on assign_tag_index on assign_info_tags(tag_id);

/* アルバム */
create table if not exists albums(
    name text NOT NULL UNIQUE,
    info_id integer NOT NULL
        REFERENCES informations(rowid)
        on delete cascade, on update cascade,
    created_at text NOT NULL default (DATETIME('now', 'localtime'))
);
create index if not exists albums_info_index on albums(info_id);
create unique index if not exists albums_name_index on albums(name);
create index if not exists albums_created_index on albums(created_at);
