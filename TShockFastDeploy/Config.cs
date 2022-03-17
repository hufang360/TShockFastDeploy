using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;


namespace Plugin
{
    public class Config{

        // 默认用户组
        public string Group = "default";

        // 默认用户组拥有的权限
        public List<string> Permissions = new List<string>();

        // GM组拥有的权限
        public List<string> GMPermissions = new List<string>();

        // 开启SSC
        public bool SSCEnable = true;

        // ----------------------------------------

        // 需要登录
        public bool RequireLogin = true;

        // 日志存放路径
        public string LogPath = "tshock/logs";

        // 日志调试
        public bool DebugLogs = false;

        // 保存地图提示
        public bool AnnounceSave = false;
        
        // 自动备份提示
        public bool ShowBackupAutosaveMessages = false;

        // 头顶聊天文字显示
        public bool EnableChatAboveHeads = true;

        // ftw的机械骷髅王不炸图（1.4.0.5 无此选项）
        public bool DisablePrimeBombs = true;


        // 重生秒数，原版默认是15秒
        public int RespawnSeconds = 5;

        // BOSS重生秒数，原版默认是30秒
        public int RespawnBossSeconds = 10;

        // ----------------------------------------

        // // 墓碑生成
        // public bool DisableTombstones = true;


        // // 开启圣诞节
        // public bool ForceXmas = false;

        // // 开启万圣节
        // public bool ForceHalloween = false;


        public static Config Load(string path)
        {
            if (File.Exists(path))
            {
                return JsonConvert.DeserializeObject<Config>(File.ReadAllText(path));
            }
            else
            {
                var c = InitDefault();
                File.WriteAllText(path, JsonConvert.SerializeObject(c, Formatting.Indented));
                return c;
            }
        }

        public static void Save(Config c, string path){
            File.WriteAllText(path, JsonConvert.SerializeObject(c, Formatting.Indented));
        }


        public static Config InitDefault()
        {
            Config c = new Config();
            c.Group = "default";
            c.Permissions = GetDefaultPerms();
            c.GMPermissions = GetGMPerms();

            return c;
        }

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
                "tshock.journey.biomespreadfreeze", //腐化传染
                "tshock.journey.setdifficulty"              // 游戏难度
           };
            // List<string> perms = TShockAPI.Handlers.NetModules.CreativePowerHandler.PermissionToDescriptionMap.Keys.ToList();
            // if( !perms.Contains(Permissions.journey_contributeresearch) ){
            //     perms.Add(Permissions.journey_contributeresearch);
            // }
        }


        public static List<string> GetDefaultPerms()
        {
            // TShockAPI\DB\GroupManager.cs

            // last
            // https://github.com/Pryaxis/TShock/blob/general-devel/TShockAPI/DB/GroupManager.cs#L60

            // 1.4.0.5
            // https://github.com/Pryaxis/TShock/blob/f538ceb79371776afa386e9bc7648366f16b897c/TShockAPI/DB/GroupManager.cs
            
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
                "tshock.ignore.noclip",               // 忽略 穿墙 检测
                "tshock.ignore.paint",  			  // 忽略 油漆 检测
                "tshock.ignore.placetile",           // 忽略 替换方块 检测
                "tshock.ignore.projectile",         // 忽略 射弹 检测
                "tshock.ignore.damage",           // 忽略 高伤害 检测
                "tshock.ignore.sendtilesquare",	// 允许 锤击改变飞镖机关方向
            };
        }

    }
}