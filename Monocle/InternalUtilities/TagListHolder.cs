using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Monocle
{
    public class TagListHolder
    {
        private EntityList[] lists;
        private int highest;

        public TagListHolder()
        {
            lists = new EntityList[Engine.MAX_TAG];
            highest = -1;
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

        public void SetLockMode(EntityList.LockModes lockMode)
        {
            for (int i = 0; i <= highest; i++)
                if (lists[i] != null)
                    lists[i].LockMode = lockMode;
        }
    }
}
