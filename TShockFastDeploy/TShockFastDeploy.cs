using System.Linq;
using System;
using System.Collections.Generic;
using System.Reflection;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.DB;


namespace Plugin
{
    [ApiVersion(2, 1)]
    public class Plugin : TerrariaPlugin
    {
        public override string Name => "FastDeploy";
        public override string Description => "快速开服";
        public override string Author => "hufang360";
        public override Version Version => Assembly.GetExecutingAssembly().GetName().Version;

        public Plugin(Main game) : base(game)
        {
        }

        public override void Initialize()
        {
            Commands.ChatCommands.Add(new Command(new List<string>() {"fastdeploy"}, ProcessCommand, "fastdeploy", "fd") { HelpText = ""});
            AddGMGroup();
            AddDefaultPermission();
        }

        private void AddGMGroup()
        {
            string msg = "";
            AddGroup("GM","*,!tshock.ignore.ssc", out msg);
            TShock.Log.ConsoleInfo($"[FastDeploy]{msg}");
        }

        private void AddDefaultPermission()
        {
            List<string> perms = new List<string>()
            {
                // 4.5.12 default组 默认权限
                // "tshock.account.changepassword",       // 更改密码
                // "tshock.account.logout",             // 登出
                // "tshock.npc.summonboss",			// 召唤boss
                // "tshock.whisper",                        // 玩家私信
                // "tshock.tp.wormhole",              // 虫洞药水
                // "tshock.world.paint",				// 使用油漆
                // "tshock.tp.pylon",                    // 传送晶塔
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
                "tshock.world.time.usesundial",     // 使用日晷
                "tshock.world.editspawn",			 // 设置全图玩家的出生点
                "tshock.world.movenpc",			    // 移动NPC

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
                "tshock.ignore.sendtilesquare",	// 忽略 修改地图限制(锤击改变飞镖机关方向)
            };
            string msg = "";
            bool success = AddPerm("default", perms, out msg);
            if( success )
                TShock.Log.ConsoleInfo($"[FastDeploy]{msg}");
            else
                TShock.Log.ConsoleError($"[FastDeploy]{msg}");
        }


        private void ProcessCommand(CommandArgs args)
        {
            if(args.Parameters.Count==0){
                args.Player.SendErrorMessage("语法错误，请输入 /fd help 查看帮助");
                return;
            }

            TSPlayer op = args.Player;
            List<string> perms = new List<string>();
            string cmd = args.Parameters[0].ToLowerInvariant();

            if( _perms.Keys.Contains( cmd ) )
            {
                perms.Add(_perms[cmd]);
            } else {
                switch (cmd)
                {
                    // case "init":
                    //     // 添加默认组
                    //     AddGMGroup();
                    //     // 给默认组添加权限
                    //     AddDefaultPermission();
                    //     break;

                    case "journey":
                    case "jour":
                    case "旅行":
                    case "旅途":
                        perms = new List<string>()
                        {
                            "tshock.journey.research",
                            "tshock.journey.biomespreadfreeze",
                            "tshock.journey.rain.freeze",
                            "tshock.journey.time.freeze",
                            "tshock.journey.wind.freeze",
                            "tshock.journey.placementrange"
                        };
                        break;
                    
                    default:
                        op.SendErrorMessage($"暂时未处理与 {cmd} 指令有关的权限！");
                        return;
                }
            }

            if( perms.Count==0 )
                return;

            string msg = "";
            bool success = AddPerm("default", perms, out msg);

            if( success ){
                args.Player.SendSuccessMessage("成功添加以下权限到默认用户组:");
                args.Player.SendSuccessMessage( string.Join("\n", perms) );
            } else {
                args.Player.SendErrorMessage($"操作失败!原因：{msg}");
            }
        }

        static Dictionary<string, string> _perms = new Dictionary<string, string>
        {
            {"tpnpc", Permissions.tpnpc },
            {"home", Permissions.home },
            {"me", Permissions.cantalkinthird },
            {"register", Permissions.canregister }
        };



        private bool AddGroup(string groupName, string permissions, out string result)
        {
            try
            {
                TShock.Groups.AddGroup(groupName, null, permissions, TShockAPI.Group.defaultChatColor);
                result = "添加组成功!";
            }
            catch (GroupExistsException)
            {
                result = "组已存在!";
                return false;
            }
            catch (GroupManagerException ex)
            {
                result = ex.ToString();
                return false;
            }
            return true;
        }

        private bool AddPerm(string groupName, List<string> permissions, out string result)
        {
            result = "";
            try
            {
                string response = TShock.Groups.AddPermissions(groupName, permissions);
                if (response.Length > 0)
                {
                    result = response;
                }
            }
            catch (GroupManagerException ex)
            {
                result = ex.ToString();
                return false;
            }
            return true;
        }

    }
}