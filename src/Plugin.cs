using BepInEx;
using BepInEx.Logging;
using System;
using System.Security.Permissions;
using System.Security;
using MonoMod.RuntimeDetour;
using IL.MoreSlugcats;

#pragma warning disable CS0618 // Type or member is obsolete
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
[module: UnverifiableCode]
#pragma warning restore CS0618 // Type or member is obsolete


namespace ThrowFix
{
    [BepInPlugin(AUTHOR + "." + MOD_ID, MOD_NAME, VERSION)]
    internal class Plugin : BaseUnityPlugin
    {
        public static new ManualLogSource Logger { get; private set; } = null!;

        public const string VERSION = "1.0.0";  
        public const string MOD_NAME = "Throw Fix";
        public const string MOD_ID = "throwfix";
        public const string AUTHOR = "forthbridge";

        public void OnEnable()
        {
            Logger = base.Logger;

            new Hook(typeof(Player).GetProperty(nameof(Player.ThrowDirection)).GetGetMethod(), Player_ThrowDirection);

            On.Weapon.Thrown += Weapon_Thrown;
            On.Weapon.Shoot += Weapon_Shoot;

        }

        private void Weapon_Shoot(On.Weapon.orig_Shoot orig, Weapon self, Creature shotBy, UnityEngine.Vector2 thrownPos, UnityEngine.Vector2 throwDir, float force, bool eu)
        {
            orig(self, shotBy, thrownPos, throwDir, force, eu);

            self.changeDirCounter = 0;
        }

        private void Weapon_Thrown(On.Weapon.orig_Thrown orig, Weapon self, Creature thrownBy, UnityEngine.Vector2 thrownPos, UnityEngine.Vector2? firstFrameTraceFromPos, RWCustom.IntVector2 throwDir, float frc, bool eu)
        {
            orig(self, thrownBy, thrownPos, firstFrameTraceFromPos, throwDir, frc, eu);

            self.changeDirCounter = 0;
        }

        private int Player_ThrowDirection(Func<Player, int> orig, Player self)
        {
            if (self.input[0].x == 0) return self.flipDirection;

            return self.input[0].x;
        }
    }
}
