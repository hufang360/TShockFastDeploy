using System.Linq;
using System;
using System.IO;
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

        private static Config _config;


        private static string FDPath = Path.Combine(TShock.SavePath, "FastDeploy");
        private static string FDConfigPath = Path.Combine(TShock.SavePath, "FastDeploy", "config.json");

        private static string ConfigPath = Path.Combine(TShock.SavePath, "config.json");
        private static string ServerSideCharacterConfigPath = Path.Combine(TShock.SavePath, "sscconfig.json");


        public Plugin(Main game) : base(game)
        {
        }

        public override void Initialize()
        {
            Commands.ChatCommands.Add(new Command(new List<string>() {"fastdeploy"}, FastDeploy, "fastdeploy", "fd") { HelpText = "快速开服"});

            // 创建配置文件夹
            if( !Directory.Exists(FDPath) )
                Directory.CreateDirectory(FDPath);
        }

        // 重载配置
        public void reload(bool force=false)
        {
            if( _config==null || force )
                _config = Config.Load(FDConfigPath);
        }

        // 保存配置
        public void Save(){
            Config.Save(_config,  FDConfigPath);
        }


        private void FastDeploy(CommandArgs args)
        {
            reload();

            TSPlayer op = args.Player;
            void ShowHelpText()
            {
                op.SendInfoMessage("/fd init，快速处理好开服所需的权限和设置");
                op.SendInfoMessage("/fd perm，给默认组设置常用权限，并新增GM超管组");
                op.SendInfoMessage("/fd journey，授权默认组拥有旅行模式的全部权限");
                op.SendInfoMessage("/fd add <指令>，授权使用某个指令");
                op.SendInfoMessage("/fd del <指令>，取消某个指令的授权");
                op.SendInfoMessage("/fd guest，设置插件的默认组为 guest");
                op.SendInfoMessage("/fd default，设置插件的默认组为 default");
                op.SendInfoMessage("/fd reload，重载配置");
            }

            if(args.Parameters.Count==0){
                ShowHelpText();
                return;
            }

            string cmd = args.Parameters[0].ToLowerInvariant();
            switch (cmd)
            {
                // 快速开服
                case "init":
                    InitSever(op);
                    return;

                //  基础权限
                case "perm":
                    InitPerm(op);
                    return;

                //  基础权限
                case "journey":
                case "jour":
                case "旅行":
                case "旅途":
                case "旅程":
                    InitJourney(op);
                    return;

                //  将默认组设置成 default
                //  将默认组设置成 guest
                case "default":
                case "guest":
                    _config.Group = cmd;
                    Save();
                    return;

                // 允许使用权限
                case "add":
                    AllowCommand(args);
                    return;

                // 不允许使用权限
                case "del":
                    DisallowCommand(args);
                    return;

                // 重载配置
                case "reload":
                    reload(true);
                    op.SendSuccessMessage("[FastDeploy]配置已重载！");
                    return;

                // 帮助
                case "help":
                    ShowHelpText();
                    return;
            }
        }


        // 设置 快速开服
        private void InitSever(TSPlayer op)
        {
            // 修改 sscconfig.json 更新此文件需重启服务器
            TShock.ServerSideCharacterConfig.Settings.Enabled = _config.SSCEnable;
            TShock.ServerSideCharacterConfig.Write(ServerSideCharacterConfigPath);

            // 修改 config.json
            TShock.Config.Settings.RequireLogin = _config.RequireLogin;
            TShock.Config.Settings.LogPath = _config.LogPath;
            TShock.Config.Settings.DebugLogs = _config.DebugLogs;
            TShock.Config.Settings.AnnounceSave = _config.AnnounceSave;
            TShock.Config.Settings.EnableChatAboveHeads = _config.EnableChatAboveHeads;
            TShock.Config.Settings.DisablePrimeBombs = _config.DisablePrimeBombs;
            TShock.Config.Settings.RespawnSeconds = _config.RespawnSeconds;
            TShock.Config.Settings.RespawnBossSeconds = _config.RespawnBossSeconds;
            TShock.Config.Write(ConfigPath);

            // 重载配置
            TShock.Utils.Reload();
			TShockAPI.Hooks.GeneralHooks.OnReloadEvent(op);

            string GetDesc(bool foo ){
                return foo ? "开启":"关闭";
            }

            List<string> msgs = new List<string>(){
                $"-----[FastDeploy]快速开服-----",
                "已做了如下操作（强制开荒等设置，需重启才能生效）：",
                $"1 {GetDesc(_config.SSCEnable)} 强制开荒（SSC）",
                "",
                $"2.1 服务器日志将保存在 {_config.LogPath}（LogPath={_config.LogPath}）",
                $"2.2 {GetDesc(_config.RequireLogin)} 注册登录（RequireLogin={_config.RequireLogin}）",
                $"2.3 {GetDesc(_config.RequireLogin)} 调试（DebugLogs={_config.DebugLogs}）",
                $"2.4 {GetDesc(_config.AnnounceSave)} 保存地图提示（AnnounceSave={_config.AnnounceSave}）",
                $"2.5 {GetDesc(_config.EnableChatAboveHeads)} 头顶聊天文字（EnableChatAboveHeads={_config.EnableChatAboveHeads}）",
                $"2.6 {GetDesc(_config.DisablePrimeBombs)} 禁用机械骷髅王炸弹（DisablePrimeBombs={_config.DisablePrimeBombs}）",
                $"2.7 {_config.RespawnSeconds}s 复活时间（RespawnSeconds={_config.RespawnSeconds}）",
                $"2.8 {_config.RespawnBossSeconds}s Boss战复活时间（RespawnBossSeconds={_config.RespawnBossSeconds}）",
                ""
            };
            op.SendSuccessMessage( string.Join("\n", msgs) );


            // 设置权限
            string msg = "";
            bool success = AddPerm(_config.Group,  _config.Permissions, out msg);
            if( success ){
                op.SendSuccessMessage($"3 成功添加以下权限到默认组({_config.Group}):");
                op.SendSuccessMessage( string.Join("\n", _config.Permissions) );
                op.SendSuccessMessage( $"输入 [c/96FF96:{Specifier}group delperm {_config.Group} <权限...>] 可删除对应权限\n");
            } else {
                op.SendErrorMessage($"3 添加权限到默认组时失败！原因：{msg}\n");
            }

            msg = "";
            // success = AddGroup("GM","*,!tshock.ignore.ssc", out msg);
            success = AddGroup("GM",string.Join(",", _config.GMPermissions), out msg);
            if( success ){
                op.SendSuccessMessage($"4 已新建 GM 超管组，并添加了以下权限:");
                op.SendSuccessMessage( string.Join("\n", _config.GMPermissions) );
                op.SendSuccessMessage( $"输入 [c/96FF96:{Specifier}user group <玩家名> GM] 添加服主");

                return;
            }

            if( msg!="组已存在" ){
                op.SendErrorMessage($"操作失败！原因：{msg}");
                return;
            }

            msg = "";
            success = AddPerm("GM",  _config.GMPermissions, out msg);
            if( success ){
                op.SendSuccessMessage($"4 成功添加以下权限到 GM 超管组:");
                op.SendSuccessMessage( string.Join("\n", _config.GMPermissions) );
                op.SendSuccessMessage( $"输入 [c/96FF96:{Specifier}user group <玩家名> GM] 添加服主");
            } else {
                op.SendErrorMessage($"4 添加权限到 GM 超管组时失败！原因：{msg}");
            }
        }


        // 设置权限
        private void InitPerm(TSPlayer op)
        {
            AddDefaultPermission(op);
            AddGMGroup(op);
        }


        // 设置旅行模式权限
        private void InitJourney(TSPlayer op)
        {
            List<string> perms = new List<string>()
            {
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

            string msg = "";
            bool success = AddPerm(_config.Group, perms, out msg);
            if( success )
            {
                op.SendSuccessMessage($"成功添加以下权限到默认组({_config.Group}):");
                op.SendSuccessMessage( string.Join("\n", perms) );
                op.SendSuccessMessage( $"输入 /group delperm {_config.Group} <权限...> 可删除对应权限");
            } else {
                op.SendErrorMessage($"操作失败!原因：{msg}");
            }
        }


        // 允许使用指令
        private void AllowCommand(CommandArgs args)
        {
            TSPlayer op = args.Player;
            if(args.Parameters.Count<2){
                op.SendErrorMessage("语法错误，需要输入指令名称, 例如：/fd add tpnpc");
                return;
            }
            string cmdStr = args.Parameters[1];

            foreach (Command cmd in Commands.ChatCommands)
            {
                if( cmd.Names.Contains(cmdStr) )
                {
                    string msg = "";
                    bool success = AddPerm(_config.Group, cmd.Permissions, out msg);
                    if( success )
                    {
                        op.SendSuccessMessage($"成功添加以下权限到默认组({_config.Group}):");
                        op.SendSuccessMessage( string.Join("\n", cmd.Permissions) );
                    } else {
                        op.SendErrorMessage($"操作失败!原因：{msg}");
                    }
                }
            }
        }


        // 不允许使用指令
        private void DisallowCommand(CommandArgs args)
        {
            TSPlayer op = args.Player;
            if(args.Parameters.Count<2){
                op.SendErrorMessage("语法错误，需要输入指令名称, 例如：/fd del tpnpc");
                return;
            }
            string cmdStr = args.Parameters[1];

            foreach (Command cmd in Commands.ChatCommands)
            {
                if( cmd.Names.Contains(cmdStr) )
                {
                    string msg = "";
                    bool success = DeletePerm(_config.Group, cmd.Permissions, out msg);
                    if( success )
                    {
                        op.SendSuccessMessage($"已从默认组({_config.Group})移除了如下权限:");
                        op.SendSuccessMessage( string.Join("\n", cmd.Permissions) );
                    } else {
                        op.SendErrorMessage($"操作失败!原因：{msg}");
                    }
                }
            }
        }


        // 为默认组添加权限
        private void AddDefaultPermission(TSPlayer op)
        {
            string msg = "";
            bool success = AddPerm(_config.Group,  _config.Permissions, out msg);
            if( success ){
                op.SendSuccessMessage($"成功添加以下权限到默认组({_config.Group}):");
                op.SendSuccessMessage( string.Join("\n", _config.Permissions) );
                op.SendSuccessMessage( $"输入 /group delperm <组名> <权限...> 可删除对应权限");
            } else {
                op.SendErrorMessage($"操作失败！原因：{msg}");
            }
        }


         // 新建GM组
        private void AddGMGroup(TSPlayer op)
        {
            string msg = "";
            bool success = AddGroup("GM","*,!tshock.ignore.ssc", out msg);
            if( success ){
                op.SendSuccessMessage("已新增 GM 超管组，输入 [c/96FF96:/user group <玩家名> GM] 添加服主");
            } else {
                if( msg=="组已存在" )
                    op.SendInfoMessage("组已存在，输入 [c/96FF96:/user group <玩家名> GM] 添加服主");
                else
                    op.SendErrorMessage($"操作失败！原因：{msg}");
            }
        }


        // 添加组方法
        private bool AddGroup(string groupName, string permissions, out string result)
        {
            try
            {
                TShock.Groups.AddGroup(groupName, null, permissions, TShockAPI.Group.defaultChatColor);
                result = "添加组成功!";
            }
            catch (GroupExistsException)
            {
                result = "组已存在";
                return false;
            }
            catch (GroupManagerException ex)
            {
                result = ex.ToString();
                return false;
            }
            return true;
        }


        // 添加权限方法
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


        // 删除权限
        private bool DeletePerm(string groupName, List<string> permissions, out string result)
        {
            result = "";
            try
            {
                string response = TShock.Groups.DeletePermissions(groupName, permissions);
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

        internal static string Specifier
		{
			get { return Commands.Specifier; }
		}

    }
}