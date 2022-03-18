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
            c.Permissions = ConfigHelper.GetDefaultPerms();
            c.GMPermissions = ConfigHelper.GetGMPerms();

            return c;
        }
    }
}