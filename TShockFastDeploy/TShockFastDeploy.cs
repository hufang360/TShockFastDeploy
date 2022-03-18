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
                op.SendInfoMessage("/fd init，快速处理（开服通用权限和设置）");
                op.SendInfoMessage("/fd perm，设置常用权限，并新增GM超管组");
                op.SendInfoMessage("/fd group <组名称>，设置本插件的默认组名称");
                op.SendInfoMessage("/fd reload，重载配置");
                op.SendInfoMessage("输入 /fd help 2 查看更多");
            }
            void ShowHelpText2(){
                op.SendInfoMessage("/fd add <指令>，指令授权");
                op.SendInfoMessage("/fd del <指令>，取消授权");
                op.SendInfoMessage("/fd refer <journey | ignore | tp>，授权参考");
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

                // 设置默认组
                case "group":
                    if( args.Parameters.Count<2 )
                    {
                        op.SendInfoMessage("输入错误，需要提供组名称，例如: /fd group default");
                        return;
                    }
                    bool flag = false;
                    string subcmd = args.Parameters[1].ToLowerInvariant();
                    foreach (Group g in TShock.Groups.groups)
                    {
                        if( g.Name==subcmd )
                        {
                            flag = true;
                            break;
                        }
                    }
                    if( flag )
                    {
                        _config.Group = subcmd;
                        Save();
                    } else {
                        op.SendInfoMessage($"{subcmd} 用户组不存在");
                    }
                    return;

                // 给某个指令授权
                case "add":
                    AddAuth(args);
                    return;

                // 取消授权
                case "del":
                    DeleteAuth(args);
                    return;

                // 查看相关类别的权限
                case "refer":
                    if( args.Parameters.Count<2 )
                    {
                        op.SendInfoMessage("输入错误，例如: /fd refer tp");
                        return;
                    }
                    ReferTopic(op, args.Parameters[1].ToLowerInvariant());
                    return;

                // 重载配置
                case "reload":
                    reload(true);
                    op.SendSuccessMessage("[FastDeploy]配置已重载！");
                    return;

                // 帮助
                case "help":
                    if( args.Parameters.Count==2 )
                        ShowHelpText2();
                    else
                        ShowHelpText();
                    return;
                
                default: ShowHelpText(); return;
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

            // https://tshock.readme.io/docs/config-settings
            List<string> msgs = new List<string>(){
                $"-----[FastDeploy]快速开服-----",
                "已做了如下操作（强制开荒等设置，需重启才能生效）：",
                $"1 {GetDesc(_config.SSCEnable)} 强制开荒（SSC）",
                "",
                $"2.1 服务器日志将保存在 {_config.LogPath}（LogPath={_config.LogPath}）",
                $"2.2 {GetDesc(_config.RequireLogin)} 注册登录（RequireLogin={_config.RequireLogin}）",
                $"2.3 {GetDesc(_config.RequireLogin)} 调试（DebugLogs={_config.DebugLogs}）",
                $"2.4 {GetDesc(_config.AnnounceSave)} 保存地图提示（AnnounceSave={_config.AnnounceSave}）",
                $"2.5 {GetDesc(_config.ShowBackupAutosaveMessages)} 自动备份提示（ShowBackupAutosaveMessages={_config.ShowBackupAutosaveMessages}）",
                $"2.6 {GetDesc(_config.EnableChatAboveHeads)} 头顶聊天文字（EnableChatAboveHeads={_config.EnableChatAboveHeads}）",
                $"2.7 {GetDesc(_config.DisablePrimeBombs)} 禁用机械骷髅王炸弹（DisablePrimeBombs={_config.DisablePrimeBombs}）",
                $"2.8 {_config.RespawnSeconds}s 复活时间（RespawnSeconds={_config.RespawnSeconds}）",
                $"2.9 {_config.RespawnBossSeconds}s Boss战复活时间（RespawnBossSeconds={_config.RespawnBossSeconds}）",
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


        // 指令授权
        private void AddAuth(CommandArgs args)
        {
            TSPlayer op = args.Player;
            if(args.Parameters.Count<2){
                op.SendErrorMessage("语法错误，请提供要授权的指令名称, 例如：/fd add tpnpc");
                return;
            }

            args.Parameters.RemoveAt(0);
            string cmdStr = string.Join("",args.Parameters);
            List<string> perms = FindACmdPerm(cmdStr);

            // 设置权限
            string msg = "";
            bool success = AddPerm(_config.Group, perms, out msg);
            if( success )
            {
                op.SendSuccessMessage($"成功添加以下权限到默认组({_config.Group}):");
                op.SendSuccessMessage( string.Join("\n", perms) );
            } else {
                op.SendErrorMessage($"操作失败!原因：{msg}");
            }

        }


        // 不允许使用指令
        private void DeleteAuth(CommandArgs args)
        {
            TSPlayer op = args.Player;
            if(args.Parameters.Count<2){
                op.SendErrorMessage("语法错误，需要输入指令名称, 例如：/fd del tpnpc");
                return;
            }
            args.Parameters.RemoveAt(0);
            string cmdStr = string.Join("",args.Parameters);
            List<string> perms = FindACmdPerm(cmdStr);

            // 设置权限
            string msg = "";
            bool success = DeletePerm(_config.Group, perms, out msg);
            if( success )
            {
                op.SendSuccessMessage($"已从默认组({_config.Group})移除了如下权限:");
                op.SendSuccessMessage( string.Join("\n", perms) );
            } else {
                op.SendErrorMessage($"操作失败!原因：{msg}");
            }

        }

        private List<string> FindACmdPerm(string cmdStr)
        {
            List<string> perms = new List<string>();

            // 特殊处理
            switch (cmdStr)
            {
                // 设置旅行模式权限
                case "journey":
                case "jour":
                    perms = ConfigHelper.GetJourneyPerms();
                    break;

                // 传送点管理
                case "warp add":
                case "warp del":
                case "warp hide":
                case "warp send":
                    perms.Add(Permissions.managewarp);
                    break;
            }


            // 查找指令
            if( perms.Count==0 ){
                foreach (Command cmd in Commands.ChatCommands)
                {
                    if( cmd.Names.Contains(cmdStr) )
                    {
                        perms = cmd.Permissions;
                        break;
                    }
                }
            }
            // 旅行相关
            if( perms.Count==0 ){
                foreach (Topic t in ConfigHelper.TopicJourney)
                {
                    if( t.cmd == cmdStr )
                    {
                        perms.Add(t.perm);
                        break;
                    }
                }
            }

            // ignore相关
            if( perms.Count==0 ){
                foreach (Topic t in ConfigHelper.TopicIgnore)
                {
                    if( t.cmd == cmdStr )
                    {
                        perms.Add(t.perm);
                        break;
                    }
                }
            }

            // tp相关
            if( perms.Count==0 ){
                foreach (Topic t in ConfigHelper.TopicTP)
                {
                    if( t.cmd == cmdStr )
                    {
                        perms.Add(t.perm);
                        break;
                    }
                }
            }

            return perms;
        }

        // 指令参考
        private void ReferTopic(TSPlayer op, string topicName)
        {
            List<Topic> topics = new List<Topic>();
            switch (topicName)
            {
                case "help":
                    op.SendInfoMessage("可用的参考有 journey、ignore 和tp，示例：/fd refer tp");
                    return;

                case "journey":
                case "jour":
                    topics =  ConfigHelper.TopicJourney;
                    break;

                case "ignore":
                case "ig":
                    topics =  ConfigHelper.TopicIgnore;
                    break;

                case "tp":
                    topics =  ConfigHelper.TopicTP;
                    break;
            }

            if( topics.Count>0 )
            {
                string text = "权限名 | 描述 | 授权";
                foreach (Topic t in topics)
                {
                    text += $"\n{t.perm} | {t.description} | /fd add {t.cmd}";
                }
                op.SendInfoMessage(text);
            } else {
                op.SendInfoMessage($"未提供 {topicName} 相关的参考");
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