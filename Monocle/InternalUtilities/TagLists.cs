using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Monocle
{
    public class TagLists
    {
        private List<List<Entity>> lists;
        private List<bool> unsorted;
        private bool areAnyUnsorted;

        internal TagLists()
        {
            lists = new List<List<Entity>>();
            unsorted = new List<bool>();
        }

        public List<Entity> this[int index]
        {
            get
            {
                while (index >= lists.Count)
                {
                    lists.Add(new List<Entity>());
                    unsorted.Add(false);
                }

                if (lists[index] == null)
                    lists[index] = new List<Entity>();

                return lists[index];
            }
        }

        internal void MarkUnsorted(int tag)
        {
            areAnyUnsorted = true;
            unsorted[tag] = true;
        }

        internal void UpdateLists()
        {
            if (areAnyUnsorted)
            {
                for (int i = 0; i < lists.Count; i++)
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
            foreach (var tag in entity.Tags)
            {
                this[tag].Add(entity);
                areAnyUnsorted = true;
                unsorted[tag] = true;
            }
        }

        internal void EntityRemoved(Entity entity)
        {
            foreach (var tag in entity.Tags)
            {
                lists[tag].Remove(entity);
            }
        }
    }
}
