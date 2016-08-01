using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Monocle
{
    public class TagLists
    {
        private List<Entity>[] lists;
        private bool[] unsorted;
        private bool areAnyUnsorted;

        internal TagLists()
        {
            lists = new List<Entity>[BitTag.TotalTags];
            unsorted = new bool[BitTag.TotalTags];
            for (int i = 0; i < lists.Length; i++)
                lists[i] = new List<Entity>();
        }

        public List<Entity> this[int index]
        {
            get
            {
                return lists[index];
            }
        }

        internal void MarkUnsorted(int index)
        {
            areAnyUnsorted = true;
            unsorted[index] = true;
        }

        internal void UpdateLists()
        {
            if (areAnyUnsorted)
            {
                for (int i = 0; i < lists.Length; i++)
                {
                    if (unsorted[i])
                    {
                        lists[i].Sort(EntityList.CompareDepth);
                        unsorted[i] = false;
                    }
                }

                areAnyUnsorted = false;
            }
        }

        internal void EntityAdded(Entity entity)
        {
            for (int i = 0; i < BitTag.TotalTags; i++)
            {
                if (entity.TagCheck(1 << i))
                {
                    this[i].Add(entity);
                    areAnyUnsorted = true;
                    unsorted[i] = true;
                }
            }
        }

        internal void EntityRemoved(Entity entity)
        {
            for (int i = 0; i < BitTag.TotalTags; i++)
                if (entity.TagCheck(1 << i))
                    lists[i].Remove(entity);
        }
    }
}
