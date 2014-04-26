using System;
using System.Collections.Generic;

namespace Monocle
{
    public class CheatListener : Entity
    {
        public string Input;

        private List<Tuple<char, Func<bool>>> inputs;
        private List<Tuple<string, Action>> cheats;
        private int maxInput;

        public CheatListener()
        {
            Visible = false;
            Input = "";

            inputs = new List<Tuple<char, Func<bool>>>();
            cheats = new List<Tuple<string, Action>>();
        }

        public override void Update()
        {
            //Detect input
            bool changed = false;
            foreach (var input in inputs)
            {
                if (input.Item2())
                {
                    Input += input.Item1;
                    changed = true;
                }
            }

            //Handle changes
            if (changed)
            {
                if (Input.Length > maxInput)
                    Input = Input.Substring(Input.Length - maxInput);

                foreach (var cheat in cheats)
                {
                    if (Input.Contains(cheat.Item1))
                    {
                        Input = "";
                        if (cheat.Item2 != null)
                            cheat.Item2();
                        cheats.Remove(cheat);
                        break;
                    }
                }
            }
        }

        public void AddCheat(string code, Action onEntered = null)
        {
            cheats.Add(new Tuple<string, Action>(code, onEntered));
            maxInput = Math.Max(code.Length, maxInput);
        }

        public void AddInput(char id, Func<bool> checker)
        {
            inputs.Add(new Tuple<char, Func<bool>>(id, checker));
        }
    }
}
