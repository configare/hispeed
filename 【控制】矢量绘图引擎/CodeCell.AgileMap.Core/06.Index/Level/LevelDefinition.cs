using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeCell.AgileMap.Core
{
    public class LevelItem
    {
        public int Level = -1;
        public string Name = null;
        public int Scale = 0;

        public LevelItem()
        { 
        }

        public LevelItem(int level, string name, int scale)
        {
            Level = level;
            Name = name;
            Scale = scale;
        }
    }

    public class LevelDefinition
    {
        private static LevelItem[] _levelItems = null;

        static LevelDefinition()
        {
            /*
                级数     名称       比例尺
                0
                1     2000KM       1:6000 0000
                2     1000KM       1:3000 0000
                3       500KM       1:1500 0000
                4       100KM       1:300  0000
                5         70KM       1:210  0000
                6         30KM       1:90   0000
                7         15KM       1:45   0000
                8           9KM       1:27   0000
                9           8KM       1:24   0000
                10         4KM       1:12   0000
                11         2KM       1:6    0000
                12         1KM       1:3    0000
                13       500M       1:15000                          
                14       250M       1:7500
                15       100M       1:3000     
             */
            _levelItems = new LevelItem[] 
                                 {
                                     new LevelItem(0,string.Empty,0),
                                     new LevelItem(1,"2000KM",60000000),
                                     new LevelItem(2,"1000KM",30000000),
                                     new LevelItem(3,"500KM",15000000),
                                     new LevelItem(4,"100KM",3000000),
                                     new LevelItem(5,"70KM",2100000),
                                     new LevelItem(6,"30KM",900000),
                                     new LevelItem(7,"15KM",450000),
                                     new LevelItem(8,"9KM",270000),
                                     new LevelItem(9,"8KM",240000),
                                     new LevelItem(10,"4KM",120000),
                                     new LevelItem(11,"2KM",60000),
                                     new LevelItem(12,"1KM",30000),
                                     new LevelItem(13,"500M",15000),
                                     new LevelItem(14,"250M",7500),
                                     new LevelItem(15,"100M",3000)
                                 };
        }

        public static LevelItem[] LevelItems
        {
            get { return _levelItems; }
        }

        public static int GetScaleByLevel(int level)
        {
            int idx = 0;
            for (int i = 0; i < _levelItems.Length; i++)
            {
                if (_levelItems[i].Level == level)
                {
                    idx = i;
                    break;
                }
            }
            if (idx == 0)
                return 70000000;
            else if (idx == _levelItems.Length - 1)
                return 1500;
            else
                return (_levelItems[idx].Scale + _levelItems[idx + 1].Scale) / 2;
        }

        public static int GetLevelByScale(int scale)
        {
            LevelItem it = GetLevelItemByScale(scale);
            return it != null ? it.Level : -1;
        }

        public static LevelItem GetLevelItemByScale(int scale)
        {
            if (scale < _levelItems[_levelItems.Length - 1].Scale)
                return _levelItems[_levelItems.Length - 1];
            if (scale >= _levelItems[1].Scale)
                return _levelItems[0];
            for (int i = _levelItems.Length - 1; i > 1; i--)
                if (scale >= _levelItems[i].Scale && scale < _levelItems[i - 1].Scale)
                    return _levelItems[i-1];
            return null;
        }

    }
}
