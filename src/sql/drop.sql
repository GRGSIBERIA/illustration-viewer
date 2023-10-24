drop index if exists info_created_index, info_imported_index, info_star_index, info_goods_index;
drop index if exists tags_name_unique;
drop index if exists assign_info_index, assign_tag_index;
drop index if exists albums_info_index, albums_name_index, albums_created_index;
drop table if exists pictures, thumbs, informations, tags, assign_info_tags, albums;
VACUUM;