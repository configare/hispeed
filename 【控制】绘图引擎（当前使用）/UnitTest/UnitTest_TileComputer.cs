using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using GeoDo.RSS.Core.RasterDrawing;
using System.Drawing;

namespace UnitTest
{
    [TestFixture]
    public class UnitTest_TileComputer
    {
        private TileComputer _tileComputer = null;

        [SetUp]
        public void Init()
        {
            int tileSize = 512;
            int sampleRatio = 2;

            _tileComputer = new TileComputer(tileSize, sampleRatio);
        }

        [Test]
        public void ComputeAllLevels()
        {
            int width = 1024, height = 768;
            Console.WriteLine("(" + width.ToString() + "," + height.ToString() + "):" + _tileComputer.GetLevelCount(width, height).ToString());
            LevelDef[] levels = _tileComputer.GetLevelDefs(width, height);
            if (levels != null)
                foreach (LevelDef level in levels)
                    Console.WriteLine(level.ToString());
        }

        [Test]
        public void ComputeAllLevels_WithWnd()
        {
            int width = 1024, height = 768;
            Console.WriteLine("(" + width.ToString() + "," + height.ToString() + "):" + _tileComputer.GetLevelCount(width, height).ToString());
            LevelDef[] levels = _tileComputer.GetLevelDefs(width, height);
            if (levels != null)
                foreach (LevelDef level in levels)
                    Console.WriteLine(level.ToString());
            //
            int beginRow = 1000;
            int beginCol = 1000;
            int wndWidth = 1200;
            int wndHeight = 800;
            Console.WriteLine("Window:Location = ({0},{1}),Width = {2},Height = {3}", beginRow, beginCol, wndWidth, wndHeight);
            if (levels != null)
            {
                foreach (LevelDef level in levels)
                {
                    TileIdentify[] tiles = level.GetTileIdentifiesByOriginal(beginRow, beginCol, wndWidth, wndHeight);
                    Console.WriteLine("     LevelNo:" + level.LevelNo.ToString() + " , TotalTileCount:" + level.TileIdentities.Length.ToString() + " , TileCount:" + (tiles != null ? tiles.Length.ToString() : "0"));
                    if (tiles != null)
                        foreach (TileIdentify t in tiles)
                            Console.WriteLine("           " + t.ToString());
                }
            }
        }

        [Test]
        public void ComputeLevel()
        {
            int width = 1024, height = 768;
            //
            Console.WriteLine("(" + width.ToString() + "," + height.ToString() + "):" + _tileComputer.GetLevelCount(width, height).ToString());
            //
            width = 512; height = 512;
            Console.WriteLine("(" + width.ToString() + "," + height.ToString() + "):" + _tileComputer.GetLevelCount(width, height).ToString());
            //
            width = 512; height = 600;
            Console.WriteLine("(" + width.ToString() + "," + height.ToString() + "):" + _tileComputer.GetLevelCount(width, height).ToString());
            //
            width = 1025; height = 512;
            Console.WriteLine("(" + width.ToString() + "," + height.ToString() + "):" + _tileComputer.GetLevelCount(width, height).ToString());
            //
            width = 1024; height = 512;
            Console.WriteLine("(" + width.ToString() + "," + height.ToString() + "):" + _tileComputer.GetLevelCount(width, height).ToString());
            //
            width = 1023; height = 512;
            Console.WriteLine("(" + width.ToString() + "," + height.ToString() + "):" + _tileComputer.GetLevelCount(width, height).ToString());
        }

        [Test]
        public void ComputeSizeByLevel()
        {
            int width = 1024, height = 768;
            int levelCount = _tileComputer.GetLevelCount(width, height);
            for (int level = 0; level < levelCount; level++)
            {
                Size size = _tileComputer.GetSizeByLevel(level, width, height);
                Console.WriteLine("Level " + level.ToString() + ":" + size.ToString());
            }
        }


        [Test]
        public void ComputePlaceholder()
        {
            int size = 1024;
            Console.WriteLine("Placeholder: " + size.ToString() + " = " + _tileComputer.GetPlaceholder(size));
        }

        [Test]
        public void ComputeIdentifies()
        {
            int width = 1024;
            int height = 768;
            ComputeIdentities(width, height);
        }

        private void ComputeIdentities(int width, int height)
        {
            int rowCount = 0, colCount = 0;
            TileIdentify[] tiles = _tileComputer.GetTileIdentifies(width, height, out rowCount, out colCount);
            if (tiles == null)
                return;
            Console.WriteLine("TileCount = " + tiles.Count().ToString());
            Console.WriteLine("Width = " + width.ToString() + " , Height = " + height.ToString());
            foreach (TileIdentify tile in tiles)
            {
                int row = tile.Row;
                int col = tile.Col;
                string rowcol = string.Format("Row = {0} , Col = {1}", row.ToString().PadRight(3, ' '), col.ToString().PadRight(3, ' '));
                Console.WriteLine(rowcol + " => " + tile.ToString());
            }
        }

        [Test]
        public void ComputeIdentifiesOfAllLevels()
        {
            int width = 4297, height = 1890;
            int levelCount = _tileComputer.GetLevelCount(width, height);
            for (int level = 0; level < levelCount; level++)
            {
                Size size = _tileComputer.GetSizeByLevel(level, width, height);
                Console.WriteLine("Level " + level.ToString() + ":" + size.ToString());
                ComputeIdentities(size.Width, size.Height);
                Console.WriteLine("-".PadRight(140, '-'));
            }
        }

        [Test]
        public void ComputeAroundTiles()
        {
            int width = 4297, height = 1890;
            int centerRow = 2;
            int centerCol = 5;
            LevelDef[] levels = _tileComputer.GetLevelDefs(width, height);
            Size wndSize = new Size(1280, 1024);
            TileIdentify[] tiles = levels[0].GetAroundTiles(_tileComputer.TileSize, centerRow, centerCol, wndSize);
            Console.WriteLine(string.Format("(Row:{0},Col:{1}) WndSize({2},{3}) around tiles:", centerRow, centerCol, wndSize.Width, wndSize.Height));
            if (tiles == null)
                Console.WriteLine("none.");
            else
                foreach (TileIdentify tile in tiles)
                {
                    int row = tile.Row;
                    int col = tile.Col;
                    string rowcol = string.Format("Row = {0} , Col = {1}", row.ToString().PadRight(3, ' '), col.ToString().PadRight(3, ' '));
                    Console.WriteLine(rowcol + " => " + tile.ToString());
                }
        }
    }
}
