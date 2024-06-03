using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Events;
using StardewValley.Menus;

namespace SurpriseBaby1
{
    public class ModEntry : Mod
    {
        private static ModConfig Config;
        int pregnantNow = 0;

        public override void Entry(IModHelper helper) {
            helper.Events.Display.MenuChanged += OnMenuChanged;
            helper.Events.GameLoop.DayStarted += OnDayStarted;
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            Config = Helper.ReadConfig<ModConfig>();
        }
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e) {
            var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
                return;

            // register mod
            configMenu.Register(
                mod: ModManifest,
                reset: () => Config = new ModConfig(),
                save: () => Helper.WriteConfig(Config)
            );

            configMenu.AddSectionTitle(
                mod: ModManifest,
                text: () => "Uncheck this box to disable pregnancy",
                tooltip: null
            );

            // add some config options
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => "Pregnancy",
                tooltip: () => "Check the box for suprise pregnancy, uncheck the box to never be asked about kids.",
                getValue: () => Config.PregnancyAllowed,
                setValue: value => Config.PregnancyAllowed = value
            );

            configMenu.AddSectionTitle(
                mod: ModManifest,
                text: () => "Check this box to enable the pregnancy notification",
                tooltip: null
            );

            // add some config options
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => "Pregnancy Notification",
                tooltip: () => "Notifies you when your spouse gets pregnant 14 days in advance.",
                getValue: () => Config.PregnancyNotification,
                setValue: value => Config.PregnancyNotification = value
            );

        }


        private void OnDayStarted(object sender, DayStartedEventArgs e) {
            if (pregnantNow == 1) {
                if (Config.PregnancyNotification == true) { //these two are seperated because if not pregnantNow just keeps ticking up and breaks. probably better way to do this but i gotta do homework so ill fix later.
                    Game1.drawObjectDialogue("You and your partner are expecting!");
                }
                pregnantNow = 0;
            }
        }

        public void OnMenuChanged(object sender, MenuChangedEventArgs e) {
            if (Game1.farmEvent is QuestionEvent && Helper.Reflection.GetField<int>(Game1.farmEvent, "whichQuestion", true).GetValue() == 1) {
                if (e.NewMenu is DialogueBox dialogue) {
                    if (Config.PregnancyAllowed == true) {
                        PregnancyAllowedMethod("Pregnant", "Yes");
                        pregnantNow++;
                    }
                    else {
                        PregnancyAllowedMethod("Pregnancy cancelled", "Not");
                    }
                    dialogue.closeDialogue();
                }
            }
        }

        //not particularly necessary but i really like functions idk (even though this isnt a function bc its not returning anything lol)
        public void PregnancyAllowedMethod(string preg, string yon) {
            Monitor.Log(preg, LogLevel.Info);
            Response yes = new(yon, yon);
            Game1.currentLocation.answerDialogue(yes);
        }

    }
}