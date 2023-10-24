select
    A.rowid,
    A.name,
    A.parent_id
from tags as A
    inner join tags as B on A.parent_id = B.rowid;