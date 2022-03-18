using System.Collections.Generic;


namespace Plugin
{
    public class ConfigHelper{

        public static List<string> GetGMPerms()
        {
            // https://tshock.readme.io/docs/server-side-character-config
           return new List<string>(){
                "*",
                "!tshock.ignore.ssc"
           };
        }

        public static List<string> GetJourneyPerms()
        {
           return new List<string>(){
                "tshock.journey.research",      // 物品复制

                "tshock.journey.time.freeze",   //时间冻结
                "tshock.journey.time.set",      // 时间调节
                "tshock.journey.time.setspeed", //时间速度
                
                "tshock.journey.wind.strength", //风强度
                "tshock.journey.wind.freeze",       //风控制
                
                "tshock.journey.rain.strength", // 雨强度
                "tshock.journey.rain.freeze",   //雨改变
                
                "tshock.journey.godmode",   // 无敌模式
                "tshock.journey.placementrange",    // 扩大放置范围
                "tshock.journey.setspawnrate",          // 敌人生成速度
                "tshock.journey.biomespreadfreeze", //生态蔓延
                "tshock.journey.setdifficulty"              // 游戏难度
           };
            // List<string> perms = TShockAPI.Handlers.NetModules.CreativePowerHandler.PermissionToDescriptionMap.Keys.ToList();
            // if( !perms.Contains(Permissions.journey_contributeresearch) ){
            //     perms.Add(Permissions.journey_contributeresearch);
            // }
        }

        public static List<Topic> TopicJourney = new List<Topic>{
            Topic.GetTopic("jresearch",          "tshock.journey.research",                "物品复制"),
            Topic.GetTopic("jtime",                 "tshock.journey.time.set",                 "时间调节"),
            Topic.GetTopic("jtimefreeze",       "tshock.journey.time.freeze",           "时间冻结"),
            Topic.GetTopic("jtimespeed",       "tshock.journey.time.setspeed",       "时间流速"),
            Topic.GetTopic("jwind",                 "tshock.journey.wind.strength",        "风强度"),
            Topic.GetTopic("jwindfreeze",      "tshock.journey.wind.freeze",            "风控制"),
            Topic.GetTopic("jrain",                  "tshock.journey.rain.strength",          "雨强度"),
            Topic.GetTopic("jrainfreeze",        "tshock.journey.rain.freeze",             "雨改变"),
            Topic.GetTopic("jgod",                  "tshock.journey.godmode",               "无敌模式"),
            Topic.GetTopic("jplace",               "tshock.journey.placementrange",     "扩大放置范围"),
            Topic.GetTopic("jspawn",             "tshock.journey.setspawnrate",          "敌人生成速度"),
            Topic.GetTopic("jbiome",             "tshock.journey.biomespreadfreeze", "生态蔓延"),
            Topic.GetTopic("jdifficulty",         "tshock.journey.setdifficulty",              "游戏难度调节")
        };

        public static List<Topic> TopicIgnore = new List<Topic>{
            Topic.GetTopic("igremove",       "tshock.ignore.removetile",             "忽略 乱挖方块检测"),
            Topic.GetTopic("igplace",          "tshock.ignore.placetile",                 "忽略 替换方块检测"),
            Topic.GetTopic("igsquare",       "tshock.ignore.sendtilesquare",       "允许 锤击改变飞镖机关方向"),
            Topic.GetTopic("igliquid",         "tshock.ignore.liquid",                     "忽略 液体检测"),
            Topic.GetTopic("igpaint",          "tshock.ignore.paint",                      "忽略 油漆检测"),
            Topic.GetTopic("igprojectile",   "tshock.ignore.projectile",               "忽略 射弹检测"),
            Topic.GetTopic("igstack",          "tshock.ignore.itemstack",              "不检测 物品堆叠上限"),
            Topic.GetTopic("igban",            "tshock.ignore.dropbanneditem",  "允许掉落ban掉的物品"),
            Topic.GetTopic("igdamage",     "tshock.ignore.damage",                "不检测 超高伤害"),
            Topic.GetTopic("ighp",              "tshock.ignore.hp",                         "不检测 生命上限"),
            Topic.GetTopic("igmp",             "tshock.ignore.mp",                        "不检测 魔力上限"),
            Topic.GetTopic("igssc",             "tshock.ignore.ssc",                         "忽略 SSC强制开荒")
        };
        public static List<Topic> TopicTP = new List<Topic>{
            Topic.GetTopic("tp",                 "tshock.tp.self",              "允许使用 /tp 指令"),
            Topic.GetTopic("tphere",          "tshock.tp.others",          "允许使用 /tphere 指令"),
            Topic.GetTopic("tpall",              "tshock.tp.allothers",      "允许传送全部玩家（/tp * <player> 和 /tphere *）"),
            Topic.GetTopic("tpnpc",            "tshock.tp.npc",              "允许使用 /tpnpc 指令"),
            Topic.GetTopic("pos",               "tshock.tp.getpos",         "允许使用 /pos 指令"),
            Topic.GetTopic("tppos",            "tshock.tp.pos",              "允许使用 /tppos 指令"),
            Topic.GetTopic("tpallow",          "tshock.tp.block",           "允许使用 /tpallow 指令"),
            Topic.GetTopic("tpoverride",     "tshock.tp.override",       "强制传送（不用对方执行 /tpallow)"),
            Topic.GetTopic("tpsilent",          "tshock.tp.silent",           "静默传送（不通知对方）"),
            
            Topic.GetTopic("home",             "tshock.tp.home",              "允许使用 /home 回城 指令"),
            Topic.GetTopic("spawn",            "tshock.tp.spawn",             "允许使用 /spawn 回重生点 指令"),

            Topic.GetTopic("tprod",              "tshock.tp.rod",                  "允许使用 混沌传送仗"),
            Topic.GetTopic("tpwormhole",   "tshock.tp.wormhole",        "允许使用 虫洞药水"),
            Topic.GetTopic("tppylon",          "tshock.tp.pylon",                "允许使用 晶塔"),
            Topic.GetTopic("tppotion",        "tshock.tp.tppotion",            "允许使用 传送药水"),
            Topic.GetTopic("tpconch1",       "tshock.tp.magicconch",      "允许使用 魔法海螺"),
            Topic.GetTopic("tpconch2",       "tshock.tp.demonconch",    "允许使用 恶魔海螺")
        };

