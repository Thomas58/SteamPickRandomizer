using System;
using System.Linq;
using System.Windows.Input;
using System.Timers;
using RandomizedSteamPick.ViewModel;
using RandomizedSteamPick.Canvas_Drawings;
using RandomizedSteamPick.Model;
using System.Collections.Generic;
using Steam.SteamObjects;
using System.Windows;

namespace RandomizedSteamPick.Commands
{
    public class RollCommand : ICommand
    {
        private static int[] iterationGroups = { 100, 50, 10 };
        private static int[] iterationInterval = { 10, 50, 100 };
        
        private Timer RollTimer = new Timer();
        private int iteration = 0;
        private int iterations = iterationGroups[0];
        private int index;
        private List<Game> list;
        private List<Game> rollList;

        private RollAnimation animation;

        public event EventHandler CanExecuteChanged;

        public RollCommand(RollAnimation animation, SteamList list)
        {
            this.list = list.GameList;
            this.animation = animation;
            RollTimer.Elapsed += new ElapsedEventHandler(Roll_Tick);
        }

        public bool CanExecute(object parameter) => 0 < list.Count && !RollTimer.Enabled;

        public void Execute(object parameter)
        {
            int comboboxIndex = (int)parameter;
            if (comboboxIndex == 2)
                this.rollList = list.Where(x => x.PlaytimeForever == 0).ToList();
            else if (comboboxIndex == 1)
            {
                MessageBox.Show("Not recently played, not implemented.");
                return;
            }
            else
                this.rollList = list;

            // Roll a name and save the result to the file.
            var RollNumber = Roll(rollList.Count);
            
            // Calculate starting index for roll animation.
            index = Mod(RollNumber - iterationGroups.Sum(), rollList.Count);

            // Start the animation.
            animation.Start(RollNumber);
            iteration = 0;
            iterations = iterationGroups[0];
            RollTimer.Interval = iterationInterval[0];
            RollTimer.Start();
        }

        private int Roll(int count)
        {
            int rollNumber = MainViewModel.Random.Next(count);
            return rollNumber;
        }

        private void Roll_Tick(object sender, ElapsedEventArgs e)
        {
            if (iterations <= 0)
            {
                iteration++;
                if (iterationGroups.Length <= iteration)
                {
                    RollTimer.Stop();
                    animation.Finish(index);
                }
                else
                {
                    iterations = iterationGroups[iteration];
                    RollTimer.Interval = iterationInterval[iteration];
                    animation.Update(index);
                }
            }
            else
            {
                animation.Update(index);
            }
            iterations--;
            index++;
            if (rollList.Count <= index)
                index = 0;
        }

        private int Mod(int x, int m)
        {
            if (m == 0) return 0;
            return (x % m + m) % m;
        }
    }
}
