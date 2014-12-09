using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Monocle
{
    public class ChoiceSet<T>
    {
        public int TotalWeight { get; private set; }
        private Dictionary<T, int> choices;

        public ChoiceSet()
        {
            choices = new Dictionary<T, int>();
            TotalWeight = 0;
        }

        /// <summary>
        /// Sets the weight of a choice
        /// </summary>
        /// <param name="choice"></param>
        /// <param name="weight"></param>
        public void Set(T choice, int weight)
        {
            int oldWeight = 0;
            choices.TryGetValue(choice, out oldWeight);
            TotalWeight -= oldWeight;

            if (weight <= 0)
            {
                if (choices.ContainsKey(choice))
                    choices.Remove(choice);
            }
            else
            {
                TotalWeight += weight;
                choices[choice] = weight;
            }
        }

        /// <summary>
        /// Sets the weight of a choice, or gets its weight
        /// </summary>
        /// <param name="choice"></param>
        /// <returns></returns>
        public int this[T choice]
        {
            get
            {
                int weight = 0;
                choices.TryGetValue(choice, out weight);
                return weight;
            }

            set
            {
                Set(choice, value);
            }
        }

        /// <summary>
        /// Sets the chance of a choice. The chance is calculated based on the current state of ChoiceSet, so if
        /// other choices are changed later the chance will not be guaranteed to remain the same
        /// </summary>
        /// <param name="choice"></param>
        /// <param name="chance">A chance between 0 and 1.0f</param>
        public void Set(T choice, float chance)
        {
            int oldWeight = 0;
            choices.TryGetValue(choice, out oldWeight);
            TotalWeight -= oldWeight;

            int weight = (int)Math.Round(TotalWeight / (1f - chance));
            if (weight <= 0 && chance > 0)
                weight = 1;

            if (weight <= 0)
            {
                if (choices.ContainsKey(choice))
                    choices.Remove(choice);
            }
            else
            {
                TotalWeight += weight;
                choices[choice] = weight;
            }
        }

        /// <summary>
        /// Sets the chance of many choices. Takes the chance of any of the given choices being picked, not the chance of
        /// any individual choice. The chances are calculated based on the current state of ChoiceSet, so if
        /// other choices are changed later the chances will not be guaranteed to remain the same
        /// </summary>
        /// <param name="totalChance"></param>
        /// <param name="choices">A chance between 0 and 1.0f</param>
        public void SetMany(float totalChance, params T[] choices)
        {
            if (choices.Length > 0)
            {
                float chance = totalChance / choices.Length;

                int oldTotalWeight = 0;
                foreach (var c in choices)
                {
                    int oldWeight = 0;
                    this.choices.TryGetValue(c, out oldWeight);
                    oldTotalWeight += oldWeight;
                }
                TotalWeight -= oldTotalWeight;

                int weight = (int)Math.Round((TotalWeight / (1f - totalChance)) / choices.Length);
                if (weight <= 0 && totalChance > 0)
                    weight = 1;

                if (weight <= 0)
                {
                    foreach (var c in choices)
                        if (this.choices.ContainsKey(c))
                            this.choices.Remove(c);
                }
                else
                {
                    TotalWeight += weight * choices.Length;
                    foreach (var c in choices)
                        this.choices[c] = weight;
                }
            }
        }

        /// <summary>
        /// Chooses a random choice in the set
        /// </summary>
        /// <param name="random"></param>
        /// <returns></returns>
        public T Get(Random random)
        {
            int at = random.Next(TotalWeight);

            foreach (var kv in choices)
            {
                if (at < kv.Value)
                    return kv.Key;
                else
                    at -= kv.Value;
            }

            throw new Exception("Random choice error!");
        }

        /// <summary>
        /// Chooses a random choice in the set, using Calc.Random to choose
        /// </summary>
        /// <returns></returns>
        public T Get()
        {
            return Get(Calc.Random);
        }

        private struct Choice
        {
            public T Data;
            public int Weight;

            public Choice(T data, int weight)
            {
                Data = data;
                Weight = weight;
            }
        }
    }
}
