using System;
using System.Collections.Generic;
using System.Windows;

namespace ScratchEditor.misc
{
    public struct ID_Data
    {
        public long id;

        public ID_Data(long i)
        {
            id = i;
        }
    }
    public class IdManager
    {
        
        
        
        private static List<ID_Data> _guids = new List<ID_Data>();
        private static Random ra = new Random();
        public static ID_Data getGuid()
        {
            ID_Data g;
            do
            {
                g = getInt();
            } while (_guids.Contains(g));
            _guids.Add(g);
            return g;
        }

        private static unsafe ID_Data getInt()
        {
            ID_Data r = new ID_Data();
            for (int i = 0; i < sizeof(ID_Data); i++)
            {
                r.id = r.id * 10 + ra.Next(0, 9);
            }

            return r;
        }
        
        public void addGuid(ID_Data g)
        {
            _guids.Add(g);
        }

        public static ID_Data setId(ID_Data currentId, ID_Data newID)
        {
            if (_guids.Contains(newID))
                throw new RankException("Cant guid that was already added.");

            _guids[_guids.IndexOf(currentId)] = newID;
            return newID;
        }
    }
}