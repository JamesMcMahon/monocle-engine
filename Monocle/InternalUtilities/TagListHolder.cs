using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Monocle
{
    public class TagListHolder : IEnumerable<EntityList>, IEnumerable
    {
        private EntityList[] lists;
        private int highest;

        internal TagListHolder()
        {
            lists = new EntityList[Engine.MAX_TAG];
            highest = -1;
        }

        internal void SetLockMode(EntityList.LockModes lockMode)
        {
            for (int i = 0; i <= highest; i++)
                if (lists[i] != null)
                    lists[i].LockMode = lockMode;
        }

        public EntityList this[int index]
        {
            get
            {
                if (lists[index] == null)
                {
                    lists[index] = new EntityList();
                    highest = Math.Max(highest, index);
                }
                return lists[index];
            }
        }

        public IEnumerator<EntityList> GetEnumerator()
        {
            for (int i = 0; i <= highest; i++)
                if (lists[i] != null)
                    yield return lists[i];
        }

        IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }


    }
}
