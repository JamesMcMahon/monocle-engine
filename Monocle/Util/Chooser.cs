using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Monocle
{
    /// <summary>
    /// Utility class for making weighted random choices from a set.
    /// </summary>
    public class Chooser<T>
    {
        private List<Choice> choices;

        public Chooser()
        {
            choices = new List<Choice>();
        }

        /// <summary>
        /// Initialize with a single choice with the given weight.
        /// </summary>
        public Chooser(T firstChoice, float weight)
            : this()
        {
            Add(firstChoice, weight);
        }

        /// <summary>
        /// Initialize with a list of choices, all with a weight of 1.
        /// </summary>
        public Chooser(params T[] choices)
            : this()
        {
            foreach (var choice in choices)
                Add(choice, 1);
        }

        public int Count
        {
            get
            {
                return choices.Count;
            }
        }

        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= Count)
                    throw new IndexOutOfRangeException();

                return choices[index].Value;
            }

            set
            {
                if (index < 0 || index >= Count)
                    throw new IndexOutOfRangeException();

                choices[index].Value = value;
            }
        }

        public Chooser<T> Add(T choice, float weight)
        {
            weight = Math.Max(weight, 0);
            choices.Add(new Choice(choice, weight));
            TotalWeight += weight;
            return this;
        }

        public T Choose()
        {
            if (TotalWeight <= 0)
                return default(T);
            else if (choices.Count == 1)
                return choices[0].Value;

            var roll = Calc.Random.NextDouble() * TotalWeight;
            float check = 0;

            for (int i = 0; i < choices.Count - 1; i++)
            {
                check += choices[i].Weight;
                if (roll < check)
                    return choices[i].Value;
            }

            return choices[choices.Count - 1].Value;
        }

        public float TotalWeight
        {
            get; private set;
        }

        public bool CanChoose
        {
            get
            {
                return TotalWeight > 0;
            }
        }

        private class Choice
        {
            public T Value;
            public float Weight;

            public Choice(T value, float weight)
            {
                Value = value;
                Weight = weight;
            }
        }

        /// <summary>
        /// Parses a chooser from a string.
        /// </summary>
        /// <param name="data">Choices to parse. Format: "choice0:weight,choice1:weight,..."</param>
        /// <returns></returns>
        public static Chooser<TT> FromString<TT>(string data) where TT : IConvertible
        {
            var chooser = new Chooser<TT>();
            string[] choices = data.Split(',');

            //If it's just a single choice with no weight, add it and return
            if (choices.Length == 1 && choices[0].IndexOf(':') == -1)
            {
                chooser.Add((TT)Convert.ChangeType(choices[0], typeof(TT)), 1f);
                return chooser;
            }

            //Parse the individual choices
            foreach (var choice in choices)
            {
                if (choice.IndexOf(':') == -1)
                {
                    //No weight, default to weight of 1
                    chooser.Add((TT)Convert.ChangeType(choice, typeof(TT)), 1f);
                }
                else
                {
                    //Has weight, handle that correctly
                    var parts = choice.Split(':');
                    var key = parts[0].Trim();
                    var weight = parts[1].Trim();

                    chooser.Add((TT)Convert.ChangeType(key, typeof(TT)), Convert.ToSingle(weight));
                }
            }

            return chooser;
        }
    }
}
