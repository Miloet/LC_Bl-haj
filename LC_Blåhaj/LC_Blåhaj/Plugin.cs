using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using LC_Blåhaj.Patches;

namespace LC_Blåhaj
{
    [BepInPlugin(modGUID, modName, modVersion)]
    public class BlåhajReplacerMod : BaseUnityPlugin
    {
        private const string modGUID = "Mellowdy.BlahajReplacer";
        private const string modName = "BlahajReplacer";
        private const string modVersion = "1.0.0";


        private readonly Harmony harmony = new Harmony(modGUID);

        public static ManualLogSource mls;

        private static BlåhajReplacerMod instance;

        void Awake()
        {
            if (instance == null) instance = this;

            mls = BepInEx.Logging.Logger.CreateLogSource(modGUID);

            mls.LogInfo("Blåhaj is soothing your soul");

            harmony.PatchAll(typeof(BlåhajReplacerMod));
            harmony.PatchAll(typeof(ItemReplacerPatch));

        }
    }
}
