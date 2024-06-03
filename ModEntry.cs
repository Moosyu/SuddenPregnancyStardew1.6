using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Events;
using StardewValley.Menus;

namespace SurpriseBaby1
{
    public class ModEntry : Mod
    {

        int pregnantNow = 0;

        public override void Entry(IModHelper helper)
        {
            helper.Events.Display.MenuChanged += this.OnMenuChanged;
            helper.Events.GameLoop.DayStarted += this.OnDayStarted;
        }
        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            if (pregnantNow == 1) {
                Game1.drawObjectDialogue("You and your partner are expecting!");
                pregnantNow = 0;
            }
        }

        public void OnMenuChanged(object sender, MenuChangedEventArgs e)
        {
            if (Game1.farmEvent is QuestionEvent && this.Helper.Reflection.GetField<int>(Game1.farmEvent, "whichQuestion", true).GetValue() == 1)
            {
                if (e.NewMenu is DialogueBox dialogue)
                {
                    this.Monitor.Log("Pregnant", LogLevel.Info);
                    Response yes = new("Yes", "Yes");
                    Game1.currentLocation.answerDialogue(yes);
                    dialogue.closeDialogue();
                    pregnantNow++;
                }
            }
        }
    }
}