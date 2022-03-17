using TShockAPI;
using System;
using System.Reflection;


namespace AutoRegister
{
    class Compatible
    {
        public static String DefaultRegistrationGroupName
        {
            // 1.4.0.5
            // get { return TShock.Config.DefaultRegistrationGroupName; }

            get { return TShock.Config.Settings.DefaultRegistrationGroupName; }
        }
    }
}