/* スターボタンが押される */
update informations set is_star = 1 where is_star <= 0;
update informations set goods = goods + 1;