        public static List<Topic> TopicWorld = new List<Topic>{
            Topic.GetTopic("tpother",       "tshock.world.events",             "忽略"),
        };


        public static List<string> GetDefaultPerms()
        {
            // TShockAPI\DB\GroupManager.cs

            // last
            // https://github.com/Pryaxis/TShock/blob/general-devel/TShockAPI/DB/GroupManager.cs#L60

            // 1.4.0.5
            // https://github.com/Pryaxis/TShock/blob/f538ceb79371776afa386e9bc7648366f16b897c/TShockAPI/DB/GroupManager.cs
            // https://github.com/Pryaxis/TShock/blob/f538ceb79371776afa386e9bc7648366f16b897c/TShockAPI/Permissions.cs

            // guest, default, vip, newadmin, admin, trustedadmin, owner
            return new List<string>(){
                // 4.5.12 default组 默认权限
                // "tshock.warp",                                       // 传送点管理
                // "tshock.account.changepassword",       // 更改密码
                // "tshock.account.logout",             // 登出
                // "tshock.npc.summonboss",			// 召唤boss
                // "tshock.whisper",                        // 玩家私信
                // "tshock.tp.wormhole",              // 虫洞药水
                // "tshock.world.paint",				// 使用油漆

                // 下面4条 1.4.0.5 的default组没有
                "tshock.tp.pylon",                          // 传送晶塔
                "tshock.tp.tppotion",                   // 传送药水
                "tshock.tp.magicconch",             // 魔法海螺
                "tshock.tp.demonconch",           // 恶魔海螺

                // guest组默认权限
                // "tshock.world.modify",           // 挖掘砍树建造
                // "tshock.account.register",      //  注册
                // "tshock.account.login",        //   登录
                // "tshock.partychat",
                // "tshock.thirdperson",
                // "tshock.canchat",
                // "tshock.synclocalarea",
                // "tshock.sendemoji",

                "tshock.tp.rod",                                 // 混沌传送法杖

                "tshock.world.toggleparty",             // 开派对
                "tshock.world.time.usesundial",     // 附魔日晷
                "tshock.world.editspawn",			 // 设置出生点
                "tshock.world.movenpc",			    // 为NPC分配房屋

                "tshock.npc.hurttown",				// 伤害NPC
                "tshock.npc.startinvasion",		  // 召唤入侵
                "tshock.npc.startdd2",				// 天国事件
                "tshock.npc.spawnpets",			 // 生成城镇宠物

                "tshock.ignore.removetile",         // 忽略 乱挖砖块(使用炸弹等) 检测
                "tshock.ignore.liquid",                // 忽略 液体 检测
                // "tshock.ignore.noclip",               // 忽略 穿墙 检测
                "tshock.ignore.paint",  			  // 忽略 油漆 检测
                "tshock.ignore.placetile",           // 忽略 替换方块 检测
                "tshock.ignore.projectile",         // 忽略 射弹 检测
                "tshock.ignore.damage",           // 忽略 高伤害 检测
                "tshock.ignore.sendtilesquare",	// 允许 锤击改变飞镖机关方向
            };
        }

        

    }
        public class Topic{
            public string cmd="";
            public string perm = "";

            public string description = "";

            public static Topic GetTopic(string _cmd, string _perm, string _description)
            {
                Topic t = new Topic();
                t.cmd = _cmd;
                t.perm = _perm;
                t.description = _description;
                return t;
            }
        }
}