using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        private static readonly string FDPath = Path.Combine(TShock.SavePath, "FastDeploy");
        private static readonly string FDConfigPath = Path.Combine(TShock.SavePath, "FastDeploy", "config.json");

        private static readonly string ConfigPath = Path.Combine(TShock.SavePath, "config.json");
        private static readonly string ServerSideCharacterConfigPath = Path.Combine(TShock.SavePath, "sscconfig.json");


        public Plugin(Main game) : base(game)
        {
        }

        public override void Initialize()
        {
            Commands.ChatCommands.Add(new Command(new List<string>() { "fastdeploy", "tshock.journey.research" }, FastDeploy, "fastdeploy", "fd") { HelpText = "快速开服" });

            // 创建配置文件夹
            if (!Directory.Exists(FDPath))
                Directory.CreateDirectory(FDPath);
        }

        // 重载配置
        private void Reload(bool force = false)
        {
            if (_config == null || force)
                _config = Config.Load(FDConfigPath);
        }

        // 保存配置
        private void Save()
        {
            Config.Save(_config, FDConfigPath);
        }

        // command
        private void FastDeploy(CommandArgs args)
        {
            Reload();

            TSPlayer op = args.Player;
            void ShowHelpText()
            {
                if (!PaginationTools.TryParsePageNumber(args.Parameters, 1, op, out int pageNumber))
                    return;
                List<string> lines = new List<string>() {
                    "/fd init，快速处理（开服通用权限和设置）",
                    "/fd perm，设置常用权限，并新增GM超管组",
                    "/fd group <组名称>，设置本插件的默认组名称",
                    "/fd reload，重载配置",
                    "/fd add <指令>，指令授权",
                    "/fd del <指令>，取消授权",
                    "/fd list，列出已授权的指令",
                    "/fd refer <journey | ignore | tp>，授权参考"
                };
                PaginationTools.SendPage(
                    op, pageNumber, lines,
                    new PaginationTools.Settings
                    {
                        HeaderFormat = "【快速开服】指令帮助 ({0}/{1})：",
                        FooterFormat = "输入 {0}fd help {{0}} 查看更多".SFormat(Specifier)
                    }
                );
            }

            if (args.Parameters.Count == 0)
            {
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
                    if (args.Parameters.Count < 2)
                    {
                        op.SendInfoMessage($"默认组为 {_config.Group}，输入 /fd group <default> 可进行更改");
                        return;
                    }
                    string subcmd = args.Parameters[1].ToLowerInvariant();
                    if (GroupExists(subcmd))
                    {
                        _config.Group = subcmd;
                        Save();
                    }
                    else
                    {
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

                // 列出授权指令
                case "list":
                    ListAuth(args);
                    return;

                // 查看相关类别的权限
                case "refer":
                    if (args.Parameters.Count < 2)
                    {
                        op.SendInfoMessage("输入错误，例如: /fd refer tp");
                        return;
                    }
                    ReferTopic(op, args.Parameters[1].ToLowerInvariant());
                    return;

                // 重载配置
                case "reload":
                    Reload(true);
                    op.SendSuccessMessage("[FastDeploy]配置已重载！");
                    return;

                // 帮助
                default:
                case "help":
                    ShowHelpText();
                    return;
            }
        }


        // 设置 快速开服
        private void InitSever(TSPlayer op)
        {
            #region json file
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

            string GetDesc(bool foo)
            {
                return foo ? "开启" : "关闭";
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
            op.SendSuccessMessage(string.Join("\n", msgs));
            #endregion

            #region default perm
            // 设置权限
            if (AddPerm(_config.Group, _config.Permissions, out string msg))
            {
                op.SendSuccessMessage($"3 成功添加以下权限到默认组({_config.Group}):");
                op.SendSuccessMessage(string.Join("\n", _config.Permissions));
                op.SendSuccessMessage($"输入 [c/96FF96:{Specifier}group delperm {_config.Group} <权限...>] 可删除对应权限\n");
            }
            else
            {
                op.SendErrorMessage($"3 添加权限到默认组时失败！原因：{msg}\n");
            }
            #endregion


            #region group
            // 组操作
            if (AddGroup("GM", ListToComma(_config.GMPermissions), out msg))
            {
                op.SendSuccessMessage($"4 已添加以下权限到 GM 超管组:");
                op.SendSuccessMessage(string.Join("\n", _config.GMPermissions));
                op.SendSuccessMessage($"输入 [c/96FF96:{Specifier}user group <玩家名> GM] 添加服主");
            }
            else
            {
                op.SendErrorMessage($"4 添加权限到 GM 超管组时失败！原因：{msg}");
            }
            #endregion
        }


        // 设置权限
        private void InitPerm(TSPlayer op)
        {
            // 为默认组添加权限
            if (AddPerm(_config.Group, _config.Permissions, out string msg))
            {
                op.SendSuccessMessage($"成功添加以下权限到默认组({_config.Group}):");
                op.SendSuccessMessage(string.Join("\n", _config.Permissions));
                op.SendSuccessMessage($"输入 /group delperm <组名> <权限...> 可删除对应权限");
            }
            else
            {
                op.SendErrorMessage($"操作失败！原因：{msg}");
            }

            // 新建GM组
            if (AddGroup("GM", ListToComma(_config.GMPermissions), out msg))
                op.SendSuccessMessage("已新增 GM 超管组，输入 [c/96FF96:/user group <玩家名> GM] 添加服主");
            else
                op.SendErrorMessage($"操作失败！原因：{msg}");
        }


        // 指令授权
        private void AddAuth(CommandArgs args)
        {
            TSPlayer op = args.Player;
            if (args.Parameters.Count < 2)
            {
                op.SendErrorMessage("语法错误，请提供要授权的指令名称, 例如：/fd add tpnpc");
                return;
            }

            args.Parameters.RemoveAt(0);
            string cmdStr = string.Join(" ", args.Parameters);
            List<string> perms = FindACmdPerm(cmdStr);
            if (perms.Count == 0)
            {
                op.SendSuccessMessage($"暂时无法处理 {cmdStr} 指令");
                return;
            }

            //权限已存在时
            if (GroupContainPerms(_config.Group, perms))
            {
                op.SendSuccessMessage($"{Specifier}{cmdStr} 指令已经授权过了！此指令所需的权限已经存在于默认组({_config.Group}):");
                op.SendSuccessMessage(string.Join("\n", perms));
                return;
            }

            // 设置权限
            if (AddPerm(_config.Group, perms, out string msg))
            {
                op.SendSuccessMessage($"{Specifier}{cmdStr} 指令授权成功！以下权限被添加到默认组({_config.Group}):");
                op.SendSuccessMessage(string.Join("\n", perms));
            }
            else
            {
                op.SendErrorMessage($"操作失败!原因：{msg}");
            }

        }


        // 不允许使用指令
        private void DeleteAuth(CommandArgs args)
        {
            TSPlayer op = args.Player;
            if (args.Parameters.Count < 2)
            {
                op.SendErrorMessage("语法错误，需要输入指令名称, 例如：/fd del tpnpc");
                return;
            }
            args.Parameters.RemoveAt(0);
            string cmdStr = string.Join("", args.Parameters);
            List<string> perms = FindACmdPerm(cmdStr);

            // 设置权限
            if (DeletePerm(_config.Group, perms, out string msg))
            {
                op.SendSuccessMessage($"已取消 {Specifier}{cmdStr} 指令的授权，默认组({_config.Group})中已无如下权限:");
                op.SendSuccessMessage(string.Join("\n", perms));
            }
            else
            {
                op.SendErrorMessage($"操作失败!原因：{msg}");
            }

        }


        // 列出授权的指令
        private void ListAuth(CommandArgs args)
        {
            if (!PaginationTools.TryParsePageNumber(args.Parameters, 1, args.Player, out int pageNumber))
                return;

            IEnumerable<string> cmdNames = from cmd in Commands.ChatCommands
                                           where CanRunCommand(cmd, _config.Group) && (cmd.Name != "setup" || TShock.SetupToken != 0)
                                           select Specifier + cmd.Name;

            PaginationTools.SendPage(args.Player, pageNumber, PaginationTools.BuildLinesFromTerms(cmdNames),
                new PaginationTools.Settings
                {
                    HeaderFormat = "默认组（"+_config.Group+"）可用的指令 ({0}/{1}):",
                    FooterFormat = "输入 {0}fd list {{0}} 查看更多".SFormat(Specifier)
                });
        }
        private bool CanRunCommand(Command cmd, string grpName)
        {
            if (cmd.Permissions == null || cmd.Permissions.Count < 1)
                return true;
            if (!GroupExists(grpName))
                return false;

            Group grp = TShock.Groups.GetGroupByName(grpName);
            foreach (var Permission in cmd.Permissions)
            {
                if (grp.Permissions.Contains(Permission))
                    return true;
            }
            return false;
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
            if (perms.Count == 0)
            {
                foreach (Command cmd in Commands.ChatCommands)
                {
                    if (cmd.Names.Contains(cmdStr))
                    {
                        perms = cmd.Permissions;
                        break;
                    }
                }
            }
            // 旅行相关
            if (perms.Count == 0)
            {
                foreach (Topic t in ConfigHelper.TopicJourney)
                {
                    if (t.cmd == cmdStr)
                    {
                        perms.Add(t.perm);
                        break;
                    }
                }
            }

            // ignore相关
            if (perms.Count == 0)
            {
                foreach (Topic t in ConfigHelper.TopicIgnore)
                {
                    if (t.cmd == cmdStr)
                    {
                        perms.Add(t.perm);
                        break;
                    }
                }
            }

            // tp相关
            if (perms.Count == 0)
            {
                foreach (Topic t in ConfigHelper.TopicTP)
                {
                    if (t.cmd == cmdStr)
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
                    topics = ConfigHelper.TopicJourney;
                    break;

                case "ignore":
                case "ig":
                    topics = ConfigHelper.TopicIgnore;
                    break;

                case "tp":
                    topics = ConfigHelper.TopicTP;
                    break;
            }

            if (topics.Count > 0)
            {
                string text = "权限名 | 描述 | 授权";
                foreach (Topic t in topics)
                {
                    text += $"\n{t.perm} | {t.description} | /fd add {t.cmd}";
                }
                op.SendInfoMessage(text);
            }
            else
            {
                op.SendInfoMessage($"未提供 {topicName} 相关的参考");
            }
        }


        // 添加组方法
        private bool AddGroup(string groupName, string permissions, out string result)
        {
            try
            {
                if (GroupExists(groupName))
                    TShock.Groups.AddPermissions(groupName, CommaToList(permissions));
                else
                    TShock.Groups.AddGroup(groupName, null, permissions, Group.defaultChatColor);
            }
            catch (GroupManagerException ex)
            {
                result = ex.ToString();
                return false;
            }
            result = "";
            return true;
        }

        // 添加权限方法
        private bool AddPerm(string groupName, List<string> permissions, out string result)
        {
            try
            {
                if (GroupExists(groupName))
                    TShock.Groups.AddPermissions(groupName, permissions);
                else
                    TShock.Groups.AddGroup(groupName, null, ListToComma(permissions), Group.defaultChatColor);
                result = "";
                return true;
            }
            catch (GroupManagerException ex)
            {
                result = ex.ToString();
                return false;
            }
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


        //组是否包含指定权限
        private bool GroupContainPerms(string groupName, List<string> permissions)
        {
            if (GroupExists(groupName))
            {
                Group grp = TShock.Groups.GetGroupByName(groupName);
                foreach (string perm in permissions)
                {
                    if (!grp.TotalPermissions.Contains(perm))
                        return false;
                }
                return true;
            }
            return false;
        }

        private List<string> CommaToList(string line)
        {
            return new List<string>(line.Split(','));
        }

        private string ListToComma(List<string> lines)
        {
            return string.Join(",", lines);
        }

        private bool GroupExists(string groupName)
        {
            return TShock.Groups.GroupExists(groupName);
        }

        internal static string Specifier
        {
            get { return Commands.Specifier; }
        }

    }
}