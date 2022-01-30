# 快速开服（FastDeploy）
插件目前只做两件事情
1、自动为普通用户（default组）添加一些必要的权限！
2、创建一个GM用户组（GameManager），默认的超管组（SuperAdmin组）会跳过SSC（强制开荒），GM组则不会，除此之外拥有超管组的所有权限。


给普通用户组添加的权限：
```json
"tshock.tp.tppotion",                   // 允许使用 传送药水
"tshock.tp.magicconch",             // 魔法海螺
"tshock.tp.demonconch",           // 恶魔海螺
"tshock.tp.rod",                                 // 混沌传送法杖

"tshock.world.toggleparty",             // 开派对
"tshock.world.time.usesundial",     // 使用附魔日晷
"tshock.world.editspawn",			 // 设置出生点
"tshock.world.movenpc",			    // 为NPC分配房屋

"tshock.npc.hurttown",				// 伤害NPC
"tshock.npc.startinvasion",		  // 召唤入侵
"tshock.npc.startdd2",				// 天国事件
"tshock.npc.spawnpets",			 // 生成城镇宠物

"tshock.ignore.removetile",         // 忽略 乱挖砖块(使用炸弹等) 检测
"tshock.ignore.liquid",                // 忽略 液体 检测
"tshock.ignore.noclip",               // 忽略 穿墙 检测
"tshock.ignore.paint",  			  // 忽略 油漆 检测
"tshock.ignore.placetile",           // 忽略 替换方块 检测
"tshock.ignore.projectile",         // 忽略 射弹 检测
"tshock.ignore.damage",           // 忽略 高伤害 检测
"tshock.ignore.sendtilesquare",	// 允许 锤击改变飞镖机关方向
```

将服主加入到GM组：
```json
/user group <玩家名> GM

// 例如,hf是服主，注册并登录后，在控制台上执行这行指令
/user group hf GM
```